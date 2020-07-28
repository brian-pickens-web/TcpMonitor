using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Management.Infrastructure;
using TcpMonitor.Domain;
using TcpMonitor.Models;

namespace TcpMonitor.Services
{
    public class TcpWinRmService : ITcpPerformanceService, ITcpSettingsService, ITcpConnectionService
    {
        private const string Server = "localhost";

        private readonly IProcessService _processService;

        public TcpWinRmService(IProcessService processService)
        {
            _processService = processService;
        }

        public static bool IsWindowsRemoteManagementEnabled()
        {
            try
            {
                var session = CimSession.Create(Server);
                session.GetInstance(TcpPerformance.TcpPerformanceNamespace, new CimInstance(TcpPerformance.TcpPerformanceClassName));
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

        public IEnumerable<TcpConnectionModel> GetTcpConnections()
        {
            var session = CimSession.Create(Server);
            var instances = session.QueryInstances(
                TcpConnection.TcpConnectionNamespace,
                Wmi.QueryDialect,
                $"Select * From {TcpConnection.TcpConnectionClassName}");

            foreach (var instance in instances)
            {
                // CIM query can return TCP Instances with State = 100 - filter those out
                var tcpState = Convert.ToInt32(instance.CimInstanceProperties[TcpConnection.StateKey].Value);
                if (tcpState > 12) continue;

                var processId = Convert.ToUInt32(instance.CimInstanceProperties[TcpConnection.ProcessIdKey].Value);
                yield return new TcpConnectionModel()
                {
                    ProcessId = processId,
                    ProcessName = _processService.GetProcessFromPid(processId).ProcessName,
                    LocalAddress = IPAddress.Parse(instance.CimInstanceProperties[TcpConnection.LocalAddressKey].Value.ToString()),
                    LocalPort = Convert.ToUInt16(instance.CimInstanceProperties[TcpConnection.LocalPortKey].Value.ToString()),
                    RemoteAddress = IPAddress.Parse(instance.CimInstanceProperties[TcpConnection.RemoteAddressKey].Value.ToString()),
                    RemotePort = Convert.ToUInt16(instance.CimInstanceProperties[TcpConnection.RemotePortKey].Value.ToString()),
                    State = TcpConnection.ConvertState(tcpState)
                };
            }
        }
    }
}
