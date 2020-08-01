using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TcpMonitor.Models;

namespace TcpMonitor.Services
{
    public class TcpConnectionCacheWrapper : ITcpConnectionService
    {
        private const short MaxTick = 3;
        private readonly ConcurrentDictionary<int, (short tick, TcpConnectionModel model)> _cache;
        private readonly ITcpConnectionService _tcpConnectionService;
        private readonly ILogger _logger;

        public TcpConnectionCacheWrapper(Func<ITcpConnectionService> tcpConnectionServiceFactory, ILogger logger)
        {
            var concurrency = Environment.ProcessorCount * 2;
            _cache = new ConcurrentDictionary<int, (Int16 tick, TcpConnectionModel model)>(concurrency, 500);
            _tcpConnectionService = tcpConnectionServiceFactory();
            _logger = logger;
        }

        public async IAsyncEnumerable<TcpConnectionModel> GetTcpConnections()
        {
            var newConnections = new List<(short tick, TcpConnectionModel model)>();

            // Yield all new connections
            await foreach (var model in _tcpConnectionService.GetTcpConnections())
            {
                var itemId = GetItemIndex(model);
                var newItem = (MaxTick, model);
                newConnections.Add(newItem);
                _cache.AddOrUpdate(itemId, newItem, (i, tuple) => newItem);
                yield return model;
            }

            if (newConnections.Count < 50)
            {
                _logger.LogTrace($"New Connections below 50. Count: {newConnections.Count}");
            }

            // Yield cached items if its tick counter hasn't expired
            var missingModels = _cache.Values.Except(newConnections);
            foreach (var (tick, model) in missingModels)
            {
                var itemId = GetItemIndex(model);
                if (tick == 0)
                {
                    _cache.TryRemove(itemId, out _);
                }
                else
                {
                    var tickDown = ((short)(tick - 1), model);
                    _cache.TryUpdate(itemId, tickDown, (tick, model));
                    yield return model;
                }
            }
        }

        private static int GetItemIndex<T>(T item)
        {
            var row = CreateRow(item);
            return string.Join("-", row).GetHashCode();
        }

        private static string[] CreateRow<T>(T value)
        {
            var type = typeof(T);
            var columnNames = type.GetProperties().Select(info => info.Name).ToArray();
            var row = new string[columnNames.Length];
            for (int i = 0; i < columnNames.Length; i++)
            {
                row[i] = type.GetProperty(columnNames[i])?.GetValue(value).ToString();
            }
            return row;
        }
    }
}
