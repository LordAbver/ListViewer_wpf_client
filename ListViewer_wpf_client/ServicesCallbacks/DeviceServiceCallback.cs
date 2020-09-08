using ListViewer_wpf_client.DeviceServiceReference;
using ListViewer_wpf_client.Structures;

namespace ListViewer_wpf_client.ServicesCallbacks
{
   
    class DeviceServiceCallback:IDeviceServiceCallback
    {
        private CallbackObjects.StateChange _stateChange;
        private CallbackObjects.StorageUpdate _storageUpdate;
        private CallbackObjects.DeviceChange _deviceChange;
        private bool _isStateChanged;
        private bool _isStorageUpdated;
        private bool _isDeviceChanged;
        private bool _checkAvailability;
        public bool IsAvailabilityChecked
        {
            get { return _checkAvailability; }
            set { _checkAvailability = value; }
        }
        public bool IsStateUpdated
        {
            set { _isStateChanged = value; }
        }
        public bool IsStorageUpdated
        {
            set { _isStorageUpdated = value; }
        }
        public bool IsDeviceChanged
        {
            set { _isDeviceChanged = value; }
        }

        public CallbackObjects.StateChange StateChanged
        {
            get
            {
                return (_stateChange.ServerName!=null && _isStateChanged) ? _stateChange : new CallbackObjects.StateChange();
            }
        }
        public CallbackObjects.StorageUpdate StorageUpdate
        {
            get
            {
                return (_storageUpdate.ServerName != null && _isStorageUpdated) ? _storageUpdate : new CallbackObjects.StorageUpdate();
            }
        }
        public CallbackObjects.DeviceChange DeviceChange
        {
            get
            {
                return (_deviceChange.ServerName!=null && _isDeviceChanged) ? _deviceChange : new CallbackObjects.DeviceChange();
            }
        }

        public void OnDevicesChange(string server)
        {
            _isDeviceChanged = true;
            _deviceChange=new CallbackObjects.DeviceChange{ServerName = server};
        }

        public void OnDevicesUpdated(string server, byte[] deviceNumbers)
        {
            //throw new System.NotImplementedException();
        }

        public void OnDevicesDeleted(string server, byte[] deviceNumbers)
        {
            //throw new System.NotImplementedException();
        }

        public void OnDevicesAdded(string server, byte[] deviceNumbers)
        {
            //throw new System.NotImplementedException();
        }

        public void OnConnectionStateChange(string server, ServerStatus serverStatus)
        {
            _isStateChanged = true;
            _stateChange=new CallbackObjects.StateChange{ServerName = server,ServerStatus = serverStatus.ToString()};
        }

        public void OnStorageUpdate(string server, int deviceChannel, StorageNotificationType type)
        {
            _isStorageUpdated = true;
            _storageUpdate=new CallbackObjects.StorageUpdate{DeviceChannel = deviceChannel,NotificationType = type,ServerName = server};
        }

        public void CheckAvailability()
        {
            _checkAvailability = true;
        }

    }
}
