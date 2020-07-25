using System;
using System.Collections.Generic;
using System.Linq;
using TcpMonitor.Models;
using TcpMonitor.Services;
using TcpMonitor.Views.Common;
using Terminal.Gui;

namespace TcpMonitor.Views
{
    public class TcpPerformanceGrid : View
    {
        private readonly GridView _tcpPerformanceGrid;

        public TcpPerformanceGrid()
        {
            Height = 5;
            Width = Dim.Fill();
            _tcpPerformanceGrid = new GridView();
            _tcpPerformanceGrid.SetRefreshableDataSource(RefreshTcpPerformanceGrid);
            Add(_tcpPerformanceGrid);
        }

        private IEnumerable<TcpPerformanceModel> RefreshTcpPerformanceGrid()
        {
            return new[] { GetTcpPerformance() };
        }

        private TcpPerformanceModel GetTcpPerformance()
        {
            return TcpPerformanceService.GetTcpPerformance();
        }
    }
}
