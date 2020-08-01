using System.Threading.Tasks;
using TcpMonitor.Models;

namespace TcpMonitor.Services
{
    public interface ITcpSettingsService
    {
        Task<TcpSettingsModel> GetTcpSettings();
    }
}
