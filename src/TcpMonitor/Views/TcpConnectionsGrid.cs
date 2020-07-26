﻿using System.Collections.Generic;
using System.Linq;
using TcpMonitor.Models;
using TcpMonitor.Services;
using TcpMonitor.Views.Common;
using Terminal.Gui;

namespace TcpMonitor.Views
{
    public sealed class TcpConnectionsGrid : ScrollView
    {
        private readonly ITcpConnectionService _tcpConnectionService;

        public TcpConnectionsGrid(ITcpConnectionService tcpConnectionService)
        {
            Height = 18;
            Width = 115;
            ShowVerticalScrollIndicator = true;
            _tcpConnectionService = tcpConnectionService;
            var tcpConnectionsGrid = new GridView();
            tcpConnectionsGrid.SetRefreshableDataSource(RefreshTcpConnectionsGrid);
            Add(tcpConnectionsGrid);
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
            return _tcpConnectionService.GetTcpConnectionData()
                .OrderBy(model => model.ProcessId);
        }
    }

}
