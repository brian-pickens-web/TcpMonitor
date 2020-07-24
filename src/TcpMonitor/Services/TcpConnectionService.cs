using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Management.Infrastructure;
using TcpMonitor.Models;

namespace TcpMonitor.Services
{
    public class TcpConnectionService
    {
        private const string Server = "localhost";
        private const string Path = @"root\cimv2";

        private const string PerfTcpV4RawDataCimClassName = "Win32_PerfRawData_Tcpip_TCPv4";
        private const string PerfConnectionFailuresKey = "ConnectionFailures";
        private const string PerfConnectionsActiveKey = "ConnectionsActive";
        private const string PerfConnectionsEstablishedKey = "ConnectionsEstablished";
        private const string PerfConnectionsPassiveKey = "ConnectionsPassive";
        private const string PerfConnectionsResetKey = "ConnectionsReset";

        public Dictionary<string, string> GetTcpPerformance()
        {
            var session = CimSession.Create(Server);
            var instance = session.GetInstance(Path, new CimInstance(PerfTcpV4RawDataCimClassName));
            var result = new Dictionary<string, string>();
            result.Add(PerfConnectionFailuresKey, instance.CimInstanceProperties[PerfConnectionFailuresKey].Value.ToString());
            result.Add(PerfConnectionsActiveKey, instance.CimInstanceProperties[PerfConnectionsActiveKey].Value.ToString());
            result.Add(PerfConnectionsEstablishedKey, instance.CimInstanceProperties[PerfConnectionsEstablishedKey].Value.ToString());
            result.Add(PerfConnectionsPassiveKey, instance.CimInstanceProperties[PerfConnectionsPassiveKey].Value.ToString());
            result.Add(PerfConnectionsResetKey, instance.CimInstanceProperties[PerfConnectionsResetKey].Value.ToString());
            return result;
        }

        public IEnumerable<TcpConnectionModel> GetTcpConnectionData()
        {
            return WinTcpTable.GetAllTCPConnections().Select(row => new TcpConnectionModel()
            {
                ProcessId = row.ProcessId,
                LocalAddress = row.LocalAddress,
                LocalPort = row.LocalPort,
                RemoteAddress = row.RemoteAddress,
                RemotePort = row.RemotePort,
                State = Map(row.State)
            });
        }

        private string Map(TcpState state)
        {
            switch (state)
            {
                case TcpState.MIB_TCP_STATE_CLOSED:
                    return "Closed";
                case TcpState.MIB_TCP_STATE_LISTEN:
                    return "Listening";
                case TcpState.MIB_TCP_STATE_SYN_SENT:
                    return "Request Sent";
                case TcpState.MIB_TCP_STATE_SYN_RCVD:
                    return "Request Received";
                case TcpState.MIB_TCP_STATE_ESTAB:
                    return "Connection Established";
                case TcpState.MIB_TCP_STATE_FIN_WAIT1:
                    return "Fin Wait 1";
                case TcpState.MIB_TCP_STATE_FIN_WAIT2:
                    return "Fin Wait 2";
                case TcpState.MIB_TCP_STATE_CLOSE_WAIT:
                    return "Close Waiting";
                case TcpState.MIB_TCP_STATE_CLOSING:
                    return "Closing";
                case TcpState.MIB_TCP_STATE_LAST_ACK:
                    return "Last Acknowledgement";
                case TcpState.MIB_TCP_STATE_TIME_WAIT:
                    return "Time Wait";
                case TcpState.MIB_TCP_STATE_DELETE_TCB:
                    return "Deleting";
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
