using System;

namespace ListViewer_wpf_client
{
    public struct EventData
    {
        public Guid EventGuid { get; set; }
        public string EventNumber { get; set; }
        public string EventOnAirTime { get; set; }
        public string EventID { get; set; }
        public string EventTitle { get; set; }
        public string EventStatus { get; set; }
        public string EventDuration { get; set; }
        public string EventSOM { get; set; }
        public string EventProtectedDevice { get; set; }
    }
}
