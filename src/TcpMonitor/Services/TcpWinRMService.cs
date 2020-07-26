using System;
using System.Linq;
using Microsoft.Management.Infrastructure;
using TcpMonitor.Domain;
using TcpMonitor.Models;

namespace TcpMonitor.Services
{
    public class TcpWinRMService : ITcpPerformanceService, ITcpSettingsService
    {
        private const string Server = "localhost";

        public static bool IsWindowsRemoteManagementEnabled()
        {
            try
            {
                new TcpWinRMService().GetTcpPerformance();
                return true;
            }
            catch (CimException)
            {
                return false;
            }
        }

        public TcpSettingsModel GetTcpSettings()
        {
            var session = CimSession.Create(Server);
            var instances = session.QueryInstances(TcpSettings.SettingsClassNamespace, Wmi.QueryDialect, TcpSettings.SettingsQuery);
            var instance = instances.FirstOrDefault();

            if (instance == null)
                return null;

            var startPort = Convert.ToInt32(instance.CimInstanceProperties[TcpSettings.DynamicPortRangeStartPortKey].Value);
            var totalPorts = Convert.ToInt32(instance.CimInstanceProperties[TcpSettings.DynamicPortRangeNumberOfPortsKey].Value);
            return new TcpSettingsModel()
            {
                DynamicPortRangeStart = startPort,
                DynamicPortRangeEnd = startPort + totalPorts,
                TotalDynamicPorts = totalPorts
            };
        }

        public TcpPerformanceModel GetTcpPerformance()
        {
            var session = CimSession.Create(Server);
            var instance = session.GetInstance(TcpPerformance.TcpPerformanceNamespace, new CimInstance(TcpPerformance.TcpPerformanceClassName));
            return new TcpPerformanceModel()
            {
                ConnectionFailures = Convert.ToInt32(instance.CimInstanceProperties[TcpPerformance.ConnectionFailuresKey].Value),
                ConnectionsActive = Convert.ToInt32(instance.CimInstanceProperties[TcpPerformance.ConnectionsActiveKey].Value.ToString()),
                ConnectionsEstablished = Convert.ToInt32(instance.CimInstanceProperties[TcpPerformance.ConnectionsEstablishedKey].Value),
                ConnectionsPassive = Convert.ToInt32(instance.CimInstanceProperties[TcpPerformance.ConnectionsPassiveKey].Value),
                ConnectionsReset = Convert.ToInt32(instance.CimInstanceProperties[TcpPerformance.ConnectionsResetKey].Value)
            };
        }
    }
}
