using System;
using Microsoft.Win32;
using TcpMonitor.Views.Framework;
using TcpMonitor.Views.GridViews;
using Terminal.Gui;

namespace TcpMonitor.Views
{
    public sealed class DashboardView : View, IVisibilityChanged
    {
        private const char DividerChar = '-';
        private const string WINDOWS_VERSION_REGKEY = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion";
        private const string WINDOWS_PRODUCT_REGVALUE = @"ProductName";
        private const string WINDOWS_RELEASE_REGVALUE = @"ReleaseId";

        private static readonly string WindowsProduct = Registry.GetValue(
            WINDOWS_VERSION_REGKEY, 
            WINDOWS_PRODUCT_REGVALUE, 
            string.Empty).ToString();

        private static readonly string WindowsRelease = Registry.GetValue(
            WINDOWS_VERSION_REGKEY, 
            WINDOWS_RELEASE_REGVALUE, 
            string.Empty).ToString();

        private readonly Action<bool> VisibilityChangedEvent;

        public DashboardView(TcpSettingsGrid tcpSettingsGrid, TcpPerformanceGrid tcpPerformanceGrid)
        {
            Width = Dim.Fill();
            Height = Dim.Fill();
            VisibilityChangedEvent += tcpPerformanceGrid.VisibilityChanged;

            var colorScheme = new ColorScheme()
            {
                Normal = new Terminal.Gui.Attribute(Color.Black, Color.Blue)
            };

            var windowsVersion = $"{WindowsProduct} {WindowsRelease}";
            var divider = string.Empty.PadLeft(windowsVersion.Length, DividerChar);

            var windowsVersionLabel = new Label(windowsVersion) { Y = 0, ColorScheme = colorScheme };
            var dividerLabel = new Label(divider) { Y = 1, ColorScheme = colorScheme };

            tcpSettingsGrid.Y = Pos.Bottom(dividerLabel) + 1;
            tcpPerformanceGrid.Y = Pos.Bottom(tcpSettingsGrid) + 1;

            Add(windowsVersionLabel);
            Add(dividerLabel);
            Add(tcpSettingsGrid);
            Add(tcpPerformanceGrid);
        }

        public void VisibilityChanged(bool isVisible)
        {
            VisibilityChangedEvent?.Invoke(isVisible);
        }
    }
}
