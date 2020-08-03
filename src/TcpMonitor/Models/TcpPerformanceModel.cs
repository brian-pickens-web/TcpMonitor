﻿namespace TcpMonitor.Models
{
    public class TcpPerformanceModel
    {
        public int ConnectionsActive { get; set; }
        public int ConnectionsEstablished { get; set; }
        public int ConnectionsPassive { get; set; }
        public int ConnectionsReset { get; set; }
        public int ConnectionFailures { get; set; }
    }
}
