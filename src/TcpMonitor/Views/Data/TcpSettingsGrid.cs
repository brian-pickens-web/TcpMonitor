using TcpMonitor.Services;
using TcpMonitor.Views.Common;
using Terminal.Gui;

namespace TcpMonitor.Views
{
    public sealed class TcpSettingsGrid : View
    {
        public TcpSettingsGrid(ITcpSettingsService tcpSettingsService)
        {
            Height = 3;
            Width = Dim.Fill();

            var settingsGrid = new GridView();
            settingsGrid.SetDataSource(tcpSettingsService.GetTcpSettings());
            Add(settingsGrid);
        }
    }
}
