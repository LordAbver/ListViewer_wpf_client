using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Threading;
using System.ServiceModel;
using System.Configuration;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using ListViewer_wpf_client.Constants;
using ListViewer_wpf_client.ListServiceReference;
using ListViewer_wpf_client.DeviceServiceReference;
using ListViewer_wpf_client.MaterialServiceReference;
using ListViewer_wpf_client.AsRunServiceReference;
using ListViewer_wpf_client.Services;
using ListViewer_wpf_client.ServicesCallbacks;
using ListViewer_wpf_client.Structures;
using ADCEventType = ListViewer_wpf_client.ListServiceReference.ADCEventType;
using DeviceDTO = ListViewer_wpf_client.DeviceServiceReference.DeviceDTO;
using LoginSession = ListViewer_wpf_client.ListServiceReference.LoginSession;
using TimeCodeDTO = ListViewer_wpf_client.ListServiceReference.TimeCodeDTO;

namespace ListViewer_wpf_client.ViewModels
{
    public partial class MainWindow
    {
        #region PUBLIC FIELDS

        public ObservableCollection<EventData> _eventsCollection = new ObservableCollection<EventData>();

        public enum SettingsMode
        {
            UseConfig,
            UseDefaults
        }

        #endregion

        #region PRIVATE FIELDS

        private OperationLogInfo _operationLogInfo;
        private readonly LoginSession _loginSession;
        private readonly MaterialServiceReference.LoginSession _materialServiceLoginSession;
        private readonly AsRunServiceReference.LoginSession _asrunServiceLoginSession;
        private static LoggerService _loggerService;
        private static Thread _callbackProcessor;
        private static Thread _updateStates;
        private static ListServiceClient _lsc;
        private static ListServiceCallback _lscb;
        private static DeviceServiceClient _dsc;
        private static DeviceServiceCallback _dscb;
        private static MaterialServiceClient _msc;
        private static MaterialServiceCallback _mscb;
        private static AsRunServiceClient _arsc;
        private static AsRunServiceCallback _arscb;
        private static EventDTO[] _updatedEvents;
        private static EventDTO[] _addedEvents;
        //private static EventDTO[] _movedEvents;
        private static EventDTO[] _deletedEvents;
        private DeviceDTO[] _deviceCollection;
        private static int _airList;
        private static string _serverName;
        private static string _name;

        #endregion

        [STAThread]
        public static void Main()
        {
            var app = new Application();
            app.Run(new MainWindow());
        }

        #region WINDOW CLOSING

        private void WindowClose(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }

