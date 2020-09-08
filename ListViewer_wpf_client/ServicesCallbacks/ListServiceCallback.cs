using System;
using System.ServiceModel;
using ListViewer_wpf_client.ListServiceReference;
using ListViewer_wpf_client.Structures;

namespace ListViewer_wpf_client.ServicesCallbacks
{
    [CallbackBehavior(UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Single)]
    public class ListServiceCallback : IListServiceCallback
    {
        private EventDTO[] _updatedEvents;
        private EventDTO[] _addedEvents;
        //private EventDTO[] _movedEvents;
        private EventDTO[] _deletedEvents;
        private CallbackObjects.StateChange _stateChange;
        private CallbackObjects.ListChange _listChange;
        private CallbackObjects.ListLock _listLocked;
        private CallbackObjects.ListLock _listUnlocked;
        private bool _isListLocked;
        private bool _isListUnlocked;
        private bool _isStateChanged;
        private bool _isEventUpdated;
        private bool _isEventAdded;
        //private bool _isEventMoved;
        private bool _isEventDeleted;
        private bool _isListChanged;
        private bool _checkAvailability;
        public bool IsAvailabilityChecked
        {
            get { return _checkAvailability; }
            set { _checkAvailability = value; }
        }
        public bool IsEventUpdated
        {
            set { _isEventUpdated= value; }
        }
        public bool IsEventAdded
        {
            set { _isEventAdded = value; }
        }
       /* public bool IsEventMoved
        {
            set { _isEventMoved = value; }
        }*/
        public bool IsEventDeleted
        {
            set { _isEventDeleted = value; }
        }
        public bool IsStateUpdated
        {
            set { _isStateChanged = value; }
        }
         public bool IsListChanged
        {
            set { _isListChanged = value; }
        }
        public bool IsListLocked
        {
            set { _isListLocked = value; }
        }
        public bool IsListUnLocked
        {
            set { _isListUnlocked = value; }
        }
        public EventDTO[] EventsUpdated
        {
            get
            {
                return (_updatedEvents!=null && _isEventUpdated) ?_updatedEvents: new EventDTO[0];
            }
        }
        public EventDTO[] EventsAdded
        {
            get
            {
                return (_addedEvents != null && _isEventAdded) ? _addedEvents : new EventDTO[0];
            }
        }
       /* public EventDTO[] EventsMoved
        {
            get
            {
                return (_movedEvents != null && _isEventMoved) ? _movedEvents : new EventDTO[0];
            }
        }*/
        public EventDTO[] EventsDeleted
        {
            get
            {
                return (_deletedEvents != null && _isEventDeleted) ? _deletedEvents : new EventDTO[0];
            }
        }
        public CallbackObjects.StateChange StateChanged
        {
            get
            {
                return (_stateChange.ServerName != null && _isStateChanged) ? _stateChange : new CallbackObjects.StateChange();
            }
        }
        public CallbackObjects.ListChange ListChanged
        {
            get
            {
                return (_listChange.ServerName != null && _isListChanged) ? _listChange : new CallbackObjects.ListChange();
            }
        }
        public CallbackObjects.ListLock ListLockedData
        {
            get
            {
                return (_listLocked.ServerName != null && _isListLocked) ? _listLocked : new CallbackObjects.ListLock();
            }
        }
        public CallbackObjects.ListLock ListUnlockedData
        {
            get
            {
                return (_listUnlocked.ServerName != null && _isListUnlocked) ? _listUnlocked : new CallbackObjects.ListLock();
            }
        }
        public void OnListChange(string server, int list, ListChangeType changeType)
        {
           _isListChanged = true;
           _listChange=new CallbackObjects.ListChange{ServerName = server,ListIndex = list,ListChangeType = changeType};
        }

        public void OnConnectionStateChange(string server, ServerStatus serverStatus)
        {
            _isStateChanged = true;
            _stateChange = new CallbackObjects.StateChange{ ServerName = server, ServerStatus = serverStatus.ToString() };
        }

        public void OnEventsUpdated(string server, int list, EventDTO[] updatedEvents)
        {
            _updatedEvents = updatedEvents;
            _isEventUpdated = true;
        }

        public void OnEventsDeleted(string server, int list, EventDTO[] deletedEvents)
        {
            _deletedEvents = deletedEvents;
            _isEventDeleted = true;
        }

        public void OnEventsAdded(string server, int list, Guid afterID, EventDTO[] addedEvents)
        {
            _addedEvents = addedEvents;
            _isEventAdded = true;
        }

        public void OnEventsMoved(string server, int list, Guid source, Guid destination, Guid[] movedEvents)
        {
            //TODO NOT IMPLEMENTED YET
           /* var playlist = Client.GetList(new LoginSession(), server, list).ToList();
            for (int i = 0; i < movedEvents.Length; i++)
                Console.WriteLine("ID {0} Guid {1}",
                                  playlist.Find(ev => ev.AdcEventId == movedEvents[i]).ID, movedEvents[i].ToString());*/
        }

        public void ListLocked(string server, int list, string clientName)
        {
            _listLocked = new CallbackObjects.ListLock {ServerName = server, ListIndex = list, ClientName = clientName};
            _isListLocked = true;
        }

        public void ListUnlocked(string server, int list, string clientName)
        {
            _listUnlocked = new CallbackObjects.ListLock { ServerName = server, ListIndex = list, ClientName = clientName };
            _isListUnlocked = true;
        }

        public void OnBreakAwayListStatusChanged(string server,Int32 list,BreakAwayListStatusDTO breakAwayListStatus)
        {
            var a = breakAwayListStatus;
            var b = 5;
        }

        public void CheckAvailability()
        {
            _checkAvailability = true;
        }
       
    }
}

