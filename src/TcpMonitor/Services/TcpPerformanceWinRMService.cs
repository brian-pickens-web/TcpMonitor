using System;
using Microsoft.Management.Infrastructure;
using Microsoft.VisualBasic;
using TcpMonitor.Models;
using WbemScripting;

namespace TcpMonitor.Services
{
    public class TcpPerformanceWinRMService : ITcpPerformanceService
    {
        private const string Server = "localhost";
        private const string Namespace = @"root\cimv2";

        public TcpPerformanceModel GetTcpPerformance()
        {
            var session = CimSession.Create(Server);
            var instance = session.GetInstance(Namespace, new CimInstance(TcpPerformance.PerfTcpV4RawDataCimClassName));
            return new TcpPerformanceModel()
            {
                ConnectionFailures = Convert.ToInt32(instance.CimInstanceProperties[TcpPerformance.PerfConnectionFailuresKey].Value),
                ConnectionsActive = Convert.ToInt32(instance.CimInstanceProperties[TcpPerformance.PerfConnectionsActiveKey].Value.ToString()),
                ConnectionsEstablished = Convert.ToInt32(instance.CimInstanceProperties[TcpPerformance.PerfConnectionsEstablishedKey].Value),
                ConnectionsPassive = Convert.ToInt32(instance.CimInstanceProperties[TcpPerformance.PerfConnectionsPassiveKey].Value),
                ConnectionsReset = Convert.ToInt32(instance.CimInstanceProperties[TcpPerformance.PerfConnectionsResetKey].Value)
            };
        }

        public static bool IsWindowsRemoteManagementEnabled()
        {
            try
            {
                new TcpPerformanceWinRMService().GetTcpPerformance();
                return true;
            }
            catch (CimException)
            {
                return false;
            }
        }
    }
}
