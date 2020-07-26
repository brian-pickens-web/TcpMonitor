using TcpMonitor.Models;

namespace TcpMonitor.Services
{
    public interface ITcpPerformanceService
    {   
        TcpPerformanceModel GetTcpPerformance();
    }
}
