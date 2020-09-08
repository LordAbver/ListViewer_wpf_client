using System;
using System.Linq;
using ListViewer_wpf_client.Configuration;
using ListViewer_wpf_client.ManagerService;

namespace ListViewer_wpf_client.Helpers
{
    class ConfigurationHelper
    {
        private static ManagerServiceClient _mgsc;
        private Guid _serviceId;
        private string _currentCfg;
        private readonly ServiceType _serviceType;

        public ConfigurationHelper(ServiceType serviceType)
        {
                _serviceType = serviceType;
                _mgsc = new ManagerServiceClient();
        }
        public ADCConnectionParameters GetCurrentConfiguration()
        {
            var services = _mgsc.GetServicesInformation().ToList();
            _serviceId = services.Find(listDto => listDto.Type == _serviceType).ServiceId;
            _currentCfg = _mgsc.GetConfiguration(_serviceId);
            switch(_serviceType)
            {
                case ServiceType.ListService:
                    var configuration = SerializationHelper.Deserialize<ListServiceConfiguration>(_currentCfg);
                    return configuration.ADCConnectionOptions;
            }
            return null;
        }
        public void SetConfiguration(ListServiceConfiguration listServiceConfiguration)
        {
                var config = new ListServiceConfiguration { ADCConnectionOptions = listServiceConfiguration.ADCConnectionOptions };
                var configString = SerializationHelper.Serialize(config);
                _mgsc.SetConfiguration(_serviceId, configString);
        }
    }
}
