using System;
using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;

namespace TcpMonitor.Views
{
    public sealed class SideMenuView : Window
    {
        private readonly List<Action<View>> _menuItemChangedEventHandlers = new List<Action<View>>();
        private readonly Dictionary<string, View> _menu = new Dictionary<string, View>();

        public SideMenuView(DashboardView dashboardView, TcpConnectionsView tcpConnectionsView)
        {
            Width = 20;
            Height = Dim.Fill();
            Title = "Select View";

            _menu.Add("Dashboard", dashboardView);
            _menu.Add("TCP Connections", tcpConnectionsView);

            var views = new ListView();
            views.Height = Dim.Fill();
            views.Width = Dim.Fill();
            views.Source = new ListWrapper(_menu.Keys.ToArray());
            views.SelectedItemChanged = SelectedItemChanged;
            Add(views);
        }

        public Action<View> OnSelectedMenuItemChanged { get; set; } = view => { };

        private void SelectedItemChanged(ListViewItemEventArgs obj)
        {
            OnSelectedMenuItemChanged(_menu[obj.Value.ToString()]);
        }
    }
}
