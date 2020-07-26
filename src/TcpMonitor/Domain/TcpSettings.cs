namespace TcpMonitor.Domain
{
    static class TcpSettings
    {
        public const string SettingsClassNamespace = Wmi.StandardCimv2Namespace;
        public const string SettingsClassName = "MSFT_NetTCPSetting";
        public const string DynamicPortRangeStartPortKey = "DynamicPortRangeStartPort";
        public const string DynamicPortRangeNumberOfPortsKey = "DynamicPortRangeNumberOfPorts";
        public const string SettingsQuery = "SELECT * FROM MSFT_NetTCPSetting WHERE CreationClassName='' AND PolicyActionName='' AND PolicyRuleCreationClassName='' AND PolicyRuleName='Internet' AND SystemCreationClassName='' AND SystemName=''";
    }
}
