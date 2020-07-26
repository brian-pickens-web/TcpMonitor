using System;
using System.Collections.Generic;
using System.Linq;
using TcpMonitor.Models;

namespace TcpMonitor.Services
{
    public class TcpConnectionService : ITcpConnectionService
    {
        private IProcessService _processService;

        public TcpConnectionService(IProcessService processService)
        {
            _processService = processService;
        }

        public IEnumerable<TcpConnectionModel> GetTcpConnectionData()
        {
            return WinTcpTable.GetAllTCPConnections().Select(row => new TcpConnectionModel()
            {
                ProcessId = row.ProcessId,
                ProcessName = _processService.GetProcessFromPid(Convert.ToInt32(row.ProcessId)).ProcessName,
                LocalAddress = row.LocalAddress,
                LocalPort = row.LocalPort,
                RemoteAddress = row.RemoteAddress,
                RemotePort = row.RemotePort,
                State = Map(row.State)
            });
        }

        private static string Map(TcpState state)
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
