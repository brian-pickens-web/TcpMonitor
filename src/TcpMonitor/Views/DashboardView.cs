using System;
using TcpMonitor.Views.Data;
using TcpMonitor.Views.Framework;
using Terminal.Gui;

namespace TcpMonitor.Views
{
    public sealed class DashboardView : View, IVisibilityChanged
    {
        private readonly Action<bool> VisibilityChangedEvent;

        public DashboardView(TcpSettingsGrid tcpSettingsGrid, TcpPerformanceGrid tcpPerformanceGrid)
        {
            Width = Dim.Fill();
            Height = Dim.Fill();
            VisibilityChangedEvent += tcpPerformanceGrid.VisibilityChanged;

            tcpSettingsGrid.Y = 0;
            tcpPerformanceGrid.Y = Pos.Bottom(tcpSettingsGrid) + 1;

            Add(tcpSettingsGrid);
            Add(tcpPerformanceGrid);
        }

        public void VisibilityChanged(bool isVisible)
        {
            VisibilityChangedEvent?.Invoke(isVisible);
        }
    }
}
