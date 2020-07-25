using System.Collections.Generic;
using System.Linq;
using TcpMonitor.Models;
using TcpMonitor.Services;
using TcpMonitor.Views.Common;
using Terminal.Gui;

namespace TcpMonitor.Views
{
    public sealed class MainView : Window
    {
        private readonly GridView _tcpConnectionsGrid;

        public MainView()
        {
            Width = Dim.Fill();
            Height = Dim.Fill();

            _tcpConnectionsGrid = new GridView();
            // _tcpConnectionsGrid.SetDataSource(GetTcpConnections());
            _tcpConnectionsGrid.SetRefreshableDataSource(RefreshTcpConnectionsGrid);
            // Add(_tcpConnectionsGrid);
        }

        private IEnumerable<TcpConnectionModel> RefreshTcpConnectionsGrid()
        {
            Remove(_tcpConnectionsGrid);
            foreach (var tcpConnectionModel in GetTcpConnections())
            {
                yield return tcpConnectionModel;
            }
            Add(_tcpConnectionsGrid);
        }

        private IEnumerable<TcpConnectionModel> GetTcpConnections()
        {
            return TcpConnectionService.GetTcpConnectionData()
                .OrderBy(model => model.ProcessId);
        }
    }
}
