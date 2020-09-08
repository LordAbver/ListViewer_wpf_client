using System;

namespace ListViewer_wpf_client.Structures
{
    public struct OperationLogInfo
    {
        public DateTime DateTime { get; set; }
        public string ServerName { get; set; }
        public string ClientName { get; set; }
        public int AirList { get; set;}
        public string AirListName { get; set; }
        public string IsListAvailable { get; set; }
        public string[] AvailableDeviceServers { get; set; }
    }
}
