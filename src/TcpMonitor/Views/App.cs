using Terminal.Gui;

namespace TcpMonitor.Views
{
    public sealed class App : Toplevel
    {
        private View _menuBar = new MenuBar(new[]
        {
            new MenuBarItem("_File", new[] { new MenuItem("_Quit", string.Empty, Application.RequestStop) })
        });

        public App(MainView mainView)
        {
            Add(_menuBar);
            mainView.Y = 1;
            Add(mainView);
        }
    }
}
