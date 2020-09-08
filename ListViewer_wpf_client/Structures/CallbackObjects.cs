using System;
using ListViewer_wpf_client.AsRunServiceReference;
using ListViewer_wpf_client.DeviceServiceReference;
using ListViewer_wpf_client.ListServiceReference;

namespace ListViewer_wpf_client.Structures
{
    public struct CallbackObjects
    {
        public struct ListChange
        {
            public string ServerName { get; set; }
            public int ListIndex { get; set; }
            public ListChangeType ListChangeType { get; set; }
        }
        public struct StateChange
        {
            public string ServerName { get; set; }
            public string ServerStatus { get; set; }
        }

        public struct StorageUpdate
        {
            public string ServerName { get; set; }
            public int DeviceChannel { get; set; }
            public StorageNotificationType NotificationType  { get; set; }
        }
        public struct DeviceChange
        {
            public string ServerName { get; set; }
        }
        public struct OnAsRun
        {
            public string ServerName { get; set; }
            public int ListIndex { get; set; }
            public AsRunEventDTO[] AsRunEvents { get; set; }
            public Guid Guid { get; set; }
        }
        public struct ListLock
        {
            public string ServerName { get; set; }
            public int ListIndex { get; set; }
            public string ClientName { get; set; }
        }
    }
}
