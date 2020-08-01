﻿using System;
using TcpMonitor.Views.Data;
using TcpMonitor.Views.Framework;
using Terminal.Gui;

namespace TcpMonitor.Views
{
    public sealed class TcpConnectionsView : View, IVisibilityChanged
    {
        private readonly Action<bool> VisibilityChangedEvent;

        public TcpConnectionsView(TcpConnectionsGrid tcpConnectionsGrid)
        {
            Width = Dim.Fill();
            Height = Dim.Fill();

            Add(tcpConnectionsGrid);

            VisibilityChangedEvent += tcpConnectionsGrid.VisibilityChanged;
        }

        public void VisibilityChanged(bool isVisible)
        {
            VisibilityChangedEvent(isVisible);
        }
    }
}