        private static void MainWindowClosing(object sender, CancelEventArgs e)
        {
            var result = MessageBox.Show("Are you sure?", "Exit programm", MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            _callbackProcessor.Abort();
            _updateStates.Abort();
            Application.Current.Shutdown();
        }

        #endregion

        #region CALLBACK PROCESSOR

        private void CallbackProcessor()
        {
            _lscb = new ListServiceCallback();
            _dscb = new DeviceServiceCallback();
            _mscb = new MaterialServiceCallback();
            _arscb = new AsRunServiceCallback();
            while (true)
            {
                //ListServiceCallbacks
                _lsc = new ListServiceClient(new InstanceContext(_lscb));
                _lsc.Endpoint.Behaviors.Find<CallbackBehaviorAttribute>().MaxItemsInObjectGraph = 2147483647;
                //DeviceServiceCallbacks
                _dsc = new DeviceServiceClient(new InstanceContext(_dscb));
                _dsc.Endpoint.Behaviors.Find<CallbackBehaviorAttribute>().MaxItemsInObjectGraph = 2147483647;
                //MaterialServiceCallbaks
                _msc = new MaterialServiceClient(new InstanceContext(_mscb));
                _msc.Endpoint.Behaviors.Find<CallbackBehaviorAttribute>().MaxItemsInObjectGraph = 2147483647;
                //AsRunServiceCallbacks
                _arsc = new AsRunServiceClient(new InstanceContext(_arscb));
                _arsc.Endpoint.Behaviors.Find<CallbackBehaviorAttribute>().MaxItemsInObjectGraph = 2147483647;
                try
                {
                    //ListService
                    _lsc.RegisterConnectionStateListener(_loginSession, _serverName);
                    _lsc.RegisterListListener(new LoginSession(), _serverName, _airList);
                    _lsc.RegisterListListener(new LoginSession(), _serverName, 25); //BA List test
                    //DeviceService
                    _dsc.RegisterConnectionStateListener(_serverName);
                    _dsc.RegisterDeviceListener(_serverName);
                    _dsc.RegisterStorageListener(_serverName);
                    //MaterialService
                    _msc.RegisterDatabaseChangeListener();
                    _msc.RegisterVerifyListListener(_materialServiceLoginSession);
                    //AsRunService
                    _arsc.RegisterAsRunListener(_serverName, _airList);
                    _arsc.RegisterConnectionStateListener(_asrunServiceLoginSession, _serverName);
                    Boolean doWork = true;
                    while (doWork)
                    {
                        Thread.Sleep(5);
                        doWork = _lsc.State == CommunicationState.Opened;
                        //Processing callbacks
                        //List Service
                        _updatedEvents = _lscb.EventsUpdated;
                        if (_updatedEvents.Count() > 0)
                        {
                            UpdateGrid(_updatedEvents, GlobalFlags.CallbackTypes.OnEvensUpdated);
                            _lscb.IsEventUpdated = false;
                            _loggerService.PrintCallBack(GlobalFlags.CallbackTypes.OnEvensUpdated, _updatedEvents);
                        }
                        _addedEvents = _lscb.EventsAdded;
                        if (_addedEvents.Count() > 0)
                        {
                            UpdateGrid(_addedEvents, GlobalFlags.CallbackTypes.OnEventsAdded);
                            _lscb.IsEventAdded = false;
                            _loggerService.PrintCallBack(GlobalFlags.CallbackTypes.OnEventsAdded, _addedEvents);
                        }
                        _deletedEvents = _lscb.EventsDeleted;
                        if (_deletedEvents.Count() > 0)
                        {
                             UpdateGrid(_deletedEvents, GlobalFlags.CallbackTypes.OnEventsDeleted);
                            _lscb.IsEventDeleted = false;
                            _loggerService.PrintCallBack(GlobalFlags.CallbackTypes.OnEventsDeleted, _deletedEvents);
                        }
                        if (_lscb.StateChanged.ServerName != null)
                        {
                            _loggerService.PrintCallBack(_lscb.StateChanged, GlobalFlags.CallbackDestination.ListService);
                            _lscb.IsStateUpdated = false;
                        }
                        if (_lscb.ListChanged.ServerName != null)
                        {
                            _loggerService.PrintCallBack(_lscb.ListChanged);
                            _lscb.IsListChanged = false;
                        }
                        if(_lscb.IsAvailabilityChecked)
                        {
                            _loggerService.PrintCallBack(GlobalFlags.CallbackDestination.ListService,GlobalFlags.InfoMessages.CheckAvailability);
                            _lscb.IsAvailabilityChecked = false;
                        }
                        if(_lscb.ListLockedData.ServerName!=null)
                        {
                            _loggerService.PrintCallBack(GlobalFlags.CallbackTypes.ListLocked, _lscb.ListLockedData);
                            _lscb.IsListLocked = false;
                        }
                        if (_lscb.ListUnlockedData.ServerName != null)
                        {
                            _loggerService.PrintCallBack(GlobalFlags.CallbackTypes.ListUnlocked, _lscb.ListUnlockedData);
                            _lscb.IsListUnLocked= false;
                        }
                        //Device Service
                        if (_dscb.StateChanged.ServerName != null)
                        {
                            _loggerService.PrintCallBack(_dscb.StateChanged,
                                                         GlobalFlags.CallbackDestination.DeviceService);
                            _dscb.IsStateUpdated = false;
                        }
                        if (_dscb.DeviceChange.ServerName != null)
                        {
                            _loggerService.PrintCallBack(_dscb.DeviceChange);
                            _dscb.IsDeviceChanged = false;
                        }
                        if (_dscb.StorageUpdate.ServerName != null)
                        {
                            _loggerService.PrintCallBack(_dscb.StorageUpdate);
                            _dscb.IsStorageUpdated = false;
                        }
                        if(_dscb.IsAvailabilityChecked)
                        {
                            _loggerService.PrintCallBack(GlobalFlags.CallbackDestination.DeviceService,GlobalFlags.InfoMessages.CheckAvailability);
                            _dscb.IsAvailabilityChecked = false;
                        }
                        //AsRun Service
                        if (_arscb.AsRunStateChanged.ServerName != null)
                        {
                            _loggerService.PrintCallBack(_arscb.AsRunStateChanged,
                                                         GlobalFlags.CallbackDestination.AsRunService);
                            _arscb.IsConnectionStateChanged = false;
                        }
                        if (_arscb.OnAsRunData.ServerName != null)
                        {
                            _loggerService.PrintCallBack(_arscb.OnAsRunData);
                            _arscb.IsOnAsRunCallbackRecieved = false;
                        }
                    }
                }
                catch (Exception)
                {
                    GC.Collect();
                }
            }
            // ReSharper disable FunctionNeverReturns
        }

        // ReSharper restore FunctionNeverReturns

        #endregion

        #region MAIN WINDOW CONSTRUCTOR

        public MainWindow(SettingsMode sm = SettingsMode.UseConfig)
        {

            switch (sm)
            {
                case SettingsMode.UseConfig:
                    //Getting data from the configuration file (app.config)
                    _name = ConfigurationManager.AppSettings["ClientName"] + System.Net.Dns.GetHostName() +
                            Guid.NewGuid();
                    _serverName = ConfigurationManager.AppSettings["DeviceServerName"];
                    _airList = Convert.ToInt32(ConfigurationManager.AppSettings["AirList"]);
                    break;
                case SettingsMode.UseDefaults:
                    _name = "default";
                    _serverName = "default";
                    _airList = 1;
                    break;
                default:
                    return;
            }
            _loginSession = new LoginSession();
            _materialServiceLoginSession = new MaterialServiceReference.LoginSession();
            _asrunServiceLoginSession = new AsRunServiceReference.LoginSession();
            _callbackProcessor = new Thread(CallbackProcessor);
            _callbackProcessor.Start();
            InitializeComponent();
            Application.Current.MainWindow.Closing += MainWindowClosing;
            //LoggerServiceInit
            _loggerService = new LoggerService();
            CreateOperationLoggerInfo();
            _loggerService.OperationLoggerInit(_operationLogInfo);
            _updateStates = new Thread(UpdateOperationLOggerInfo);
            _updateStates.Start();
        }

        public void CreateOperationLoggerInfo()
        {
            _operationLogInfo = new OperationLogInfo
                                    {
                                        DateTime = DateTime.Now,
                                        AirList = _airList,
                                        AvailableDeviceServers = GetAvailableDeviceServers(),
                                        IsListAvailable = IsListAvailable,
                                        ServerName = _serverName,
                                        ClientName = _name,
                                        AirListName = ListName
                                    };
        }

        public void UpdateOperationLOggerInfo()
        {
            while (true)
            {
                Thread.Sleep(10000);
                CreateOperationLoggerInfo();
                _loggerService.OperationLoggerUpdateInfo(_operationLogInfo);
            }
            // ReSharper disable FunctionNeverReturns
        }

        // ReSharper restore FunctionNeverReturns

        #endregion

        #region DATA PRINTING

        public void UpdateGrid(EventDTO[] eventsToUpdate, GlobalFlags.CallbackTypes gridUpdateType)
        {
            foreach (var eventDTO in eventsToUpdate)
            {
                var found = _eventsCollection.FirstOrDefault(targetGuid => targetGuid.EventGuid == eventDTO.AdcEventId);
                int index = _eventsCollection.IndexOf(found);
                EventDTO dto = eventDTO;
                if (index >= 0)
                {
                    if (gridUpdateType == GlobalFlags.CallbackTypes.OnEvensUpdated)
                    Dispatcher.Invoke(new Action(() =>_eventsCollection[index] = new EventData
                                                                                    {
                                                                                        EventGuid = dto.AdcEventId,
                                                                                        EventNumber =(index + 1).ToString(),
                                                                                        EventOnAirTime =TimeCodeDTOToStringConverter(dto.OnAirTime, false),
                                                                                        EventID = dto.ID,
                                                                                        EventTitle = dto.Title,
                                                                                        EventStatus =GetStatusString(dto.EventStatus,dto.DeviceIndex),
                                                                                        EventDuration =TimeCodeDTOToStringConverter(dto.Duration),
                                                                                        EventSOM =TimeCodeDTOToStringConverter(dto.Som),
                                                                                        EventProtectedDevice =GetDeviceByIndex(dto.BackupDeviceIndex)
                                                                                    }
                                              ));
                    MarkLine(GetGridLineType(dto.EventStatus, dto.DeviceIndex), index, false);
                    if (gridUpdateType == GlobalFlags.CallbackTypes.OnEventsDeleted)
                    {
                        Dispatcher.Invoke(new Action(() => _eventsCollection.Remove(found)));
                    }
                }
                if (gridUpdateType == GlobalFlags.CallbackTypes.OnEventsAdded)
                {
                Dispatcher.Invoke(new Action(() =>_eventsCollection.Add(new EventData
                                                                           {
                                                                               EventGuid = dto.AdcEventId,
                                                                               EventNumber =(_eventsCollection.Count + 1).ToString(),
                                                                               EventOnAirTime =TimeCodeDTOToStringConverter(dto.OnAirTime, false),
                                                                               EventID = dto.ID,
                                                                               EventTitle = dto.Title,
                                                                               EventStatus =GetStatusString(dto.EventStatus,dto.DeviceIndex),
                                                                               EventDuration =TimeCodeDTOToStringConverter(dto.Duration),
                                                                               EventSOM =TimeCodeDTOToStringConverter(dto.Som),
                                                                               EventProtectedDevice =GetDeviceByIndex(dto.BackupDeviceIndex)
                                                                           })));
                    MarkLine(GetGridLineType(dto.EventStatus, dto.DeviceIndex), _eventsCollection.Count, false);
                }
            }
        }

        private void EventPrint(EventDTO ev, int i)
        {
            _eventsCollection.Add(new EventData
                                      {
                                          EventGuid = ev.AdcEventId,
                                          EventNumber = i.ToString(),
                                          EventOnAirTime = TimeCodeDTOToStringConverter(ev.OnAirTime, false),
                                          EventID = ev.ID,
                                          EventTitle = ev.Title,
                                          EventStatus = GetStatusString(ev.EventStatus, ev.DeviceIndex),
                                          EventDuration = TimeCodeDTOToStringConverter(ev.Duration),
                                          EventSOM = TimeCodeDTOToStringConverter(ev.Som),
                                          EventProtectedDevice = GetDeviceByIndex(ev.BackupDeviceIndex)
                                      });
            MarkLine(GetGridLineType(ev.EventStatus, ev.DeviceIndex), i - 1, true);
        }

        private void MarkLine(GlobalFlags.GridLineTypes lineType, int lineIndex, bool needMassUpdate)
        {
            if (needMassUpdate)
            {
                BaseGrid.UpdateLayout();
                BaseGrid.ScrollIntoView(BaseGrid.Items[lineIndex]);
            }
            var lvitem = BaseGrid.ItemContainerGenerator.ContainerFromIndex(lineIndex) as ListViewItem;
            if (lvitem != null)
            {
                switch (lineType)
                {
                    case GlobalFlags.GridLineTypes.Error:
                        Dispatcher.Invoke(new Action(() => lvitem.Foreground = Brushes.Red));
                        break;
                    case GlobalFlags.GridLineTypes.Running:
                        Dispatcher.Invoke(new Action(() => lvitem.Foreground = Brushes.Green));
                        break;
                    case GlobalFlags.GridLineTypes.Done:
                        Dispatcher.Invoke(new Action(() => lvitem.Foreground = Brushes.Gray));
                        break;
                    case GlobalFlags.GridLineTypes.Ready:
                        Dispatcher.Invoke(new Action(() => lvitem.Foreground = Brushes.Blue));
                        break;
                }
            }
        }
        #endregion

        #region LIST SERVICE

        public ObservableCollection<EventData> EventsCollection
        {
            get { return _eventsCollection; }
        }

        public void GetList(int listName)
        {
            _eventsCollection.Clear();
            try
            {
                _deviceCollection = _dsc.GetDevices(_serverName);
                int i = 1;
                var res = _lsc.GetList(_loginSession, _serverName, Convert.ToInt32(listName));
                res.ToList().ForEach(rec => EventPrint(rec, i++));
                listNameLabel.Content = "List name:" + _lsc.GetListName(_loginSession, _serverName, listName);
                numberOfEventsLabel.Content = "Number of events:" + res.Length;
            }
            catch (Exception)
            {
                MessageBox.Show("Server " + _serverName + "is disconected", "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

        }

        private void PerformListThread(object sender, RoutedEventArgs e)
        {
            _lsc.PerformListThread(_loginSession,_serverName,_airList);
        }
        private void PerformListPlay(object sender, RoutedEventArgs e)
        {
            _lsc.PerformListPlay(_loginSession, _serverName, _airList);
        }
        public void DeleteAllEvents()
        {
            try
            {
                if (_lsc.LockList(_loginSession, _serverName, _airList, _name))
                {
                    _loggerService.PrintInfo(GlobalFlags.InfoMessages.ListLocked);
                }
                _lsc.DeleteAllEvents(_loginSession, _serverName, _airList);
                Thread.Sleep(1000);
                _loggerService.PrintInfo(GlobalFlags.InfoMessages.DeleteAllEvents);
                _lsc.UnlockList(_loginSession, _serverName, _airList, _name);
                _loggerService.PrintInfo(GlobalFlags.InfoMessages.ListUnlocked);
                GetList(_airList);
            }
            catch (Exception)
            {
                return;
                //TODO ERROR Processing
            }
        }

        public string[] GetAvailableDeviceServers()
        {
            try
            {
                return _lsc.GetAvailableDeviceServers(_loginSession);
            }
            catch (Exception)
            {

                return new string[0];
            }

        }

        private string IsListAvailable
        {
            get
            {
                try
                {
                    return (_lsc.IsListAvailable(_loginSession, _serverName, _airList)) ? "Available" : "Not Available";
                }
                catch (Exception)
                {

                    return "Not Available";
                }

            }
        }

        private string ListName
        {
            get
            {
                try
                {
                    return _lsc.GetListName(_loginSession, _serverName, _airList);
                }
                catch (Exception)
                {

                    return "Not Available";
                }

            }
        }

        private void ShowButtonClick(object sender, RoutedEventArgs e)
        {
            int lister = Convert.ToInt32(listNumber.Text);
            _airList = lister;
            GetList(lister);
        }

        private GlobalFlags.GridLineTypes GetGridLineType(EventRunStatus[] eventStatus, int deviceIndex)
        {
            GlobalFlags.GridLineTypes result = GlobalFlags.GridLineTypes.Ready;
            if (eventStatus.Contains(EventRunStatus.Done))
            {
                result = GlobalFlags.GridLineTypes.Done;
            }
            else if (eventStatus.Contains(EventRunStatus.Running))
            {
                result = GlobalFlags.GridLineTypes.Running;
            }
            else if (deviceIndex <= 0)
            {
                result = GlobalFlags.GridLineTypes.Error;
            }
            return result;
        }

        private string GetStatusString(EventRunStatus[] eventStatus, int deviceIndex)
        {
            string result = "READY";
            if (eventStatus.Contains(EventRunStatus.Done))
            {
                result = "DONE";
            }
            else if (eventStatus.Contains(EventRunStatus.Running))
            {
                result = "RUNNING";
            }
            else if (deviceIndex <= 0)
            {
                result = "ERROR";
            }
            return result;
        }

        private void DaeItemClick(object sender, RoutedEventArgs e)
        {
            DeleteAllEvents();
        }

        #endregion

        #region TIME DECODERS

        public string TimeCodeDTOToStringConverter(TimeCodeDTO timeCodeDTO, bool needFrames = true)
        {
            int hh = timeCodeDTO.Hour;
            int mm = timeCodeDTO.Minute;
            int ss = timeCodeDTO.Second;
            int frames = timeCodeDTO.Frame;
            if (needFrames)
            {
                return String.Format("{0}:{1}:{2};{3}", TimeDecoder(hh), TimeDecoder(mm), TimeDecoder(ss),
                                     TimeDecoder(frames));
            }
            return String.Format("{0}:{1}:{2}", TimeDecoder(hh), TimeDecoder(mm), TimeDecoder(ss));
        }

        public string TimeDecoder(int timePart)
        {
            string result = timePart.ToString();
            if (timePart < 10)
            {
                result = String.Format("{0}{1}", "0", timePart);
            }
            else if (timePart > 59)
            {
                result = "00";
            }
            return result;
        }

        #endregion

        #region DEVICE SERVICE SECTION

        public void GetDevices()
        {
         List<DeviceDTO> devices = new List<DeviceDTO>();
         try
            {
                
                int deviceCount = _dsc.GetDevicesCount(_serverName);
                for (int i = 1; i <= deviceCount; i++)
                {
                 try
                    {
                      devices.AddRange(_dsc.GetDevicesByNumbers(_serverName,new[]{(byte)i})); 
                    }
                 catch (FaultException)
                    {
                        continue;
                    }
                }
            }
            catch(Exception ex)
            {
                return;
            }
            devices.ToList().ForEach(device =>
                                        _loggerService.PrintInfo(GlobalFlags.InfoMessages.PrintDevice,
                                                                 String.Format(
                                                                     "Device name : {0} \t Device number : {1} \t Device type : {2} ",
                                                                     device.DeviceName, device.DeviceNumber,
                                                                     device.DeviceType))
                    );
        }
        private static EventDTO SimpleEvent(string newID, byte hour = 0, byte min = 0, byte sec = 15, string title = "", ADCEventType type = ADCEventType.Primary)
        {
            var duration = new ListServiceReference.TimeCodeDTO()
            {
                Hour = hour,
                Minute = min,
                Second = sec,
                FrameRate =
                    new ListServiceReference.FrameRateDTO
                    {
                        FrameRate = ListServiceReference.FrameRateEnum.NTSC,
                        Dropframe = false
                    }
            };

            return new EventDTO
            {
                AdcEventId = Guid.Empty,
                EventType = type,
                ID = newID,
                Duration = duration,
                Som = duration,
                OnAirTime = duration,
                CompileSom = duration,
                Title = title,
                SegmentNumber = 255,
                ABoxSom = duration,
                BBoxSom = duration
            };
        }
        private string GetDeviceByIndex(int backupDeviceIndex)
        {
            string deviceName = "NOT FOUND";
            try
            {

                for (int i = 0; i < _deviceCollection.ToList().Count; i++)
                {
                    if (_deviceCollection[i].DeviceNumber == backupDeviceIndex)
                    {
                        return String.Format("{0}:{1}", _deviceCollection[i].DeviceName,
                                             _deviceCollection[i].DeviceNumber);
                    }
                }
            }
            catch (Exception)
            {
                return deviceName;
                //TODO Error processing!!
            }
            return deviceName;
        }

        private void GetDevicesClick(object sender, RoutedEventArgs e)
        {
            //GetDevices();               
             try
             {
                 _lsc.PerformBreakAway("ILYIN", _airList, new EventDTO[1] { SimpleEvent("Demo0001") });
                /* var res = new BreakAwayConfigurationDTO();
                var group = new BreakAwayGroupDTO();
                group.Commercial = false;
                group.Program = false;
                group.Slide = false;
                group.Jip = false;
                group.DefaultPath = "test2";
                group.SequencePath = "test45";
                group.GroupName = "Test BreakAway";
                group.GapTime = new TimeCodeDTO();
                group.MinTime = new TimeCodeDTO();
                group.MaxTime = new TimeCodeDTO();
                group.MaxFill = false;
                group.Channels = new List<ChannelDescription>() { new ChannelDescription() { ServerName = "ILYIN", List = 1 }, 
                                                                  new ChannelDescription() { ServerName = "ILYIN", List = 2 }}.ToArray();
                res.GroupList = new List<BreakAwayGroupDTO> { group }.ToArray();
                 _lsc.SetBreakAwayConfiguration(res);
                 _lsc.PerformBreakAway("ILYIN", _airList, new EventDTO[1] { SimpleEvent("Demo0001") });
                 /*var listCount = _lsc.GetListCount(_serverName);
                 var target = 6;
                 var breakAwayLists = new List<int>();
                 var transmissionLists = new List<int>();
                 int k = 0;
                 for(var i=1;i<=listCount;i++)
                 {
                     var listType = _lsc.GetListType(_loginSession, _serverName, i);
                     if(listType==TypeOfList.Breakaway)
                     {
                         breakAwayLists.Add(i);
                     }
                     if (listType == TypeOfList.Sequence)
                     {
                         transmissionLists.Add(i);
                     }
                 }
                 var search = transmissionLists.IndexOf(target);
                 var result = breakAwayLists[search];
                 var a = 5;*/
                 //var test1 = _lsc.GetBreakAwayListStatus(_serverName, 2);
                 //var p = 6;

             }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK,
                                         MessageBoxImage.Error);
            }

        }
        private void ConfigureListServiceClick(object sender,RoutedEventArgs e)
        {
            try
            {
                ListServiceConfig listServiceConfig = new ListServiceConfig();
                listServiceConfig.Show();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK,
                                        MessageBoxImage.Error);
            } 
        }
        private void MaterialServiceTestClick(object sender, RoutedEventArgs e)
        {
            return;
        }
        #endregion      

    }
}
