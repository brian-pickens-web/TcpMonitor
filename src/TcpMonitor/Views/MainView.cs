using Terminal.Gui;

namespace TcpMonitor.Views
{
    public sealed class MainView : Window
    {
        public MainView(TcpSettingsGrid tcpSettingsGrid, TcpPerformanceGrid tcpPerformanceGrid, TcpConnectionsGrid tcpConnectionsGrid)
        {
            Width = Dim.Fill();
            Height = Dim.Fill();

            tcpSettingsGrid.Y = 0;
            tcpPerformanceGrid.Y = Pos.Bottom(tcpSettingsGrid) + 1;
            tcpConnectionsGrid.Y = Pos.Bottom(tcpPerformanceGrid) + 1;

            Add(tcpSettingsGrid);
            Add(tcpPerformanceGrid);
            Add(tcpConnectionsGrid);
        }
    }
}
