using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using TcpMonitor.Domain;
using TcpMonitor.Models;
using WbemScripting;

namespace TcpMonitor.Services
{
    public class TcpComService : ITcpPerformanceService, ITcpConnectionService, ITcpSettingsService
    {
        private const string CimV2Path = @"winmgmts://./" + Wmi.Cimv2Namespace;
        private const string StandardV2Path = @"winmgmts://./" + Wmi.StandardCimv2Namespace;

        private static readonly SWbemServicesEx CimV2Services = (SWbemServicesEx)Interaction.GetObject(CimV2Path);
        private static readonly SWbemServicesEx StandardV2Services = (SWbemServicesEx)Interaction.GetObject(StandardV2Path);
        private static readonly SWbemRefresher ObjRefresher = (SWbemRefresher)Interaction.CreateObject("WbemScripting.Swbemrefresher");
        private static readonly SWbemRefreshableItem TcpPerformanceRefreshableItem = ObjRefresher.AddEnum(CimV2Services, TcpPerformance.TcpPerformanceClassName);
        private static readonly SWbemRefreshableItem TcpConnectionsRefreshableItem = ObjRefresher.AddEnum(StandardV2Services, TcpConnection.TcpConnectionClassName);

        public readonly ILogger _logger;
        public readonly IProcessService _processService;

        public TcpComService(ILogger logger, IProcessService processService)
        {
            _logger = logger;
            _processService = processService;

            _logger.LogWarning("");
        }

        public TcpSettingsModel GetTcpSettings()
        {
            var objectSet = StandardV2Services.ExecQuery(TcpSettings.SettingsQuery);
            foreach (ISWbemObjectEx objInstance in objectSet)
            {
                var portStart = Convert.ToInt32(objInstance.Properties_.Item(TcpSettings.DynamicPortRangeStartPortKey).get_Value());
                var portRange = Convert.ToInt32(objInstance.Properties_.Item(TcpSettings.DynamicPortRangeNumberOfPortsKey).get_Value());

                return new TcpSettingsModel()
                {
                    DynamicPortRangeStart = portStart,
                    DynamicPortRangeEnd = portStart + portRange,
                    TotalDynamicPorts = portRange
                };
            }

            return new TcpSettingsModel();
        }

        public TcpPerformanceModel GetTcpPerformance()
        {
            ObjRefresher.Refresh();
            foreach (ISWbemObjectEx objInstance in TcpPerformanceRefreshableItem.ObjectSet)
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

            return new TcpPerformanceModel();
        }

        public IEnumerable<TcpConnectionModel> GetTcpConnections()
        {
            ObjRefresher.Refresh();
            foreach (ISWbemObjectEx objInstance in TcpConnectionsRefreshableItem.ObjectSet)
            {
                // CIM query can return TCP Instances with State = 100 - filter those out
                var tcpState = Convert.ToInt32(objInstance.Properties_.Item(TcpConnection.StateKey).get_Value());
                if (tcpState > 12) continue;

                var processId = objInstance.Properties_.Item(TcpConnection.ProcessIdKey).get_Value();
                yield return new TcpConnectionModel()
                {
                    ProcessId = Convert.ToUInt16(processId),
                    ProcessName = _processService.GetProcessFromPid(Convert.ToInt32(processId)).ProcessName,
                    LocalAddress = IPAddress.Parse(objInstance.Properties_.Item(TcpConnection.LocalAddressKey).get_Value().ToString()),
                    LocalPort = Convert.ToUInt16(objInstance.Properties_.Item(TcpConnection.LocalPortKey).get_Value()),
                    RemoteAddress = IPAddress.Parse(objInstance.Properties_.Item(TcpConnection.RemoteAddressKey).get_Value().ToString()),
                    RemotePort = Convert.ToUInt16(objInstance.Properties_.Item(TcpConnection.RemotePortKey).get_Value()),
                    State = ((TcpConnection.State)tcpState).AsString()
                };
            }
        }
    }
}
