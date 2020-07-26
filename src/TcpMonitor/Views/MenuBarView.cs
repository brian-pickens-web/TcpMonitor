using System.Collections.Generic;
using Terminal.Gui;

namespace TcpMonitor.Views
{
    public sealed class MenuBarView : MenuBar
    {
        // MenuItems
        private static MenuItem QuitMenuItem = new MenuItem("_Quit", string.Empty, Application.RequestStop);

        // MenuBarItems
        private static MenuBarItem FileMenu = new MenuBarItem("_File", new[] { QuitMenuItem });

        public MenuBarView()
        {
            Menus = new []{ FileMenu };
        }
    }
}
