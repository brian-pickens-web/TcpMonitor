using TcpMonitor.Services;
using TcpMonitor.Views.Framework;
using Terminal.Gui;

namespace TcpMonitor.Views.GridViews
{
    public sealed class TcpSettingsGrid : View
    {
        public TcpSettingsGrid(ITcpSettingsService tcpSettingsService)
        {
            Height = 4;
            Width = Dim.Fill();
            var settingsGrid = new GridView();
            var data = tcpSettingsService.GetTcpSettings().GetAwaiter().GetResult();
            settingsGrid.SetSource(data);
            Add(settingsGrid);

            // Application.Loaded += async (args) =>
            // {
            //     var data = await tcpSettingsService.GetTcpSettings();
            //     settingsGrid.SetSource(data);
            // };
        }
    }
}
