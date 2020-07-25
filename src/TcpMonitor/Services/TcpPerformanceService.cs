using System;
using Microsoft.Management.Infrastructure;
using Microsoft.VisualBasic;
using TcpMonitor.Models;
using WbemScripting;

namespace TcpMonitor.Services
{
    public static class TcpPerformanceService
    {
        private const string Server = "localhost";
        private const string Namespace = @"root\cimv2";
        private const string Path = @"winmgmts://./root\cimv2";

        private const string PerfTcpV4RawDataCimClassName = "Win32_PerfRawData_Tcpip_TCPv4";
        private const string PerfConnectionFailuresKey = "ConnectionFailures";
        private const string PerfConnectionsActiveKey = "ConnectionsActive";
        private const string PerfConnectionsEstablishedKey = "ConnectionsEstablished";
        private const string PerfConnectionsPassiveKey = "ConnectionsPassive";
        private const string PerfConnectionsResetKey = "ConnectionsReset";

        private static readonly SWbemServicesEx ServicesEx = (SWbemServicesEx)Interaction.GetObject(Path);
        private static readonly SWbemRefresher ObjRefresher = (SWbemRefresher)Interaction.CreateObject("WbemScripting.Swbemrefresher");

        static TcpPerformanceService()
        {
            ObjRefresher.AddEnum(ServicesEx, PerfTcpV4RawDataCimClassName);
        }

        public static TcpPerformanceModel GetTcpPerformance()
        {
            return GetTcpPerformanceCom();
        }

        private static TcpPerformanceModel GetTcpPerformanceCom()
        {
            ObjRefresher.Refresh();
            foreach (SWbemRefreshableItem refreshItem in ObjRefresher)
            {
                foreach (ISWbemObjectEx objInstance in refreshItem.ObjectSet)
                {
                    return new TcpPerformanceModel()
                    {
                        ConnectionFailures = Convert.ToInt32(objInstance.Properties_.Item(PerfConnectionFailuresKey).get_Value()),
                        ConnectionsActive = Convert.ToInt32(objInstance.Properties_.Item(PerfConnectionsActiveKey).get_Value()),
                        ConnectionsEstablished = Convert.ToInt32(objInstance.Properties_.Item(PerfConnectionsEstablishedKey).get_Value()),
                        ConnectionsPassive = Convert.ToInt32(objInstance.Properties_.Item(PerfConnectionsPassiveKey).get_Value()),
                        ConnectionsReset = Convert.ToInt32(objInstance.Properties_.Item(PerfConnectionsResetKey).get_Value())
                    };
                }
            }
             
            return null;
        }

        private static TcpPerformanceModel GetTcpPerformanceWinRM()
        {
            var session = CimSession.Create(Server);
            var instance = session.GetInstance(Namespace, new CimInstance(PerfTcpV4RawDataCimClassName));
            return new TcpPerformanceModel()
            {
                ConnectionFailures = Convert.ToInt32(instance.CimInstanceProperties[PerfConnectionFailuresKey].Value),
                ConnectionsActive = Convert.ToInt32(instance.CimInstanceProperties[PerfConnectionsActiveKey].Value.ToString()),
                ConnectionsEstablished = Convert.ToInt32(instance.CimInstanceProperties[PerfConnectionsEstablishedKey].Value),
                ConnectionsPassive = Convert.ToInt32(instance.CimInstanceProperties[PerfConnectionsPassiveKey].Value),
                ConnectionsReset = Convert.ToInt32(instance.CimInstanceProperties[PerfConnectionsResetKey].Value)
            };
        }
    }
}
