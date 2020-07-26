using System;
using System.Collections.Generic;
using System.Linq;
using TcpMonitor.Models;
using TcpMonitor.Services;
using TcpMonitor.Views.Common;
using Terminal.Gui;

namespace TcpMonitor.Views
{
    public sealed class TcpPerformanceGrid : View
    {
        private readonly ITcpPerformanceService _tcpPerformanceService;

        public TcpPerformanceGrid(ITcpPerformanceService tcpPerformanceService)
        {
            _tcpPerformanceService = tcpPerformanceService;
            Height = 5;
            Width = Dim.Fill();
            var tcpPerformanceGrid = new GridView();
            tcpPerformanceGrid.SetRefreshableDataSource(RefreshTcpPerformanceGrid);
            Add(tcpPerformanceGrid);
        }

        private IEnumerable<TcpPerformanceModel> RefreshTcpPerformanceGrid()
        {
            return new[] { GetTcpPerformance() };
        }

        private TcpPerformanceModel GetTcpPerformance()
        {
            return _tcpPerformanceService.GetTcpPerformance();
        }
    }
}
