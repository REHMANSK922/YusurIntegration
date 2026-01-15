using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using YusurIntegration.Data;
using YusurIntegration.DTOs;
using YusurIntegration.Helpers;
using YusurIntegration.Hubs;
using YusurIntegration.Models;
using YusurIntegration.Models.Enums;
using YusurIntegration.Services.Interfaces;
using static YusurIntegration.DTOs.YusurPayloads;
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
        private readonly OrderResponseTracker _orderResponseTracker;
        public OrderService(AppDbContext db, 
                    IYusurApiClient yusur,
                    IOrderValidationService ordervalidation,
                    IHubContext<YusurHub> yhub,
                    ILogger<OrderService> logger,
                    ConnectionManager cn,
                    OrderResponseTracker orderResponseTracker
                          )
        {
            _db = db;
            _yusur = yusur;
            _orderValidationService = ordervalidation;
            _hub = yhub;
            _logger = logger;
            _cn = cn;
            _orderResponseTracker = orderResponseTracker;

        }
        public async Task HandleNewOrderAsync_old(NewOrderDto dto)
        {
            _logger.LogInformation($"Handling new order: {dto.orderId} for branch: {dto.branchLicense}");

          
            string connteted = _cn.IsConnected(dto.branchLicense) ? "YES" : "NO";
            var payload = System.Text.Json.JsonSerializer.Serialize(dto);
            await _db.WebhookLogs.AddAsync(new Models.WebhookLog { WebhookType = "notifyNewOrder", 
                OrderId = dto.orderId, 
                BranchConnected = connteted, 
                Payload = payload, 
                Status = "PENDING_VALIDATION", BranchLicense = dto.branchLicense });
            await _db.SaveChangesAsync();
            var order = Helpers.OrderMapping.MapDtoToOrder(dto);

            if (!_cn.IsConnected(dto.branchLicense))
            {
                _logger.LogWarning($"Branch {dto.branchLicense} is offline. Auto-rejecting order {dto.orderId}.");
                var rejectRequest = new OrderRejectRequestDto(dto.orderId, "Pharmacy POS Offline/No Connection");
                await _yusur.RejectOrderAsync(rejectRequest);

                //sending admin that branch is offline added for monitoring this option added in feature ====>

                // 2. Log for Admins
                await _hub.Clients.Group("Admins").SendAsync("PosOfflineOrderReject", order);

                return;
            }


            /*
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
                        activity.TradeDrugs.Add(new TradeDrug { Code = td.code, Name = td.name, Quantity = td.quantity, ActivityId = act.id });
                    }
                }

                order.Activities.Add(activity);
            }

            */


            string _orderstatus = "RECEIVED";

            // stopped due to non syncing of stock .
            /*
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
            */
           
            order.Status = _orderstatus;
            _db.Orders.Add(order);

            await _db.SaveChangesAsync();

         // Stopped due to non syncing of stock .

            /*
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
                var success = await _yusur.AcceptOrderAsync(request);
                if (success)
                {
                    _logger.LogInformation($"Order {order.OrderId} send successfully.");
                    await _hub.Clients.Group(dto.branchLicense).SendAsync("OrderSubmitted", new
                    {
                        Order = order,
                        Status = "ACCEPTED_BY_PROVIDER"
                    });

                }
                else
                {
                    _logger.LogWarning($"Failed to accept order {order.OrderId}.");

                }
            }
            else
            {

                var request = new YusurPayloads.OrderRejectRequestDto(order.OrderId, "Stock not available for one or more items");
                await _yusur.RejectOrderAsync(request);
            }
            */

            _logger.LogInformation($"Order {order.OrderId} send successfully.");
            _logger.LogInformation($"DEBUG: Attempting to send Order {order.OrderId} to Group: '{dto.branchLicense}'");

            //await _hub.Clients.Group(dto.branchLicense).SendAsync("ReceiveOrderWithOutStockCheck", new
            //{
            //    Order = order
            //});
            //await _hub.Clients.Group(dto.branchLicense).SendAsync("ReceiveOrderWithOutStockCheck", order);

            await _hub.Clients.Group(dto.branchLicense).SendAsync("ReceiveOrderWithOutStockCheck", dto);
        }
        public async Task HandleNewOrderAsync_v1(NewOrderDto dto)
        {
            _logger.LogInformation($"New order {dto.orderId}. Checking branch {dto.branchLicense} connectivity...");

            // 1. Initial State Check
            var branchStatus = _cn.GetBranchStatus(dto.branchLicense); // Assuming this returns an object with IsConnected and LastSeen
            bool isCurrentlyConnected = branchStatus.IsConnected;
            DateTime? lastSeen = branchStatus.LastSeen;

            // 2. RECONNECT GRACE PERIOD
           // If offline, but was seen within the last 60 seconds, wait 15s to see if they reconnect
            if (!isCurrentlyConnected && lastSeen.HasValue && (DateTime.UtcNow - lastSeen.Value).TotalSeconds < 60)
            {
                _logger.LogInformation($"Branch {dto.branchLicense} recently offline. Waiting 15s for reconnection...");
              // Wait up to 15 seconds for IsConnected to become true
                for (int i = 0; i < 15; i++)
                {
                    await Task.Delay(1000); // Check every second
                    if (_cn.IsConnected(dto.branchLicense))
                    {
                        isCurrentlyConnected = true;
                        _logger.LogInformation($"Branch {dto.branchLicense} reconnected! Proceeding...");
                        break;
                    }
                }
            }
          // 3. HARD REJECT (If still offline after grace period)
            if (!isCurrentlyConnected)
            {
                _logger.LogWarning($"Branch {dto.branchLicense} is strictly offline. Auto-rejecting order {dto.orderId}.");
                await AutoRejectOrder(dto.orderId, "Pharmacy POS Offline/No Connection");
                var payload = System.Text.Json.JsonSerializer.Serialize(dto);

                // 2. Log for Admins
                await _hub.Clients.Group("Admins").SendAsync("PosOfflineOrderReject", dto.orderId);

                string connteted = isCurrentlyConnected ? "YES" : "NO";

                await _db.WebhookLogs.AddAsync(new WebhookLog
                {
                    WebhookType = "notifyNewOrder",
                    OrderId = dto.orderId,
                    BranchConnected = connteted,
                    Payload = payload,
                    Status = "REJECTED DUE TO OFFLINE",
                    BranchLicense = dto.branchLicense
                });
                await _db.SaveChangesAsync();
                return;
            }
          // At this point, branch is confirmed and no need to save order again to avoid delay on server side, transfer order to branch directly.

            /*
            // 4. NORMAL WORKFLOW (Branch is Online)
            var order = Helpers.OrderMapping.MapDtoToOrder(dto);
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            */

            //await _hub.Clients.Group(dto.branchLicense).SendAsync("ReceiveOrderWithOutStockCheck", dto);


            _logger.LogInformation($"Branch {dto.branchLicense} is online. Sending order {dto.orderId} to POS...");
            await _hub.Clients.Group(dto.branchLicense).SendAsync("NewOrderReceivedSendToPos", dto);

            //Saving the order locally after sending to POS to reduce delay time for POS receiving order



            // 5. WAIT FOR PHARMACIST RESPONSE
            // Now wait for the pharmacist to actually click 'Accept' (30 seconds)

            string orderid = dto.orderId;
            string branchno = dto.branchLicense;

            var clientResponded = await _orderResponseTracker.WaitForClientDataAsync(orderid, "Acceptance", TimeSpan.FromSeconds(45));
            if (clientResponded==null)
            {
                _logger.LogWarning($"No action taken by Pharmacist on {dto.orderId} within 30s. Auto-rejecting.");
                await AutoRejectOrder(dto.orderId, "No response from pharmacist (Interaction Timeout)");
            }
            else
            {
                _logger.LogWarning($"Order accepted Pharmacist on {dto.orderId} within 30s. Auto-rejecting.");
                await HandleSendYusurOrderAccept(clientResponded,branchno);
            }
        }
        public async Task HandleNewOrderAsync(NewOrderDto dto)
        {
            _logger.LogInformation($"New order {dto.orderId}. Checking branch {dto.branchLicense} connectivity...");

            // 1. Initial State Check
            var branchStatus = _cn.GetBranchStatus(dto.branchLicense); // Assuming this returns an object with IsConnected and LastSeen
            bool isCurrentlyConnected = branchStatus.IsConnected;
            DateTime? lastSeen = branchStatus.LastSeen;

            // 2. RECONNECT GRACE PERIOD  If offline, but was seen within the last 60 seconds, wait 15s to see if they reconnect
            if (!isCurrentlyConnected && lastSeen.HasValue && (DateTime.UtcNow - lastSeen.Value).TotalSeconds < 60)
            {
                _logger.LogInformation($"Branch {dto.branchLicense} recently offline. Waiting 15s for reconnection...");
                // Wait up to 15 seconds for IsConnected to become true
                for (int i = 0; i < 15; i++)
                {
                    await Task.Delay(1000); // Check every second
                    if (_cn.IsConnected(dto.branchLicense))
                    {
                        isCurrentlyConnected = true;
                        _logger.LogInformation($"Branch {dto.branchLicense} reconnected! Proceeding...");
                        break;
                    }
                }
            }
            // 3. HARD REJECT (If still offline after grace period)
            if (!isCurrentlyConnected)
            {
                _logger.LogWarning($"Branch {dto.branchLicense} is strictly offline. Auto-rejecting order {dto.orderId}.");
                await AutoRejectOrder(dto.orderId, "Pharmacy POS Offline/No Connection");
                var payload = System.Text.Json.JsonSerializer.Serialize(dto);

                // 2. Log for Admins
                await _hub.Clients.Group("Admins").SendAsync("PosOfflineOrderReject",$"Branch {dto.branchLicense} is strictly offline. Auto-rejecting order {dto.orderId}.");

                string connteted = isCurrentlyConnected ? "YES" : "NO";
                await _db.WebhookLogs.AddAsync(new WebhookLog
                {
                    WebhookType = "notifyNewOrder",
                    OrderId = dto.orderId,
                    BranchConnected = connteted,
                    Payload = payload,
                    Status = "REJECTED DUE TO OFFLINE",
                    BranchLicense = dto.branchLicense
                });
                await _db.SaveChangesAsync();
                return;
            }
            // 4. NORMAL WORKFLOW (Branch is Online)
            // At this point, branch is confirmed and no need to save order again to avoid delay on server side, transfer order to branch directly.

            _logger.LogInformation($"Branch {dto.branchLicense} is online. Sending order {dto.orderId} to POS...");

            await _hub.Clients.Group(dto.branchLicense).SendAsync("NewOrderReceivedSendToPos", dto);
            Order order = OrderMapping.MapDtoToOrder(dto);
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // 5. WAIT FOR PHARMACIST RESPONSE
            // Now wait for the pharmacist to actually click 'Accept' (45 seconds)

            string orderid = dto.orderId;
            string branchno = dto.branchLicense;

            var clientResponded = await _orderResponseTracker.WaitForClientDataAsync(orderid, "Acceptance", TimeSpan.FromSeconds(45));
            if (clientResponded == null)
            {
                _logger.LogWarning($"No action taken by Pharmacist on {dto.orderId} within 30s. Auto-rejecting.");
                await AutoRejectOrder(dto.orderId, "No response from pharmacist (Interaction Timeout)");
            }
            else
            {
                _logger.LogWarning($"Order accepted Pharmacist on {dto.orderId} within 30s. Auto-rejecting.");
                await HandleSendYusurOrderAccept(clientResponded, branchno);
            }
        }
        public async Task HandleOrderAllocationAsync(OrderAllocationDto dto)
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
            await _hub.Clients.Group(dto.branchLicense).SendAsync("OnOrderOrderAllocation", dto);
        }
        public async Task<(bool Success, string? ErrorMessage, Order? Data)> HandleAuthorizationResponseAsync(AuthorizationResponseDto dto)
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
        public async Task HandleStatusUpdateAsync(StatusUpdateDto dto)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == dto.orderId);
            if (order == null) return;
            var internalstatus = dto.status;
            order.Status = dto.status;
            _db.OrderStatusHistory.Add(new OrderStatusHistory { OrderId = order.OrderId, Status = internalstatus, FailureReason = dto.failureReason });
            await _db.SaveChangesAsync();

            //await _hub.Clients.Group(dto.branchLicense).SendAsync("OnOrderStatusUpdated", new
            //{
            //    OrderId = order.OrderId,
            //    Status = dto.status,
            //    FailureReason = dto.failureReason
            //});

            await _hub.Clients.Group(dto.branchLicense).SendAsync("OnOrderStatusUpdated",dto);

            //await _hub.Clients.Group(dto.branchLicense).SendAsync("NewOrderReceivedSendToPos", dto);


            //switch (dto.status)
            //{
            //    case "INACTIVE":
            //        await _hub.Clients.Group(dto.branchLicense).SendAsync("OrderAcceptedByOther", new { OrderId = order.OrderId, Status = dto.status });
            //        break;
            //    case "ACCEPTED_BY_PROVIDER":
            //        await _hub.Clients.Group(dto.branchLicense).SendAsync("OrderAccepted", new { OrderId = order.OrderId, Status = dto.status});
            //        break;
            //    case "REJECTED_BY_PROVIDER":
            //        await _hub.Clients.Group(dto.branchLicense).SendAsync("OrderRejectedByProvider", new { OrderId = order.OrderId, Status = dto.status });
            //        break;
            //    case "CANCELLED_BY_PROVIDER": // order cannelled by pharmacy
            //        await _hub.Clients.Group(dto.branchLicense).SendAsync("OrderCancelledSucess", new { OrderId = order.OrderId, Status = dto.status });
            //        break;
            //    case "ERX_HUB_AUTH_SUBMIT_TIMED_OUT": // authorization request not sent within time
            //        await _hub.Clients.Group(dto.branchLicense).SendAsync("OrderCancelledSucess", new { OrderId = order.OrderId, Status = dto.status });
            //        break;
            //    case "WAITING_ERX_HUB_APPROVAL": // authorization request not sent within time
            //        await _hub.Clients.Group(dto.branchLicense).SendAsync("OrderCancelledSucess", new { OrderId = order.OrderId, Status = dto.status });
            //        break;

            //    case "ERX_HUB_TIMED_OUT": // authorization request not sent within time
            //        await _hub.Clients.Group(dto.branchLicense).SendAsync("OrderCancelledSucess", new { OrderId = order.OrderId, Status = dto.status });
            //        break;

            //    case "ERX_HUB_CLAIM_SUBMIT_TIMED_OUT": // authorization request not sent within time
            //        await _hub.Clients.Group(dto.branchLicense).SendAsync("OrderCancelledSucess", new { OrderId = order.OrderId, Status = dto.status });
            //        break;


            //    case "WAITING_ERX_HUB_CLAIM_APPROVAL": // authorization request not sent within time
            //        await _hub.Clients.Group(dto.branchLicense).SendAsync("OrderCancelledSucess", new { OrderId = order.OrderId, Status = dto.status });
            //        break;


            //    case "ERX_HUB_CLAIM_FAILED": // authorization request not sent within time
            //        await _hub.Clients.Group(dto.branchLicense).SendAsync("OrderCancelledSucess", new { OrderId = order.OrderId, Status = dto.status });
            //        break;


            //    case "WAITING_PATIENT_CONFIRMATION": // authorization request not sent within time
            //        await _hub.Clients.Group(dto.branchLicense).SendAsync("OrderCancelledSucess", new { OrderId = order.OrderId, Status = dto.status });
            //        break;



            //    case "READY_FOR_CUSTOMER_PICKUP": // authorization request not sent within time
            //        await _hub.Clients.Group(dto.branchLicense).SendAsync("OrderCancelledSucess", new { OrderId = order.OrderId, Status = dto.status });
            //        break;

            //    case "DISPENSED": // authorization request not sent within time
            //        await _hub.Clients.Group(dto.branchLicense).SendAsync("OrderCancelledSucess", new { OrderId = order.OrderId, Status = dto.status });
            //        break;

            //    case "OUT_FOR_DELIVERY": // authorization request not sent within time
            //        await _hub.Clients.Group(dto.branchLicense).SendAsync("OrderCancelledSucess", new { OrderId = order.OrderId, Status = dto.status });
            //        break;


            //    case "RETURNED": // authorization request not sent within time
            //        await _hub.Clients.Group(dto.branchLicense).SendAsync("OrderCancelledSucess", new { OrderId = order.OrderId, Status = dto.status });
            //        break;
            //    case "DELIVERED":
            //        await _hub.Clients.Group(dto.branchLicense).SendAsync("OrderCancelledSucess", new { OrderId = order.OrderId, Status = dto.status });
            //        break;
            //    case "FAILED":
            //        await _hub.Clients.Group(dto.branchLicense).SendAsync("OrderCancelledSucess", new { OrderId = order.OrderId, Status = dto.status });
            //        break;

            //    default:
            //        break;
            //}

            //if (dto.status == "ACCEPTED_BY_PROVIDER")
            //{
            //    await _hub.Clients.Group(dto.branchLicense).SendAsync("OrderAccepted", new
            //    {
            //        OrderId = order.OrderId,
            //        Status = dto.status
            //    });
            //}
        }
        public async Task HandleSendYusurOrderAccept(OrderAcceptRequestDto dto, string branchno)
        {
            var errorResponse = await _yusur.AcceptOrderAsync(dto);

            var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == dto.orderId);

            if (errorResponse == null)
            {
                if (order != null)
                {
                    order.Status = "Submitted_to_Yusur";
                    await _db.SaveChangesAsync();
                }
                _logger.LogInformation($"Order {dto.orderId} send successfully.");
                await _hub.Clients.Group(branchno).SendAsync("OrderSubmitted", new
                {
                    OrderId = dto.orderId,
                    Status = "Order Submitted"

                });
            }
            else
            {
                if (order != null)
                {
                    order.Status = "Failed_Submitted_to_Yusur";
                    await _db.SaveChangesAsync();
                }
                //send notification order submission failed
                await _hub.Clients.Group(branchno).SendAsync("OrderSubmittedFailed", new
                {
                    OrderId = dto.orderId,
                    Errors = errorResponse.errors
                });
               _logger.LogWarning($"Failed to accept order {dto.orderId}.");
            }
        }
        public async Task AutoRejectOrder(string orderId, string reason)
        {
            var rejectRequest = new OrderRejectRequestDto(orderId, reason);
            await _yusur.RejectOrderAsync(rejectRequest);
        }

    }
}

