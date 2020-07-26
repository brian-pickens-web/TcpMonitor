namespace TcpMonitor.Domain
{
    static class TcpPerformance
    {
        public const string TcpPerformanceNamespace = Wmi.Cimv2Namespace;
        public const string TcpPerformanceClassName = "Win32_PerfRawData_Tcpip_TCPv4";
        public const string ConnectionFailuresKey = "ConnectionFailures";
        public const string ConnectionsActiveKey = "ConnectionsActive";
        public const string ConnectionsEstablishedKey = "ConnectionsEstablished";
        public const string ConnectionsPassiveKey = "ConnectionsPassive";
        public const string ConnectionsResetKey = "ConnectionsReset";
    }
}
