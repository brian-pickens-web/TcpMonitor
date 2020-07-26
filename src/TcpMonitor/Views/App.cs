using Terminal.Gui;

namespace TcpMonitor.Views
{
    public sealed class App : Toplevel
    {
        public App(MenuBarView menuBarView, SideMenuView sideMenuView, MainView mainView)
        {
            menuBarView.Y = Pos.Top(this);
            sideMenuView.Y = Pos.Bottom(menuBarView);
            sideMenuView.X = Pos.Left(this);
            mainView.Y = Pos.Bottom(menuBarView);
            mainView.X = Pos.Right(sideMenuView);

            sideMenuView.OnSelectedMenuItemChanged = view =>
            {
                mainView.RemoveAll();
                mainView.Add(view);
            };

            Add(menuBarView);
            Add(sideMenuView);
            Add(mainView);
        }
    }
}
