using System;
using Microsoft.VisualBasic;
using TcpMonitor.Domain;
using TcpMonitor.Models;
using WbemScripting;

namespace TcpMonitor.Services
{
    public class TcpSettingsComService : ITcpSettingsService
    {
        private const string Path = @"winmgmts://./" + TcpSettings.SettingsClassNamespace;

        private static readonly SWbemServicesEx ServicesEx = (SWbemServicesEx)Interaction.GetObject(Path);

        public TcpSettingsModel GetTcpSettings()
        {
            var objectSet = ServicesEx.ExecQuery(TcpSettings.SettingsQuery);
            foreach (ISWbemObjectEx objInstance in objectSet)
            {
                var portStart = Convert.ToInt32(objInstance.Properties_.Item(TcpSettings.DynamicPortRangeStartPortKey).get_Value());
                var portRange = Convert.ToInt32(objInstance.Properties_.Item(TcpSettings.DynamicPortRangeNumberOfPortsKey).get_Value());

                return new TcpSettingsModel()
                {
                    DynamicPortRangeStart = portStart,
                    DynamicPortRangeEnd = portStart + portRange,
                    TotalDynamicPorts = portRange
                };
            }

            return null;
        }
    }
}
