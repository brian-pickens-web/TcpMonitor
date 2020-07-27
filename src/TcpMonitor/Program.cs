using SimpleInjector;
using TcpMonitor.Views;
using Terminal.Gui;

namespace TcpMonitor
{
    /// <summary>
    /// Uses Terminal.Gui at https://github.com/migueldeicaza/gui.cs
    /// </summary>
    public class Program
    {
        private static Container _serviceProvider;

        public static void Main()
        {
            _serviceProvider = ConfigurationFactory.CreateContainer();
            Application.Init();
            Application.Run(_serviceProvider.GetInstance<App>());
        }
    }
}
