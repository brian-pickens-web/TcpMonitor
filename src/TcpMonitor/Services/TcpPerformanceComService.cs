using System;
using Microsoft.VisualBasic;
using TcpMonitor.Models;
using WbemScripting;

namespace TcpMonitor.Services
{
    public class TcpPerformanceComService : ITcpPerformanceService
    {
        private const string Path = @"winmgmts://./root\cimv2";
        
        private static readonly SWbemServicesEx ServicesEx = (SWbemServicesEx)Interaction.GetObject(Path);
        private static readonly SWbemRefresher ObjRefresher = (SWbemRefresher)Interaction.CreateObject("WbemScripting.Swbemrefresher");

        static TcpPerformanceComService()
        {
            ObjRefresher.AddEnum(ServicesEx, TcpPerformance.PerfTcpV4RawDataCimClassName);
        }

        public TcpPerformanceModel GetTcpPerformance()
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
                        ConnectionFailures = Convert.ToInt32(objInstance.Properties_.Item(TcpPerformance.PerfConnectionFailuresKey).get_Value()),
                        ConnectionsActive = Convert.ToInt32(objInstance.Properties_.Item(TcpPerformance.PerfConnectionsActiveKey).get_Value()),
                        ConnectionsEstablished = Convert.ToInt32(objInstance.Properties_.Item(TcpPerformance.PerfConnectionsEstablishedKey).get_Value()),
                        ConnectionsPassive = Convert.ToInt32(objInstance.Properties_.Item(TcpPerformance.PerfConnectionsPassiveKey).get_Value()),
                        ConnectionsReset = Convert.ToInt32(objInstance.Properties_.Item(TcpPerformance.PerfConnectionsResetKey).get_Value())
                    };
                }
            }
             
            return null;
        }
    }
}
