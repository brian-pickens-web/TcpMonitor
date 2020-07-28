using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using TcpMonitor.Models;
using TcpMonitor.Services;
using TcpMonitor.Views.Common;
using Terminal.Gui;

namespace TcpMonitor.Views.Data
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
            ShowVerticalScrollIndicator = true;

            var tcpConnectionsGrid = new GridView();
            tcpConnectionsGrid.SetRefreshableDataSource(RefreshTcpConnectionsGrid);
            Add(tcpConnectionsGrid);

            VisibilityChangedEvent += isVisible =>
            {
                if (isVisible)
                {
                    _logger.LogTrace("TcpConnectionsGrid.StartRefresh()");
                    tcpConnectionsGrid.StartRefresh();
                }
                else
                {
                    _logger.LogTrace("TcpConnectionsGrid.StopRefresh()");
                    tcpConnectionsGrid.StopRefresh();
                }
            };
        }

        private IEnumerable<TcpConnectionModel> RefreshTcpConnectionsGrid()
        {
            ContentSize = new Size(109, 2);
            foreach (var tcpConnectionModel in GetTcpConnections())
            {
                ContentSize = Size.Add(ContentSize, new Size(0, 1));
                yield return tcpConnectionModel;
            }
        }

        private IEnumerable<TcpConnectionModel> GetTcpConnections()
        {
            return _tcpConnectionService.GetTcpConnections()
                .OrderBy(model => model.ProcessId);
        }

        public void VisibilityChanged(bool isVisible)
        {
            _logger.LogInformation($"TcpConnectionGrid Visibility Changed: {isVisible}");
            VisibilityChangedEvent(isVisible);
        }
    }

}
