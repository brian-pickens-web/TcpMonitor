using System.Net;

namespace TcpMonitor.Models
{
    public class TcpConnectionModel
    {
        public uint ProcessId { get; set; }
        public string ProcessName { get; set; }
        public IPAddress LocalAddress { get; set; }
        public ushort LocalPort { get; set; }
        public IPAddress RemoteAddress { get; set; }
        public ushort RemotePort { get; set; }
        public string State { get; set; }
    }
}
