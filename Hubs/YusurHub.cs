using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using YusurIntegration.Data;
using YusurIntegration.Services;
namespace YusurIntegration.Hubs
{
    public class YusurHub : Hub
    {
        private readonly ConnectionManager _cm;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<YusurHub> _logger;

        //public YusurHub(ConnectionManager connections, IServiceScopeFactory scopeFactory, ILogger<YusurHub> logger)
        //{
        //    _cm = connections;
        //    _scopeFactory = scopeFactory;
        //    _logger = logger;
        //}
            public YusurHub(ConnectionManager connections, IServiceScopeFactory scopeFactory, ILogger<YusurHub> logger)
            {
            _cm = connections;
            _scopeFactory = scopeFactory;
            _logger = logger;
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

    }

 }

