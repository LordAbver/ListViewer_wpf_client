using System;
using ListViewer_wpf_client.AsRunServiceReference;
using ListViewer_wpf_client.Structures;

namespace ListViewer_wpf_client.ServicesCallbacks
{
    class AsRunServiceCallback:IAsRunServiceCallback
    {
        private CallbackObjects.StateChange _asRunConnectionStateChanged;
        private CallbackObjects.OnAsRun _onAsRun;
        private bool _isOnAsRunCallbackRecieved;
        private bool _isConnectionStateChanged;
        public bool IsOnAsRunCallbackRecieved
        {
            set { _isOnAsRunCallbackRecieved = value; }
        }
        public bool IsConnectionStateChanged
        {
            set { _isConnectionStateChanged = value; }   
        }
        public CallbackObjects.OnAsRun OnAsRunData
        {
            get
            {
                return (_onAsRun.ServerName != null && _isOnAsRunCallbackRecieved)? _onAsRun: new CallbackObjects.OnAsRun();
            }
        }
        public CallbackObjects.StateChange AsRunStateChanged
        {
            get
            {
                return (_asRunConnectionStateChanged.ServerName != null && _isConnectionStateChanged) ? _asRunConnectionStateChanged : new CallbackObjects.StateChange();
            }
        }
        public void OnAsRun(string server, int list, AsRunEventDTO[] logAsRun, Guid requestId)
        {
            _isOnAsRunCallbackRecieved = true;
            _onAsRun=new CallbackObjects.OnAsRun(){ServerName = server,ListIndex = list,AsRunEvents = logAsRun,Guid = requestId};
        }

        public void OnConnectionStateChange(string server, ServerStatus serverStatus)
        {
            _isConnectionStateChanged = true;
            _asRunConnectionStateChanged=new CallbackObjects.StateChange(){ServerName = server,ServerStatus = serverStatus.ToString()};
        }
    }
}
