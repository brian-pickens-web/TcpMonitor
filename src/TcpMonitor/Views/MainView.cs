using System.Collections.Generic;
using System.Linq;
using TcpMonitor.Models;
using TcpMonitor.Services;
using TcpMonitor.Views.Common;
using Terminal.Gui;

namespace TcpMonitor.Views
{
    public sealed class MainView : Window
    {
        public MainView()
        {
            Width = Dim.Fill();
            Height = Dim.Fill();

            var tcpConnectionsGrid = new TcpConnectionsGrid();
            Add(tcpConnectionsGrid);
        }
    }
}
