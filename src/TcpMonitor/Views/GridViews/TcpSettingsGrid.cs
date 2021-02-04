using System.Threading.Tasks;
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
            settingsGrid.SetSourceAsync(tcpSettingsService.GetTcpSettings);
            Add(settingsGrid);

            Task.Run(async () => await settingsGrid.UpdateAsync());
        }
    }
}
