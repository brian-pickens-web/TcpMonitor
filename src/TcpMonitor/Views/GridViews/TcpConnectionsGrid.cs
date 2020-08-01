using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using TcpMonitor.Models;
using TcpMonitor.Services;
using TcpMonitor.Views.Framework;
using Terminal.Gui;

namespace TcpMonitor.Views.GridViews
{
    public sealed class TcpConnectionsGrid : ScrollView, IVisibilityChanged
    {
        private readonly ILogger _logger;
        private readonly ITcpConnectionService _tcpConnectionService;
        private readonly Action<bool> VisibilityChangedEvent;

        public TcpConnectionsGrid(ILogger logger, ITcpConnectionService tcpConnectionService)
        {
            _logger = logger;
            _tcpConnectionService = tcpConnectionService;

            Height = Dim.Fill();
            Width = Dim.Fill();
            ContentSize = new Size(80, 4);
            ShowVerticalScrollIndicator = true;

            var tcpConnectionsGrid = new GridView();
            tcpConnectionsGrid.SetSourceAsync(RefreshTcpConnectionsGrid);
            tcpConnectionsGrid.RefreshSize = new Progress<Size>(UpdateContentSize);
            
            Add(tcpConnectionsGrid);

            VisibilityChangedEvent += isVisible =>
            {
                if (isVisible)
                {
                    _logger.LogTrace("TcpConnectionsGrid.StartRefresh()");
                    tcpConnectionsGrid.StartRefresh(6000);
                }
                else
                {
                    _logger.LogTrace("TcpConnectionsGrid.StopRefresh()");
                    tcpConnectionsGrid.StopRefresh();
                }
            };
        }

        private void UpdateContentSize(Size gridSize)
        {
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
            ContentSize = new Size(90, count);
            _logger.LogTrace($"Connections Count: {count}");
        }

        public void VisibilityChanged(bool isVisible)
        {
            _logger.LogInformation($"TcpConnectionGrid Visibility Changed: {isVisible}");
            VisibilityChangedEvent(isVisible);
        }
    }

}
