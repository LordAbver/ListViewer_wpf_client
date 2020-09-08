using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ListViewer_wpf_client.Configuration;
using ListViewer_wpf_client.Helpers;
using ListViewer_wpf_client.ManagerService;

namespace ListViewer_wpf_client.ViewModels
{
    /// <summary>
    /// Interaction logic for ListServiceConfig.xaml
    /// </summary>
    public partial class ListServiceConfig : Window
    {
        private readonly ConfigurationHelper _configurationHelper;
        public ListServiceConfig()
        {
            _configurationHelper=new ConfigurationHelper(ServiceType.ListService);
            InitializeComponent();
            GetCurrentSettings();
        }
        private void GetCurrentSettings()
        {
                var configuration = _configurationHelper.GetCurrentConfiguration();
                _appName.Text = configuration.ApplicationName;
                _ds.Text = configuration.DeviceServersToInitialize[0];
                _connect.IsChecked = configuration.DeviceServersToConnect.Contains(_ds.Text);    
        }
        private void Save(object sender, RoutedEventArgs e)
        {
            var configuration = new ListServiceConfiguration
                                    {
                                        ADCConnectionOptions = new ADCConnectionParameters()
                                                                   {
                                                                       ApplicationName = _appName.Text,
                                                                       DeviceServersToConnect = new[]{_ds.Text},
                                                                       DeviceServersToInitialize = _connect.IsChecked.GetValueOrDefault() ? new[] { _ds.Text } : new string[0]
                                                                   }
                                    };
            try
            {
                _configurationHelper.SetConfiguration(configuration);
                MessageBox.Show("Configuration saved", "Configuration", MessageBoxButton.OK,
                                        MessageBoxImage.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK,
                                       MessageBoxImage.Error);
            }
        }
    }
}
