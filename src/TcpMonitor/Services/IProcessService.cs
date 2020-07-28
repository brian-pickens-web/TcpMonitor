using System.Diagnostics;

namespace TcpMonitor.Services
{
    public interface IProcessService
    {
        Process GetProcessFromPid(int pid);
        Process GetProcessFromPid(uint pid);
    }
}
