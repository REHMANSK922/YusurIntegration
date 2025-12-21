using Microsoft.AspNetCore.SignalR;
using YusurIntegration.Services;
namespace YusurIntegration.Hubs
{
    public class YusurHub : Hub
    {
        private readonly ConnectionManager _cm;
        private readonly IServiceScopeFactory _scopeFactory;
        public YusurHub(ConnectionManager connections, IServiceScopeFactory scopeFactory)
        {
            _cm = connections;
            _scopeFactory = scopeFactory;
        }
        public override Task OnConnectedAsync()
        {
            var http = Context.GetHttpContext();
            var branch = http?.Request.Query["branch"].ToString();
            if (!string.IsNullOrEmpty(branch))
            {
                _cm.Add(branch, Context.ConnectionId);
            }
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _cm.RemoveByConnectionId(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }



        public async Task Acknowledge(string messageId)
        {
            //if (string.IsNullOrWhiteSpace(messageId)) return;

            //using var scope = _scopeFactory.CreateScope();
            //var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            //var msg = db.PendingMessages.SingleOrDefault(p => p.MessageId == messageId);
            //if (msg != null)
            //{
            //    db.PendingMessages.Remove(msg);
            //    await db.SaveChangesAsync();
            //}
        }

    }
}
