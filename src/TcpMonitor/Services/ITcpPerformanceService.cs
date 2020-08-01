using System.Threading.Tasks;
using TcpMonitor.Models;

namespace TcpMonitor.Services
{
    public interface ITcpPerformanceService
    {   
        Task<TcpPerformanceModel> GetTcpPerformance();
    }
}
