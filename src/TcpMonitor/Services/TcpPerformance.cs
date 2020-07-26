namespace TcpMonitor.Services
{
    public static class TcpPerformance
    {
        public const string PerfTcpV4RawDataCimClassName = "Win32_PerfRawData_Tcpip_TCPv4";
        public const string PerfConnectionFailuresKey = "ConnectionFailures";
        public const string PerfConnectionsActiveKey = "ConnectionsActive";
        public const string PerfConnectionsEstablishedKey = "ConnectionsEstablished";
        public const string PerfConnectionsPassiveKey = "ConnectionsPassive";
        public const string PerfConnectionsResetKey = "ConnectionsReset";
    }
}
