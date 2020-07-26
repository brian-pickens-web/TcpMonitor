using Terminal.Gui;

namespace TcpMonitor.Views
{
    public sealed class DashboardView : View
    {
        public DashboardView(TcpSettingsGrid tcpSettingsGrid, TcpPerformanceGrid tcpPerformanceGrid)
        {
            Width = Dim.Fill();
            Height = Dim.Fill();

            tcpSettingsGrid.Y = 0;
            tcpPerformanceGrid.Y = Pos.Bottom(tcpSettingsGrid) + 1;

            Add(tcpSettingsGrid);
            Add(tcpPerformanceGrid);
        }
    }
}
