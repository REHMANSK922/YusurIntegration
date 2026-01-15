using System.Collections.Concurrent;
using static YusurIntegration.DTOs.YusurPayloads;

namespace YusurIntegration.Helpers
{
    public class OrderResponseTracker
    {
        // Now we store the DTO instead of just a true/false
        private readonly ConcurrentDictionary<string, TaskCompletionSource<OrderAcceptRequestDto?>> _pendingTasks = new();

        public async Task<OrderAcceptRequestDto?> WaitForClientDataAsync(string orderId, string stepName, TimeSpan timeout)
        {
            var key = $"{orderId}:{stepName}";
            var tcs = new TaskCompletionSource<OrderAcceptRequestDto?>();
            _pendingTasks[key] = tcs;

            var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeout));
            _pendingTasks.TryRemove(key, out _);

            return completedTask == tcs.Task ? await tcs.Task : null;
        }

        public void SetResult(string orderId, string stepName, OrderAcceptRequestDto? data)
        {
            var key = $"{orderId}:{stepName}";
            if (_pendingTasks.TryGetValue(key, out var tcs))
            {
                tcs.TrySetResult(data);
            }
        }
    }



    //public class OrderResponseTracker
    //{
    //    // Key is now "OrderId:StepName" (e.g., "ORD123:Acceptance")
    //    private readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> _pendingTasks = new();

    //    //public async Task<bool> WaitForClientAsync(string orderId, string stepName, TimeSpan timeout)
    //    //{
    //    //    var key = $"{orderId}:{stepName}";
    //    //    var tcs = new TaskCompletionSource<bool>();

    //    //    _pendingTasks[key] = tcs;

    //    //    var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeout));

    //    //    _pendingTasks.TryRemove(key, out _);

    //    //    // Return true only if the client responded AND sent a 'Success' boolean
    //    //    return completedTask == tcs.Task && await tcs.Task;
    //    //}



    //    public void SetResult(string orderId, string stepName, bool success)
    //    {
    //        var key = $"{orderId}:{stepName}";
    //        if (_pendingTasks.TryGetValue(key, out var tcs))
    //        {
    //            tcs.TrySetResult(success);
    //        }
    //    }
    //}
}
