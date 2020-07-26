using System;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace TcpMonitor.Services
{
    public class ProcessService : IProcessService
    {
        private readonly IMemoryCache _cache;

        public ProcessService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Process GetProcessFromPid(int pid)
        {
            if (_cache.TryGetValue(pid, out Process process))
            {
                return process;
            }

            // Key not in cache, so get data.
            process = Process.GetProcessById(pid);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            // Save data in cache.
            _cache.Set(pid, process, cacheEntryOptions);

            return process;
        }
    }
}
