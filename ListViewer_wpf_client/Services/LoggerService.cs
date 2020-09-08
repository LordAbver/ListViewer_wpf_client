using System;
using System.IO;
using System.Windows;
using ListViewer_wpf_client.Constants;
using ListViewer_wpf_client.DeviceServiceReference;
using ListViewer_wpf_client.ListServiceReference;
using ListViewer_wpf_client.Structures;
using ListViewer_wpf_client.ViewModels;

namespace ListViewer_wpf_client.Services
{
    public class LoggerService : DependencyObject
    {
        private OperationLogInfo _operationLogInfo;
        private readonly Log _newLog;
        private string OperationLoggerContent
        {
            get { return (string)GetValue(OperationLoggerContentProperty); }
            set
            {
                SetValue(OperationLoggerContentProperty, value);    
            }
        }
        public static readonly DependencyProperty OperationLoggerContentProperty = DependencyProperty.Register("OperationLoggerContent", typeof(string), typeof(LoggerService), new PropertyMetadata("System logging"));

        private string ListServiceCallBackContent
        {
            get { return (string)GetValue(ListServiceCallbackContentProperty); }
            set
            {
                SetValue(ListServiceCallbackContentProperty, value);
            }
        }
        public static readonly DependencyProperty ListServiceCallbackContentProperty = DependencyProperty.Register("ListServiceCallBackContent", typeof(string), typeof(LoggerService), new PropertyMetadata("List Service Callbacks...\r\n"));
        private string DeviceServiceCallBackContent
        {
            get { return (string)GetValue(DeviceServiceCallbackContentProperty); }
            set
            {
                SetValue(DeviceServiceCallbackContentProperty, value);
            }
        }
        public static readonly DependencyProperty DeviceServiceCallbackContentProperty = DependencyProperty.Register("DeviceServiceCallBackContent", typeof(string), typeof(LoggerService), new PropertyMetadata("Device Service Callbacks...\r\n"));
        private string AsRunServiceCallBackContent
        {
            get { return (string) GetValue(AsRunServiceCallbackContentProperty); }
            set
            {
                SetValue(AsRunServiceCallbackContentProperty,value);
            }
        }
        public static readonly DependencyProperty AsRunServiceCallbackContentProperty = DependencyProperty.Register("AsRunServiceCallBackContent", typeof(string), typeof(LoggerService), new PropertyMetadata("AsRun Service Callbacks...\r\n"));
        public LoggerService()
        {
            //Operation logger
            _newLog = new Log {DataContext = this};
            _newLog.Show();          
        }
        public void PrintInfo(GlobalFlags.InfoMessages msg, string addData = "",GlobalFlags.CallbackDestination cbDist=GlobalFlags.CallbackDestination.OperationLog)
        {
            switch (msg)
            {
                case GlobalFlags.InfoMessages.LoggingTime:
                    AddNote(String.Format("Logging time:{0} \r\n", _operationLogInfo.DateTime.ToString()),GlobalFlags.CallbackDestination.OperationLog);
                    File.AppendAllText(AppConstants.OperationLoggerFname,String.Format("Logging time:{0} \r\n", _operationLogInfo.DateTime.ToString()));
                     break;
                case GlobalFlags.InfoMessages.ServerName:
                    AddNote(String.Format("Current server is: {0} \r\n", _operationLogInfo.ServerName), GlobalFlags.CallbackDestination.OperationLog);
                    File.AppendAllText(AppConstants.OperationLoggerFname,String.Format("Current server is: {0} \r\n", _operationLogInfo.ServerName));
                    break;
                case GlobalFlags.InfoMessages.ClientName:
                    AddNote(String.Format("Client ID is: {0} \r\n", _operationLogInfo.ClientName), GlobalFlags.CallbackDestination.OperationLog);
                    File.AppendAllText(AppConstants.OperationLoggerFname,String.Format("Client ID is: {0} \r\n", _operationLogInfo.ClientName));
                    break;
                case GlobalFlags.InfoMessages.ListLocked:
                    AddNote(String.Format("List {0}({1}) is locked \r\n", _operationLogInfo.AirList, _operationLogInfo.AirListName), GlobalFlags.CallbackDestination.OperationLog);
                    File.AppendAllText(AppConstants.OperationLoggerFname, String.Format("List {0}({1}) is locked \r\n", _operationLogInfo.AirList, _operationLogInfo.AirListName));
                    break;
                case GlobalFlags.InfoMessages.ListUnlocked:
                    AddNote(String.Format("List {0}({1}) is Unlocked \r\n", _operationLogInfo.AirList, _operationLogInfo.AirListName), GlobalFlags.CallbackDestination.OperationLog);
                    File.AppendAllText(AppConstants.OperationLoggerFname,String.Format("List {0}({1}) is Unlocked \r\n", _operationLogInfo.AirList, _operationLogInfo.AirListName));
                    break;
                case GlobalFlags.InfoMessages.DeleteAllEvents:
                    AddNote(String.Format("Perform DeleteAllEvents method List number={0}({1}) \r\n", _operationLogInfo.AirList, _operationLogInfo.AirListName), GlobalFlags.CallbackDestination.OperationLog);
                    File.AppendAllText(AppConstants.OperationLoggerFname, String.Format("List {0}({1}) is Unlocked \r\n", _operationLogInfo.AirList, _operationLogInfo.AirListName));
                    break;
                case GlobalFlags.InfoMessages.NumberOfDevices:
                    AddNote(String.Format("The number of devices on the server {0} is {1} \r\n", _operationLogInfo.ServerName, addData), GlobalFlags.CallbackDestination.OperationLog);
                    break;
                case GlobalFlags.InfoMessages.PrintDevice:
                    AddNote(addData+"\r\n",GlobalFlags.CallbackDestination.OperationLog);
                    break;
                case GlobalFlags.InfoMessages.IsListAvailable:
                    AddNote(String.Format("List number:{0}({1}) is {2} \r\n", _operationLogInfo.AirList, _operationLogInfo.AirListName, _operationLogInfo.IsListAvailable), GlobalFlags.CallbackDestination.OperationLog);
                    break;
                case GlobalFlags.InfoMessages.AvailableDeviceServers:
                    AddNote(String.Format("Available Device Servers List: \r\n"), GlobalFlags.CallbackDestination.OperationLog);
                    int i = 1;
                    foreach (var availableServer in _operationLogInfo.AvailableDeviceServers)
                    {
                        AddNote(String.Format("[{0}]=>{1} \r\n", i, availableServer), GlobalFlags.CallbackDestination.OperationLog);
                        i++;
                    }                        
                        break;
                case GlobalFlags.InfoMessages.CheckAvailability:
                    AddNote("CheckAvailability() \r\n", cbDist);
                    break;
                default:
                    return;
            }
        }
        public void PrintCallBack(GlobalFlags.CallbackDestination cbDist, GlobalFlags.InfoMessages msg)
        {
            PrintInfo(msg, "", cbDist);
        }
        public void PrintCallBack(GlobalFlags.CallbackTypes callbackTypes,EventDTO[] events)
        {
            string callbackType;
            switch (callbackTypes)
            {
                case GlobalFlags.CallbackTypes.OnEvensUpdated:
                    callbackType = "This event was updated on";
                    break;
                case GlobalFlags.CallbackTypes.OnEventsAdded:
                    callbackType = "This event was added on";
                    break;
                case GlobalFlags.CallbackTypes.OnEventsDeleted:
                    callbackType = "This event was deleted from the";
                    break;
                default:
                    return;
            }
            if (events != null && events.Length!=0)
            {
                foreach (var eventDto in events)
                {
                    AddNote(String.Format("{4}  server:{2} in list:{3} =>ID={0} Title={1} Prot. Index={5} \r\n",
                            eventDto.ID, eventDto.Title, _operationLogInfo.ServerName,
                            _operationLogInfo.AirList,callbackType,eventDto.BackupDeviceIndex), GlobalFlags.CallbackDestination.ListService);                   
                    File.AppendAllText(AppConstants.ListserviceCallBackFname, String.Format("{4}  server:{2} in list:{3} =>ID={0} Title={1} Prot. Index={5} \r\n",
                            eventDto.ID, eventDto.Title, _operationLogInfo.ServerName,
                            _operationLogInfo.AirList, callbackType, eventDto.BackupDeviceIndex));
                }
            }
        }
        public void PrintCallBack(CallbackObjects.StateChange state,GlobalFlags.CallbackDestination callbackDestination)
        {
            AddNote(String.Format("[OnConnectionStateChange Callback]=>Device Server:{0} change connection state to:{1} \r\n", state.ServerName, state.ServerStatus), callbackDestination);
            switch(callbackDestination)
            {
                case GlobalFlags.CallbackDestination.DeviceService:
                    File.AppendAllText(AppConstants.DeviceserviceCallBackFname, String.Format("[OnConnectionStateChange Callback]=>Device Server:{0} change connection state to:{1} \r\n", state.ServerName, state.ServerStatus));
                    break;
                case GlobalFlags.CallbackDestination.ListService:
                    File.AppendAllText(AppConstants.ListserviceCallBackFname, String.Format("[OnConnectionStateChange Callback]=>Device Server:{0} change connection state to:{1} \r\n", state.ServerName, state.ServerStatus));
                    break;
                case GlobalFlags.CallbackDestination.AsRunService:
                    File.AppendAllText(AppConstants.AsRunServiceFname, String.Format("[OnConnectionStateChange Callback]=>Device Server:{0} change connection state to:{1} \r\n", state.ServerName, state.ServerStatus));
                    break;
            }                    
        }
        public void PrintCallBack(GlobalFlags.CallbackTypes cTypes,CallbackObjects.ListLock lstLockData)
        {
            string msg = (cTypes == GlobalFlags.CallbackTypes.ListLocked) ? "locked" : "unlocked";
            string fullMsg = String.Format("The list {0} was {1} on server {2} by client {3} \r\n", lstLockData.ListIndex,
                                           msg, lstLockData.ServerName, lstLockData.ClientName);
            AddNote(fullMsg,GlobalFlags.CallbackDestination.ListService);
            File.AppendAllText(AppConstants.ListserviceCallBackFname,fullMsg);
        }
        public void PrintCallBack(CallbackObjects.DeviceChange deviceChange)
        {
            AddNote(String.Format("[OnDevicesChange Callback]=>The device configuration was changed on server:{0} \r\n", deviceChange.ServerName), GlobalFlags.CallbackDestination.DeviceService);
            File.AppendAllText(AppConstants.DeviceserviceCallBackFname, String.Format("[OnDevicesChange Callback]=>The device configuration was changed on server:{0} \r\n", deviceChange.ServerName));
        }
        public void PrintCallBack(CallbackObjects.StorageUpdate storageUpdate)
        {
            string typeString;
            switch (storageUpdate.NotificationType)
            {
                case StorageNotificationType.NeedsRefresh:
                    typeString = "Need Refresh";
                    break;
                case StorageNotificationType.ReadWholeStorage:
                    typeString = "Read Whole Message";
                    break;
                default:
                    typeString = "UNKNOWN";
                    break;
            }
            AddNote(String.Format("[OnStorageUpdate Callback]=>Device Server:{0}, Device channel:{1}, Notifaction Type:{2} \r\n", storageUpdate.ServerName, storageUpdate.DeviceChannel, typeString), GlobalFlags.CallbackDestination.DeviceService);
            File.AppendAllText(AppConstants.DeviceserviceCallBackFname, String.Format("[OnStorageUpdate Callback]=>Device Server:{0}, Device channel:{1}, Notifaction Type:{2} \r\n", storageUpdate.ServerName, storageUpdate.DeviceChannel, typeString));
        }
        public void PrintCallBack(CallbackObjects.ListChange listChange)
        {
            string stringType;
            switch (listChange.ListChangeType)
            {
                case ListChangeType.ContentsChanged:
                    stringType = "Contents Changed";
                    break;
                case ListChangeType.DisplayChanged:
                    stringType = "Display Changed";
                    break;
                case ListChangeType.SystemChanged:
                    stringType = "System Changed";
                    break;
                default:
                    return;
            }
            AddNote(String.Format("LIST NUMBER: {0} (SERVER:{2}) WAS CHANGED, CHANGED TYPE:{1} \r\n", listChange.ListIndex, stringType,listChange.ServerName), GlobalFlags.CallbackDestination.ListService);
            File.AppendAllText(AppConstants.ListserviceCallBackFname, String.Format("LIST NUMBER: {0} (SERVER:{2}) WAS CHANGED, CHANGED TYPE:{1} \r\n", listChange.ListIndex, stringType, listChange.ServerName));          
        }
        public void PrintCallBack(CallbackObjects.OnAsRun onAsRunData)
        {
            foreach(var asRunEvent in onAsRunData.AsRunEvents)
            {
                AddNote(String.Format("[ASRUN EVENT CB] Server={0} List={1},Event ID={2},Event Title={3},Chanel={4} \r\n",
                onAsRunData.ServerName,onAsRunData.ListIndex,asRunEvent.Id,asRunEvent.Title,asRunEvent.Channel),GlobalFlags.CallbackDestination.AsRunService);
            }
        }
        public void OperationLoggerInit(OperationLogInfo operationLogInfo,bool reInit=false)
        {
            if (!reInit)
            {
                _operationLogInfo = operationLogInfo;
            }
            PrintInfo(GlobalFlags.InfoMessages.LoggingTime);
            PrintInfo(GlobalFlags.InfoMessages.ServerName);
            PrintInfo(GlobalFlags.InfoMessages.ClientName);
            PrintInfo(GlobalFlags.InfoMessages.IsListAvailable);
            PrintInfo(GlobalFlags.InfoMessages.AvailableDeviceServers);
        }
        public void OperationLoggerUpdateInfo(OperationLogInfo operationLogInfo)
        {
            Dispatcher.Invoke(new Action(() => OperationLoggerContent =""));
            OperationLoggerInit(operationLogInfo);
        }
        private bool CheckLength(GlobalFlags.CallbackDestination callbackDestination)
        {
            int length = 0;
            switch(callbackDestination)
            {
                case GlobalFlags.CallbackDestination.ListService:
                    Dispatcher.Invoke(new Action(() => length = ListServiceCallBackContent.Length));
                    if (length >= AppConstants.MaxLogLength)
                        return false;
                    break;
                case GlobalFlags.CallbackDestination.DeviceService:
                    Dispatcher.Invoke(new Action(() => length = DeviceServiceCallBackContent.Length));
                    if (length >= AppConstants.MaxLogLength)
                        return false;
                    break;
                case GlobalFlags.CallbackDestination.AsRunService:
                    Dispatcher.Invoke(new Action(() => length = AsRunServiceCallBackContent.Length));
                    if (length >= AppConstants.MaxLogLength)
                        return false;
                    break;
                default:
                    return true;
            }
            return true;
        }
        private void AddNote(string note,GlobalFlags.CallbackDestination callbackDestination)
        {
            try
            {
                switch(callbackDestination)
                {
                    case GlobalFlags.CallbackDestination.OperationLog:
                        Dispatcher.Invoke(new Action(() =>  OperationLoggerContent+=note));
                    break;
                    case GlobalFlags.CallbackDestination.ListService:
                    if (_newLog.IsNeedToClearListServiceContent || !CheckLength(callbackDestination))
                        {
                            Dispatcher.Invoke(new Action(() => ListServiceCallBackContent =""));
                            _newLog.IsNeedToClearListServiceContent = false;
                        }
                        Dispatcher.Invoke(new Action(() => ListServiceCallBackContent += note));
                        break;
                    case GlobalFlags.CallbackDestination.DeviceService:
                        if (_newLog.IsNeedToClearDeviceServiceContent || !CheckLength(callbackDestination))
                        {
                            Dispatcher.Invoke(new Action(() => DeviceServiceCallBackContent = ""));
                            _newLog.IsNeedToClearDeviceServiceContent = false;
                        }
                        Dispatcher.Invoke(new Action(() => DeviceServiceCallBackContent += note));
                        break;
                    case GlobalFlags.CallbackDestination.AsRunService:
                        if (_newLog.IsNeedToClearAsRunServiceContent || !CheckLength(callbackDestination))
                        {
                            Dispatcher.Invoke(new Action(() => AsRunServiceCallBackContent = ""));
                            _newLog.IsNeedToClearAsRunServiceContent = false;
                        }
                        Dispatcher.Invoke(new Action(() => AsRunServiceCallBackContent += note));
                        break;                   
                    default:
                        return;
                }
            }
            catch
            {
                return;
            }            
        }


    }
}
