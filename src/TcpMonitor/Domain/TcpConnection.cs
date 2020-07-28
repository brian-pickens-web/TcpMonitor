using System;

namespace TcpMonitor.Domain
{
    static class TcpConnection
    {
        public const string TcpConnectionNamespace = Wmi.StandardCimv2Namespace;
        public const string TcpConnectionClassName = "MSFT_NetTCPConnection";
        public const string ProcessIdKey = "OwningProcess";
        public const string LocalAddressKey = "LocalAddress";
        public const string LocalPortKey = "LocalPort";
        public const string RemoteAddressKey = "RemoteAddress";
        public const string RemotePortKey = "RemotePort";
        public const string StateKey = "State";

        public static string ConvertState(int state)
        {
            var asEnum = (State)state;
            return asEnum.AsString();
        }

        public static string AsString(this State state)
        {
            return state switch
            {
                State.CLOSED => "Closed",
                State.LISTEN => "Listening",
                State.SYN_SENT => "Request Sent",
                State.SYN_RCVD => "Request Received",
                State.ESTAB => "Established",
                State.FIN_WAIT1 => "Fin Wait 1",
                State.FIN_WAIT2 => "Fin Wait 2",
                State.CLOSE_WAIT => "Close Waiting",
                State.CLOSING => "Closing",
                State.LAST_ACK => "Last Acknowledgement",
                State.TIME_WAIT => "Time Wait",
                State.DELETE_TCB => "Deleting",
                _ => string.Empty
            };
        }

        public enum State
        {
            CLOSED = 1,
            LISTEN = 2,
            SYN_SENT = 3,
            SYN_RCVD = 4,
            ESTAB = 5,
            FIN_WAIT1 = 6,
            FIN_WAIT2 = 7,
            CLOSE_WAIT = 8,
            CLOSING = 9,
            LAST_ACK = 10,
            TIME_WAIT = 11,
            DELETE_TCB = 12
        }
    }
}
