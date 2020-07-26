using Terminal.Gui;

namespace TcpMonitor.Views
{
    public sealed class TcpConnectionsView : View
    {
        public TcpConnectionsView(TcpConnectionsGrid tcpConnectionsGrid)
        {
            Width = Dim.Fill();
            Height = Dim.Fill();

            Add(tcpConnectionsGrid);
        }
    }
}
