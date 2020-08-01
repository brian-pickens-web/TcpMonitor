using System;
using Microsoft.Extensions.Logging;
using TcpMonitor.Services;
using TcpMonitor.Views.Framework;
using Terminal.Gui;

namespace TcpMonitor.Views.GridViews
{
    public sealed class TcpPerformanceGrid : View, IVisibilityChanged
    {
        private readonly ILogger _logger;
        private readonly Action<bool> VisibilityChangedEvent;

        public TcpPerformanceGrid(ILogger logger, ITcpPerformanceService tcpPerformanceService)
        {
            _logger = logger;

            Height = 4;
            Width = Dim.Fill();

            var tcpPerformanceGrid = new GridView();
            tcpPerformanceGrid.SetSourceAsync(tcpPerformanceService.GetTcpPerformance);
            Add(tcpPerformanceGrid);

            VisibilityChangedEvent += isVisible =>
            {
                if (isVisible)
                {
                    _logger.LogTrace("TcpPerformanceGrid.StartRefresh()");
                    tcpPerformanceGrid.StartRefresh(3001);
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
