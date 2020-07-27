using System;
using Microsoft.Extensions.Logging;
using TcpMonitor.Services;
using TcpMonitor.Views.Common;
using Terminal.Gui;

namespace TcpMonitor.Views.Data
{
    public sealed class TcpPerformanceGrid : View, IVisibilityChanged
    {
        private readonly ILogger _logger;
        private readonly Action<bool> VisibilityChangedEvent;

        public TcpPerformanceGrid(ILogger logger, ITcpPerformanceService tcpPerformanceService)
        {
            _logger = logger;

            Height = 3;
            Width = Dim.Fill();

            var tcpPerformanceGrid = new GridView();
            tcpPerformanceGrid.SetRefreshableDataSource(tcpPerformanceService.GetTcpPerformance);
            Add(tcpPerformanceGrid);

            VisibilityChangedEvent += isVisible =>
            {
                if (isVisible)
                {
                    _logger.LogTrace("TcpPerformanceGrid.StartRefresh()");
                    tcpPerformanceGrid.StartRefresh();
                }
                else
                {
                    _logger.LogTrace("TcpPerformanceGrid.StopRefresh()");
                    tcpPerformanceGrid.StopRefresh();
                }
            };
        }

        public void VisibilityChanged(bool isVisible)
        {
            _logger.LogInformation($"TcpPerformanceGrid Visibility Changed: {isVisible}");
            VisibilityChangedEvent(isVisible);
        }
    }
}
