using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using YusurIntegration.Data;
using YusurIntegration.DTOs;
using YusurIntegration.Helpers;
using YusurIntegration.Models;
using YusurIntegration.Services;
using YusurIntegration.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static YusurIntegration.DTOs.YusurPayloads;
namespace YusurIntegration.Hubs
{
    public class YusurHub : Hub
    {
        private readonly ConnectionManager _cm;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<YusurHub> _logger;
        private readonly IOrderService _orderService;
        private readonly OrderResponseTracker _orderResponseTracker;

        public YusurHub(ConnectionManager connections,
            IServiceScopeFactory scopeFactory, 
            ILogger<YusurHub> logger,
            IOrderService orderService , 
            OrderResponseTracker orderResponseTracker)
            {
            _cm = connections;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _orderService = orderService;
            _orderResponseTracker = orderResponseTracker;
        }
        //public override Task OnConnectedAsync()
        //{
        //    var http = Context.GetHttpContext();
        //    var branch = http?.Request.Query["branch"].ToString();
        //    if (!string.IsNullOrEmpty(branch))
        //    {
        //        _cm.Add(branch, Context.ConnectionId);
        //    }
        //    return base.OnConnectedAsync();
        //}
        //public override Task OnDisconnectedAsync(Exception? exception)
        //{
        //    _cm.RemoveByConnectionId(Context.ConnectionId);
        //    return base.OnDisconnectedAsync(exception);
        //}

        //public async Task SubscribeToBranch(string branchLicense)
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, branchLicense);
        //    _logger.LogInformation($"Client {Context.ConnectionId} subscribed to branch {branchLicense}");
        //}
            public async Task Acknowledge(string messageId)
            {
                if (string.IsNullOrWhiteSpace(messageId)) return;

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var msg = db.PendingMessages.SingleOrDefault(p => p.MessageId == messageId);
                if (msg != null)
                {
                    db.PendingMessages.Remove(msg);
                    await db.SaveChangesAsync();
                _logger.LogInformation($"Message {messageId} acknowledged and removed.");
            }
            }
            public override async Task OnConnectedAsync()
            {
                var http = Context.GetHttpContext();
                var connId = Context.ConnectionId;
                var branch = http?.Request.Query["branch"].ToString();

            var isAdmin = http?.Request.Query["isAdmin"].ToString() == "true";

            if (isAdmin)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
                if (!string.IsNullOrEmpty(branch))
                    await Clients.Group("Admins").SendAsync("AdminLog", $"Branch {branch} connected.");
            }

            if (!string.IsNullOrEmpty(branch))
                {
                 await Groups.AddToGroupAsync(connId, branch);
                _cm.Add(branch, Context.ConnectionId);
                _logger.LogInformation($"Client {connId} connected to branch {branch}");
                await ProcessPendingMessages(connId, branch);
               }
               await base.OnConnectedAsync();
            }
            public async Task RegisterClientInfo(string license, string posName)
            {
                if (!string.IsNullOrEmpty(license))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, license);
                }
            }
            // This is the FIX for the "couple of minutes" disconnection
            public override async Task OnDisconnectedAsync(Exception? exception)
            {
            // No manual cleanup needed for Groups! 
            // SignalR removes the connection automatically.
                _cm.RemoveByConnectionId(Context.ConnectionId);
                await base.OnDisconnectedAsync(exception);
            }
            private async Task ProcessPendingMessages(string connId, string branch)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                var pending = await db.PendingMessages
                    .Where(x => x.BranchLicense == branch)
                    .OrderBy(x => x.CreatedAt)
                    .ToListAsync();

                if (pending.Any())
                {
                    foreach (var msg in pending)
                    {
                        await Clients.Client(connId).SendAsync(msg.MessageType, msg);
                    }
                _logger.LogInformation($"Delivered {pending.Count} pending messages to connection {connId} for branch {branch}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing pending messages for branch {Branch}", branch);
            }
        }
            public async Task ConfirmOrderStockAvailable(OrderAcceptRequestDto acceptdto)
            {

                var branch = Context.Items["BranchName"] as string;
               _orderResponseTracker.SetResult(acceptdto.orderId, "Acceptance", acceptdto);
            //// 1. Logic to call Yusur API (Accept/Reject) via your Service
            //    await _orderService.HandleSendYusurOrderAccept(acceptdto, branch);
            //    // 2. Log for Admins
            //    await Clients.Group("Admins").SendAsync("AdminLog", $"Order {acceptdto.orderId} was  Accepted by branch.{branch}"); // add branch info from cleint side.. 

            }
            public async Task OrderReject(OrderAcceptRequestDto acceptdto, string branch, string rejectedreason)
            {
            // 1. Logic to call Yusur API (Accept/Reject) via your Service
                await _orderService.AutoRejectOrder(acceptdto.orderId, rejectedreason);
            // 2. Log for Admins
                await Clients.Group("Admins").SendAsync("AdminLog", $"Order {acceptdto.orderId} was  Accepted by branch.{branch}"); // add branch info from cleint side.. 
            }
            public async Task OrderCancel(OrderAcceptRequestDto acceptdto, string branch,string rejectedreason)
            {
                // 1. Logic to call Yusur API (Accept/Reject) via your Service
                    await _orderService.AutoRejectOrder(acceptdto.orderId, rejectedreason);
                // 2. Log for Admins
                    await Clients.Group("Admins").SendAsync("AdminLog", $"Order {acceptdto.orderId} was  Accepted by branch.{branch}"); // add branch info from cleint side.. 
            }





        ////    public async Task RequestPrescriptionDispense(string orderId)
        ////    {
        ////    // 1. Get the branch license from the current connection context
        ////    var branchLicense = Context.Items["BranchLicense"] as string;

        ////    // 2. Fetch the data from your database to fill the Dispense DTO
        ////    var order = await _db.Orders
        ////        .Include(o => o.Patient)
        ////        .Include(o => o.ShippingAddress)
        ////        .FirstOrDefaultAsync(o => o.OrderId == orderId);

        ////    if (order == null) return;

        ////    // 3. Map local order to the Dispense Request we just defined
        ////    var dispenseReq = new PrescriptionDispenseRequest
        ////    {
        ////        PatientNationalId = order.Patient.nationalId,
        ////        PrescriptionReferenceNumber = order.ErxReference,
        ////        IsPickup = order.IsPickup,
        ////        ShippingAddress = new ShippingAddressDto
        ////        {
        ////            streetAddress1 = order.ShippingAddress.addressLine1,
        ////            phone = order.Patient.phone, // Ensure this is captured
        ////            cityId = order.ShippingAddress.CityId, // From your DB
        ////            coordinates = new CoordinatesDto
        ////            {
        ////                latitude = (float)order.ShippingAddress.Coordinates.latitude,
        ////                longitude = (float)order.ShippingAddress.Coordinates.longitude
        ////            }
        ////        }
        ////    };

        ////    // 4. Call the Yusur API service
        ////    var response = await _yusurService.PrescriptionDispenseAsync(dispenseReq);

        ////    // 5. Tell the client if it worked or failed
        ////    if (response?.errors == null)
        ////    {
        ////        order.Status = "DISPENSED";
        ////        await _db.SaveChangesAsync();
        ////        await Clients.Caller.SendAsync("DispenseSuccess", orderId);
        ////    }
        ////    else
        ////    {
        ////        await Clients.Caller.SendAsync("DispenseFailed", orderId, response.errors);
        ////    }
        ////}




    }

 }

