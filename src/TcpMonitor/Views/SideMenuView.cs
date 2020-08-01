using System;
using System.Collections.Generic;
using System.Linq;
using TcpMonitor.Views.Framework;
using Terminal.Gui;

namespace TcpMonitor.Views
{
    public sealed class SideMenuView : Window
    {
        private readonly Dictionary<string, View> _menu = new Dictionary<string, View>();

        public SideMenuView(DashboardView dashboardView, TcpConnectionsView tcpConnectionsView)
        {
            Width = 20;
            Height = Dim.Fill();
            Title = "Select View";

            _menu.Add("Dashboard", dashboardView);
            _menu.Add("TCP Connections", tcpConnectionsView);

            OnSelectedMenuItemChanged += SetViewsVisibility;

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
            var selectedView = _menu[obj.Value.ToString()];
            OnSelectedMenuItemChanged(selectedView);
        }

        private void SetViewsVisibility(View view)
        {
            foreach (var item in _menu.Values.Select(view => view as IVisibilityChanged))
            {
                item?.VisibilityChanged(false);
            }
            var trySetVisibility = view as IVisibilityChanged;
            trySetVisibility?.VisibilityChanged(true);
        }
    }
}
