
namespace ListViewer_wpf_client.Configuration
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://Harris/Automation/ADC/Services")]
    public partial class ADCConnectionParameters : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string _applicationNameField;

        private string[] _deviceServersToInitializeField;

        private string[] _deviceServersToConnectField;

        public ADCConnectionParameters()
        {
            this._applicationNameField = "ADCService";
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "normalizedString")]
        public string ApplicationName
        {
            get
            {
                return this._applicationNameField;
            }
            set
            {
                this._applicationNameField = value;
                this.RaisePropertyChanged("ApplicationName");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("DeviceServerName", DataType = "normalizedString", IsNullable = false)]
        public string[] DeviceServersToInitialize
        {
            get
            {
                return this._deviceServersToInitializeField;
            }
            set
            {
                this._deviceServersToInitializeField = value;
                this.RaisePropertyChanged("DeviceServersToInitialize");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("DeviceServerName", DataType = "normalizedString", IsNullable = false)]
        public string[] DeviceServersToConnect
        {
            get
            {
                return this._deviceServersToConnectField;
            }
            set
            {
                this._deviceServersToConnectField = value;
                this.RaisePropertyChanged("DeviceServersToConnect");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://Harris/Automation/ADC/Services/ListService")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://Harris/Automation/ADC/Services/ListService", IsNullable = false)]
    public partial class ListServiceConfiguration : object, System.ComponentModel.INotifyPropertyChanged
    {

        private ADCConnectionParameters _aDcConnectionOptionsField;

        /// <remarks/>
        public ADCConnectionParameters ADCConnectionOptions
        {
            get
            {
                return this._aDcConnectionOptionsField;
            }
            set
            {
                this._aDcConnectionOptionsField = value;
                this.RaisePropertyChanged("ADCConnectionOptions");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
