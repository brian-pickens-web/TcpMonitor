using Terminal.Gui;

namespace TcpMonitor.Views
{
    public sealed class MainView : Window
    {
        public MainView(TcpPerformanceGrid tcpPerformanceGrid, TcpConnectionsGrid tcpConnectionsGrid)
        {
            Width = Dim.Fill();
            Height = Dim.Fill();

            Add(tcpPerformanceGrid);
            Add(tcpConnectionsGrid);
        }
    }
}
