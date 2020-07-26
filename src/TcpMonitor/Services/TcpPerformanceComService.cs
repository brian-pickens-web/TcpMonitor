using System;
using Microsoft.VisualBasic;
using TcpMonitor.Domain;
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
            ObjRefresher.AddEnum(ServicesEx, TcpPerformance.TcpPerformanceClassName);
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
                        ConnectionFailures = Convert.ToInt32(objInstance.Properties_.Item(TcpPerformance.ConnectionFailuresKey).get_Value()),
                        ConnectionsActive = Convert.ToInt32(objInstance.Properties_.Item(TcpPerformance.ConnectionsActiveKey).get_Value()),
                        ConnectionsEstablished = Convert.ToInt32(objInstance.Properties_.Item(TcpPerformance.ConnectionsEstablishedKey).get_Value()),
                        ConnectionsPassive = Convert.ToInt32(objInstance.Properties_.Item(TcpPerformance.ConnectionsPassiveKey).get_Value()),
                        ConnectionsReset = Convert.ToInt32(objInstance.Properties_.Item(TcpPerformance.ConnectionsResetKey).get_Value())
                    };
                }
            }
             
            return null;
        }
    }
}
