using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using YusurIntegration.Data;
using YusurIntegration.DTOs;
using YusurIntegration.Hubs;
using YusurIntegration.Models;
using YusurIntegration.Services.Interfaces;
//using static YusurIntegration.DTOs.YusurPayloads;
namespace YusurIntegration.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;
        private readonly IYusurApiClient _yusur;
        private readonly IOrderValidationService _orderValidationService;
        private readonly IHubContext<YusurHub> _hub;
        private readonly ILogger<OrderService> _logger;
        private readonly ConnectionManager _cn;

        public OrderService(AppDbContext db, IYusurApiClient yusur,
            IOrderValidationService ordervalidation, 
            IHubContext<YusurHub> yhub ,
            ILogger<OrderService> logger,
            ConnectionManager cn )
        {
            _db = db;
            _yusur = yusur;
            _orderValidationService = ordervalidation;
            _hub = yhub;
            _logger = logger;
            _cn = cn;

        }

        public async Task HandleNewOrderAsync(YusurPayloads.NewOrderDto dto)
        {
            _logger.LogInformation($"Handling new order: {dto.orderId} for branch: {dto.branchLicense}");

            var payload = System.Text.Json.JsonSerializer.Serialize(dto);
            string connteted =_cn.IsConnected(dto.branchLicense) ? "YES" : "NO";

            await _db.WebhookLogs.AddAsync(new Models.WebhookLog { WebhookType = "notifyNewOrder",OrderId = dto.orderId, BranchConnected =connteted, Payload = payload, Status = "PENDING_VALIDATION", BranchLicense = dto.branchLicense });
            await _db.SaveChangesAsync();

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
                    memberId = dto.patient.memberId,
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
                Activities = new List<Activity>()
            };

            foreach (var act in dto.activities ?? new())
            {
                var activity = new Activity
                {
                    ActivityId = act.id,
                    GenericCode = act.genericCode,
                    Instructions = act.instructions,
                    ArabicInstructions = act.arabicInstructions,
                    Duration = act.duration,
                    Refills = act.refills,
                    TradeDrugs = new List<TradeDrug>()
                };

                if (act.tradeDrugs != null)
                {
                    foreach (var td in act.tradeDrugs)
                    {
                        activity.TradeDrugs.Add(new TradeDrug { Code = td.code, Name = td.name, Quantity = td.quantity });
                    }
                }

               order.Activities.Add(activity);
            }
          
            string _orderstatus = "RECEIVED";

            // basic auto allocation: pick first trade drug of each activity as selected
            foreach (var activity in order.Activities)
            {

                var tradeDrugsList = activity.TradeDrugs.Select(x => new TradeDrug
                {
                    Code = x.Code,
                    Quantity = x.Quantity
                }).ToList();

                ActivityValidationResultDto te = await _orderValidationService.ValidateActivityAsync(
                    order.BranchLicense,
                    DateTime.Now,
                    tradeDrugsList);

                if (te != null)
                {
                    activity.SelectedTradeCode = te.DrugCode ?? string.Empty;
                    activity.Itemno = te.ItemNo ?? string.Empty;

                    if (te.IsValid)
                    {
                        activity.SelectedQuantity = te.Quantity;
                    }
                    else
                    {
                        activity.SelectedQuantity = 0;
                       _orderstatus ="STOCK NOT_AVAILABLE";
                        break;
                    }
                }
            }

            order.Status = _orderstatus;
           _db.Orders.Add(order);

            await _db.SaveChangesAsync();

            if (_orderstatus == "RECEIVED")
            {
                var activities = order.Activities
                      .Select(a => new YusurPayloads.AcceptActivityDto(
                          id: a.ActivityId,
                          tradeCode: a.SelectedTradeCode!,
                          Quantity: (int)a.SelectedQuantity
                      ))
                      .ToList();
                var request = new YusurPayloads.OrderAcceptRequestDto(order.OrderId, activities);
                var success =  await _yusur.AcceptOrderAsync(request);

            }
            else
            {

                var request = new YusurPayloads.OrderRejectRequestDto(order.OrderId, "Stock not available for one or more items");
                await _yusur.RejectOrderAsync(request);
            }

        }

        public async Task HandleOrderAllocationAsync(YusurPayloads.OrderAllocationDto dto)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == dto.orderId);
            if (order == null) return;

            foreach (var act in dto.activities ?? new())
            {
                var a = order.Activities.FirstOrDefault(x => x.ActivityId == act.id);
                if (a != null)
                {
                    a.SelectedTradeCode = act.tradeCode;
                }
            }
            order.Status = "WAITING_WASFATY_APPROVAL";
            await _db.SaveChangesAsync();
        }


        public async Task<(bool Success, string? ErrorMessage, Order? Data)> HandleAuthorizationResponseAsync(YusurPayloads.AuthorizationResponseDto dto)
        {
            var payload = System.Text.Json.JsonSerializer.Serialize(dto);
            await _db.WebhookLogs.AddAsync(new Models.WebhookLog { WebhookType = "notifyAuthorizationResponseReceived", Payload = payload, BranchLicense = dto.branchLicense });
            await _db.SaveChangesAsync();

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var order = await _db.Orders.Include(o => o.Activities).ThenInclude(a => a.TradeDrugs).FirstOrDefaultAsync(o => o.OrderId == dto.orderId);
                if (order == null) return (false, "Order not found", null);

                order.failureReason = dto.failureReason;

                foreach (var act in dto.activities ?? new())
                {
                    var a = order.Activities.FirstOrDefault(x => x.ActivityId == act.id);
                    if (a == null) continue;
                    // map authStatus
                    if (act.authStatus == "APPROVED")
                    {
                        a.Isapproved = true;
                    }
                    else if (act.authStatus == "REJECTED")
                    {
                        a.SelectedQuantity = 0;
                        a.rejectionReason = act.rejectionReason;
                    }
                }
                order.Status = dto.status;


                var PendingMessage = new PendingMessage
                {
                    MessageId = Guid.NewGuid().ToString(),
                    BranchLicense = dto.branchLicense,
                    MessageType = "notifyAuthorizationResponseReceived",
                    PayloadJson = payload,
                    CreatedAt = DateTime.UtcNow
                };
                _db.PendingMessages.Add(PendingMessage);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                await _hub.Clients.Group(dto.branchLicense).SendAsync("notifyAuthorizationResponseReceived", new
                {
                    Id = PendingMessage.Id,
                    OrderId = dto.orderId,
                    Status = dto.status
                });

                return (true, null, order);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return (false, "Exception occurred", null);
            }
        }
        public async Task HandleStatusUpdateAsync(YusurPayloads.StatusUpdateDto dto)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == dto.orderId);
            if (order == null) return;
            var internalstatus = OrderStatusMapper.FromYusur(dto.status);
            order.Status = dto.status;
            _db.OrderStatusHistory.Add(new OrderStatusHistory { OrderId = order.OrderId, Status = internalstatus,FailureReason = dto.failureReason });
            await _db.SaveChangesAsync();
        }
    }
}
