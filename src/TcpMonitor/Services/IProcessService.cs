using System.Diagnostics;

namespace TcpMonitor.Services
{
    public interface IProcessService
    {
        Process GetProcessFromPid(int pid);
    }
}
