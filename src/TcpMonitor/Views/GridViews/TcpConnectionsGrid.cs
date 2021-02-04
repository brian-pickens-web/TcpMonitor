using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TcpMonitor.Models;
using TcpMonitor.Services;
using TcpMonitor.Views.Framework;
using Terminal.Gui;

namespace TcpMonitor.Views.GridViews
{
    public sealed class TcpConnectionsGrid : View, IVisibilityChanged
    {
        // Services
        private readonly ILogger _logger;
        private readonly ITcpConnectionService _tcpConnectionService;
        private readonly Action<bool> _visibilityChangedEvent;

        // Views
        private readonly ScrollView _connectionsContent;
        private readonly GridView _tcpConnectionsGrid;
        private readonly Button _refreshButton;

        public TcpConnectionsGrid(ILogger logger, ITcpConnectionService tcpConnectionService)
        {
            _logger = logger;
            _tcpConnectionService = tcpConnectionService;

            Height = Dim.Fill();
            Width = Dim.Fill();

            _tcpConnectionsGrid = new GridView() { Height = Dim.Height(this) - 2 };
            _tcpConnectionsGrid.SetSourceAsync(RefreshTcpConnectionsGrid);

            _connectionsContent = new ScrollView()
            {
                Width = Dim.Fill(),
                Height = Dim.Height(this) - 1,
                ContentSize = new Size(80, 4),
                ShowVerticalScrollIndicator = true
            };
            _connectionsContent.Add(_tcpConnectionsGrid);
            
            _refreshButton = new Button()
            {
                Text = "Refresh",
                Y = Pos.Bottom(this) - 1,
                MouseClick = async args => await _tcpConnectionsGrid.UpdateAsync()
            };

            Add(_connectionsContent);
            Add(_refreshButton);

            _visibilityChangedEvent += async b =>
            {
                if (b) await Refresh();
            };
        }

        private async IAsyncEnumerable<TcpConnectionModel> RefreshTcpConnectionsGrid()
        {
            var orderedTcpConnections = _tcpConnectionService
                .GetTcpConnections()
                .OrderBy(model => model.ProcessId);

            var count = 0;
            await foreach (var tcpConnectionModel in orderedTcpConnections)
            {
                count++;
                yield return tcpConnectionModel;
            }

            _connectionsContent.ContentSize = new Size(90, count);
            _logger.LogTrace($"Connections Count: {count}");
        }

        public void VisibilityChanged(bool isVisible)
        {
            _logger.LogInformation($"TcpConnectionGrid Visibility Changed: {isVisible}");
            _visibilityChangedEvent(isVisible);
        }

        private async Task Refresh()
        {
            _refreshButton.Visible = false;
            await _tcpConnectionsGrid.UpdateAsync();
        }
    }

}
