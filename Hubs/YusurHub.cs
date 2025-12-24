using Microsoft.AspNetCore.SignalR;
using YusurIntegration.Data;
using YusurIntegration.Services;
namespace YusurIntegration.Hubs
{
    public class YusurHub : Hub
    {
        //private readonly ConnectionManager _cm;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<YusurHub> _logger;

        //public YusurHub(ConnectionManager connections, IServiceScopeFactory scopeFactory, ILogger<YusurHub> logger)
        //{
        //    _cm = connections;
        //    _scopeFactory = scopeFactory;
        //    _logger = logger;
        //}
            public YusurHub( IServiceScopeFactory scopeFactory, ILogger<YusurHub> logger)
            {
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
                if (!string.IsNullOrEmpty(branch))
                    {
                        await Groups.AddToGroupAsync(connId, branch);
                    }
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var pending = db.PendingMessages
                    .Where(x => x.BranchLicense == branch)
                    .OrderBy(x => x.CreatedAt)
                    .ToList();

                foreach (var msg in pending)
                {
                    await Clients.Client(connId)
                        .SendAsync(msg.MessageType, msg);
                }
                await db.SaveChangesAsync();
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
                await base.OnDisconnectedAsync(exception);
            }
        }

 }

