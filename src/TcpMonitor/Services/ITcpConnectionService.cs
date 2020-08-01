using System.Collections.Generic;
using TcpMonitor.Models;

namespace TcpMonitor.Services
{
    public interface ITcpConnectionService
    {
        IAsyncEnumerable<TcpConnectionModel> GetTcpConnections();
    }
}
