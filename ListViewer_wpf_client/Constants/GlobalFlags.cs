namespace ListViewer_wpf_client.Constants
{
    public struct GlobalFlags
    {
        public enum InfoMessages
        {
            ServerName,
            ClientName,
            ListLocked,
            ListUnlocked,
            DeleteAllEvents,
            NumberOfDevices,
            PrintDevice,
            IsServerAvailable,
            IsListAvailable,
            AvailableDeviceServers,
            LoggingTime,
            CheckAvailability
        }
        public enum CallbackTypes
        {
            OnEventsDeleted,
            OnEventsAdded,
            OnEvensUpdated,
            OnConnectionStateChanged,
            OnListChanged,
            ListLocked,
            ListUnlocked
        }
        public enum CallbackDestination
        {
            OperationLog,
            ListService,
            DeviceService,
            AsRunService
        }
        public enum DeviceServiceCallbackTypes
        {
            OnDevicesChange,
            OnConnectionStateChange,
            OnStorageUpdate
        }
        public enum GridLineTypes
        {
            Error,
            Running,
            Ready,
            Done
        }
    }
}
