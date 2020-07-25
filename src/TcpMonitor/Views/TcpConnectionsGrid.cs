using System.Collections.Generic;
using System.Linq;
using TcpMonitor.Models;
using TcpMonitor.Services;
using TcpMonitor.Views.Common;
using Terminal.Gui;

namespace TcpMonitor.Views
{
    public sealed class TcpConnectionsGrid : ScrollView
    {
        private readonly GridView _tcpConnectionsGrid;

        public TcpConnectionsGrid()
            : base(new Rect(0, 5, 100, 21))
        {
            ShowVerticalScrollIndicator = true;
            _tcpConnectionsGrid = new GridView();
            _tcpConnectionsGrid.SetRefreshableDataSource(RefreshTcpConnectionsGrid);
            Add(_tcpConnectionsGrid);
        }

        private IEnumerable<TcpConnectionModel> RefreshTcpConnectionsGrid()
        {
            ContentSize = new Size(99, 2);
            foreach (var tcpConnectionModel in GetTcpConnections())
            {
                ContentSize = Size.Add(ContentSize, new Size(0, 1));
                yield return tcpConnectionModel;
            }
        }

        private IEnumerable<TcpConnectionModel> GetTcpConnections()
        {
            return TcpConnectionService.GetTcpConnectionData()
                .OrderBy(model => model.ProcessId);
        }
    }

}
