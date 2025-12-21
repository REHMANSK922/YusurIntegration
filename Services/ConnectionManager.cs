using System.Collections.Concurrent;
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
        // Key = branchLicense
        // Value = HashSet of connection IDs
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
        public bool IsConnected(string storeId)
        {
            return _map.ContainsKey(storeId);
        }



    }



}
