using System.Collections.Concurrent;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace YusurIntegration.Services
{
    //public class ConnectionManager
    //{
    //    private readonly ConcurrentDictionary<string, string> _map = new();

    //    public void AddOrUpdate(string branchLicense, string connectionId)
    //    {
    //        _map.AddOrUpdate(branchLicense, connectionId, (k, v) => connectionId);
    //    }

    //    public bool TryGetConnection(string branchLicense, out string connectionId)
    //    {
    //        return _map.TryGetValue(branchLicense, out connectionId);
    //    }

    //    public void RemoveByConnectionId(string connectionId)
    //    {
    //        var toRemove = _map.Where(kvp => kvp.Value == connectionId).Select(kvp => kvp.Key).ToList();
    //        foreach (var key in toRemove) _map.TryRemove(key, out _);
    //    }

    //}
    public class ConnectionManager
    {
        // Key = branchLicense         // Value = HashSet of connection IDs
        
        /*
        private readonly ConcurrentDictionary<string, HashSet<string>> _map
            = new ConcurrentDictionary<string, HashSet<string>>();

        public void Add(string branchLicense, string connectionId)
        {
            var list = _map.GetOrAdd(branchLicense, _ => new HashSet<string>());
            lock (list)
            {
                list.Add(connectionId);
            }
        }

        public void Remove(string branchLicense, string connectionId)
        {
            if (_map.TryGetValue(branchLicense, out var list))
            {
                lock (list)
                {
                    list.Remove(connectionId);
                    if (list.Count == 0)
                        _map.TryRemove(branchLicense, out _);
                }
            }
        }

        // Called when we know only the connection ID (disconnect)
        public void RemoveByConnectionId(string connectionId)
        {
            foreach (var kvp in _map)
            {
                lock (kvp.Value)
                {
                    if (kvp.Value.Contains(connectionId))
                    {
                        kvp.Value.Remove(connectionId);

                        if (kvp.Value.Count == 0)
                            _map.TryRemove(kvp.Key, out _);

                        return;
                    }
                }
            }
        }

        // This is what your controller needs
        public string[] GetConnections(string branchLicense)
        {
            if (_map.TryGetValue(branchLicense, out var list))
            {
                lock (list)
                {
                    return list.ToArray();
                }
            }
            return Array.Empty<string>();
        }
        public bool IsConnected(string branchLicense)
        {
            return _map.ContainsKey(branchLicense);
        }

        */



        private readonly ConcurrentDictionary<string, BranchTracker> _map
            = new ConcurrentDictionary<string, BranchTracker>();

        public void Add(string branchLicense, string connectionId)
        {
            var tracker = _map.GetOrAdd(branchLicense, _ => new BranchTracker());
            lock (tracker)
            {
                tracker.Connections.Add(connectionId);
                tracker.IsConnected = true;
                tracker.LastSeen = DateTime.UtcNow;
            }
        }
        public void RemoveByConnectionId(string connectionId)
        {
            foreach (var kvp in _map)
            {
                var tracker = kvp.Value;
                lock (tracker)
                {
                    if (tracker.Connections.Contains(connectionId))
                    {
                        tracker.Connections.Remove(connectionId);
                        if (tracker.Connections.Count == 0)
                        {
                            tracker.IsConnected = false;
                            tracker.LastSeen = DateTime.UtcNow;
                        }
                        return;
                    }
                }
            }
        }
        public void remove(string branchLicense, string connectionId)
        {
            if (_map.TryGetValue(branchLicense, out var tracker))
            {
                lock (tracker)
                {
                    tracker.Connections.Remove(connectionId);
                    if (tracker.Connections.Count == 0)
                    {
                        tracker.IsConnected = false;
                        tracker.LastSeen = DateTime.UtcNow;
                    }
                }
            }
        }
        public bool IsConnected(string branchLicense)
        {
            if (_map.TryGetValue(branchLicense, out var tracker))
            {
                lock (tracker)
                {
                    return tracker.IsConnected;
                }
            }
            return false;
        }
        public (bool IsConnected, DateTime LastSeen) GetBranchStatus(string branchLicense)
        {
            if (_map.TryGetValue(branchLicense, out var tracker))
            {
                lock (tracker)
                {
                    return (tracker.IsConnected, tracker.LastSeen);
                }
            }
            return (false, DateTime.MinValue);
        }
        public string[] GetConnections(string branchLicense)
        {
            if (_map.TryGetValue(branchLicense, out var tracker))
            {
                lock (tracker)
                {
                    return tracker.Connections.ToArray();
                }
            }
            return Array.Empty<string>();
        }

    }
    class BranchTracker
    {
        public HashSet<string> Connections { get; } = new();
        public bool IsConnected { get; set; }
        public DateTime LastSeen { get; set; }
    }


}
