using TcpMonitor.Views.Common;
using Terminal.Gui;

namespace TcpMonitor.Views
{
    public sealed class MainView : Window
    {
        private GridView _gridView;

        public MainView()
        {
            Width = Dim.Fill();
            Height = Dim.Fill();
        }
    }
}
