using System.ComponentModel;
using System.Windows;

namespace ListViewer_wpf_client.ViewModels
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Log
    {
        private bool _clearListServiceContent;
        private bool _clearDeviceServiceContent;
        private bool _clearOperationLogContent;
        private bool _clearAsRunLogContent;

        public bool IsNeedToClearListServiceContent
        {
            get
            {
               return  _clearListServiceContent;
            }
            set { _clearListServiceContent = value; }
        }
        public bool IsNeedToClearAsRunServiceContent
        {
            get
            {
                return _clearAsRunLogContent;
            }
            set { _clearAsRunLogContent = value; }
        }
        public bool IsNeedToClearDeviceServiceContent
        {
            get
            {
                return _clearDeviceServiceContent;
            }
            set { _clearDeviceServiceContent = value; }
        }
        public bool IsNeedToClearOperationLogContent
        {
            get
            {
                return _clearOperationLogContent;
            }
        }
        public Log()
        {
            InitializeComponent();
        }

        private void ClearListServiceContentClick(object sender, RoutedEventArgs e)
        {
            _clearListServiceContent = true;
            ListServiceTextBox.Text = "";            
        }

        private void ClearOperationLogContentClick(object sender, RoutedEventArgs e)
        {
            OperationLoggerTextBox.Text = "";
            _clearOperationLogContent = true;

        }

        private void ClearDeviceServiceLogButtonClick(object sender, RoutedEventArgs e)
        {
            DeviceServiceListenerTextBox.Text = "";
            _clearDeviceServiceContent = true;
        }

        private void ClearAsRunServiceLogButtonClick(object sender, RoutedEventArgs e)
        {
            AsRunServiceTextBox.Text = "";
            _clearAsRunLogContent = true;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            e.Cancel = true;
            //MessageBox.Show("Why do you want to close the log window??!!", "This is not a bug!",MessageBoxButton.OK ,MessageBoxImage.Warning);
        }
    }
}
