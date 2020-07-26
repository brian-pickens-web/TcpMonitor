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
            Height = 3;
            Width = Dim.Fill();
            var tcpPerformanceGrid = new GridView();
            tcpPerformanceGrid.SetRefreshableDataSource(GetTcpPerformance);
            Add(tcpPerformanceGrid);
        }

        private TcpPerformanceModel GetTcpPerformance()
        {
            return _tcpPerformanceService.GetTcpPerformance();
        }
    }
}
