using Microsoft.EntityFrameworkCore;
using YusurIntegration.Data;
using YusurIntegration.DTOs;
using YusurIntegration.Models;
using YusurIntegration.Services.Interfaces;
using static YusurIntegration.DTOs.YusurPayloads;
namespace YusurIntegration.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;
        private readonly YusurApiClient _yusur;
        readonly IOrderValidationService _orderValidationService;

        public OrderService(AppDbContext db, YusurApiClient yusur,IOrderValidationService ordervalidation)
        {
            _db = db;
            _yusur = yusur;
            _orderValidationService = ordervalidation;

        }

        public async Task HandleNewOrderAsync(NewOrderDto dto)
        {
            // create order
            var order = new Order
            {
                OrderId = dto.orderId,
                VendorId = dto.vendorId,
                BranchLicense = dto.branchLicense,
                ErxReference = dto.erxReference,
                IsPickup = dto.isPickup,
                Status = "RECEIVED",
                Patient = dto.patient != null ? new Patient
                {
                    firstName = dto.patient.firstName,
                    nationalId = dto.patient.nationalId,
                    lastName = dto.patient.lastName,
                    gender = dto.patient.gender,
                    bloodGroup = dto.patient.bloodGroup
                } : null,
                ShippingAddress = dto.shippingAddress != null ? new ShippingAddress
                {
                    addressLine1 = dto.shippingAddress.addressLine1,
                    addressLine2 = dto.shippingAddress.addressLine2,
                    area = dto.shippingAddress.area,
                    city = dto.shippingAddress.city,
                    Coordinates = dto.shippingAddress.coordinates != null ? new Coordinates
                    {
                        latitude = dto.shippingAddress.coordinates != null ? dto.shippingAddress.coordinates.latitude : 0,
                        longitude = dto.shippingAddress.coordinates != null ? dto.shippingAddress.coordinates.longitude : 0
                    } : new Coordinates()
                } : null,   

            };

            foreach (var act in dto.activities ?? new())
            {
                var a = new Activity
                {
                    ActivityIdFromYusur = act.id,
                    GenericCode = act.genericCode,
                    Instructions = act.instructions,
                    ArabicInstructions = act.arabicInstructions,
                    Duration = act.duration,
                    Refills = act.refills
                };

                if (act.tradeDrugs != null)
                {
                    foreach (var td in act.tradeDrugs)
                    {
                        a.TradeDrugs.Add(new TradeDrugs { Code = td.code, Name = td.name, Quantity = td.quantity });
                    }
                }

               order.Activities.Add(a);
            }


            _db.Orders.Add(order);
            await _db.SaveChangesAsync();


            // basic auto allocation: pick first trade drug of each activity as selected
            foreach (var a in order.Activities)
            {
 
                var tradeDrugsList = a.TradeDrugs.Select(x => new TradeDrugs
                {
                    Code = x.Code,
                    Quantity = x.Quantity
                }).ToList();

                ActivityValidationResultDto te = await _orderValidationService.ValidateActivityAsync(
                    order.BranchLicense,
                    DateTime.Now,
                    tradeDrugsList);

                if (te.IsValid)
                {
                    a.SelectedTradeCode = te.DrugCode;
                    a.SelectedQuantity = te.Quantity;
                    a.Itemno = te.ItemNo;
                }
            }

            order.Status = "ACCEPTED_BY_PROVIDER";
            _db.OrderStatusHistory.Add(new OrderStatusHistory { OrderId = order.Id, Status = order.Status });
            await _db.SaveChangesAsync();

            // call Yusur to accept order (send activities mapping)
            var activitiesForYusur = order.Activities.Select(x => new { id = x.ActivityIdFromYusur, tradeCode = x.SelectedTradeCode, quantity = x.SelectedQuantity }).ToList();
            await _yusur.AcceptOrderAsync(order.OrderId, activitiesForYusur);
        }

        public async Task HandleOrderAllocationAsync(OrderAllocationDto dto)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == dto.orderId);
            if (order == null) return;

            foreach (var act in dto.activities ?? new())
            {
                var a = order.Activities.FirstOrDefault(x => x.ActivityIdFromYusur == act.id);
                if (a != null)
                {
                    a.SelectedTradeCode = act.tradeCode;
                }
            }

            order.Status = "WAITING_ERX_HUB_APPROVAL";
            _db.OrderStatusHistory.Add(new OrderStatusHistory { OrderId = order.Id, Status = order.Status });
            await _db.SaveChangesAsync();
        }

        public async Task HandleAuthorizationResponseAsync(AuthorizationResponseDto dto)
        {
            var order = await _db.Orders.Include(o => o.Activities).ThenInclude(a => a.TradeDrugs).FirstOrDefaultAsync(o => o.OrderId == dto.orderId);
            if (order == null) return;

            // update activities status
            foreach (var act in dto.activities ?? new())
            {
                var a = order.Activities.FirstOrDefault(x => x.ActivityIdFromYusur == act.id);
                if (a == null) continue;

                // map authStatus
                if (act.authStatus == "APPROVED")
                {
                    // keep selected trade code
                }
                else if (act.authStatus == "REJECTED")
                {
                    // mark quantity 0 to indicate not allowed
                    a.SelectedQuantity = 0;
                }
            }

            order.Status = dto.status;
            _db.OrderStatusHistory.Add(new OrderStatusHistory { OrderId = order.Id, Status = order.Status });
            await _db.SaveChangesAsync();
        }

        public async Task HandleStatusUpdateAsync(StatusUpdateDto dto)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == dto.orderId);
            if (order == null) return;
            order.Status = dto.status;
            _db.OrderStatusHistory.Add(new OrderStatusHistory { OrderId = order.Id, Status = order.Status });
            await _db.SaveChangesAsync();
        }
    }
}
