using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Model.EF_EngineeredModels;
using RouteConfigurator.Services;
using RouteConfigurator.Services.Interface;
using RouteConfigurator.View.EngineeredModelView;
using System.Collections.ObjectModel;

namespace RouteConfigurator.ViewModel.EngineeredModelViewModel
{
    public class EngineeredSupervisorViewModel  : ViewModelBase
    {
        #region PrivateVariables
        /// <summary>
        /// Navigation service to help navigate to other pages
        /// </summary>
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Data access service to retrieve data from a data source
        /// </summary>
        private IDataAccessService _serviceProxy = new DataAccessService();

        private bool _goHomeAllowed = false;

        private ObservableCollection<Component> _components = new ObservableCollection<Component>();
        private Component _selectedComponent;

        private string _componentNameFilter = "";
        private string _componentEnclosureSizeFilter = "";

        private ObservableCollection<Enclosure> _enclosures = new ObservableCollection<Enclosure>();
        private Enclosure _selectedEnclosure;

        private string _enclosureTypeFilter = "";
        private string _enclosureSizeFilter = "";

        private ObservableCollection<WireGauge> _wireGauges = new ObservableCollection<WireGauge>();
        private WireGauge _selectedWireGauge;

        private string _wireGaugeFilter = "";

        private string _informationText = "";

        private bool _loading = false;
        #endregion

        #region RelayCommands
        public RelayCommand loadedCommand { get; set; }
        public RelayCommand addComponentCommand { get; set; }
        public RelayCommand modifyComponentsCommand { get; set; }
        public RelayCommand modifyEnclosuresCommand { get; set; }
        public RelayCommand addWireGaugeCommand { get; set; }
        public RelayCommand modifyWireGaugesCommand { get; set; }
        public RelayCommand viewRequestsCommand { get; set; }
        public RelayCommand standardModelsCommand { get; set; }
        public RelayCommand goHomeCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public EngineeredSupervisorViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            if(navigationService.Parameter != null)
            {
                _goHomeAllowed = true;
            }

            loadedCommand = new RelayCommand(loaded);
            addComponentCommand = new RelayCommand(addComponent);
            modifyComponentsCommand = new RelayCommand(modifyComponents);
            modifyEnclosuresCommand = new RelayCommand(modifyEnclosures);
            addWireGaugeCommand = new RelayCommand(addWireGauge);
            modifyWireGaugesCommand = new RelayCommand(modifyWireGauges);
            viewRequestsCommand = new RelayCommand(viewRequests);
            standardModelsCommand = new RelayCommand(standardModels);
            goHomeCommand = new RelayCommand(goHome);
        }
        #endregion

        #region Commands
        private void loaded()
        {
            updateComponentTable();
            updateEnclosureTable();
            updateWireGaugeTable();
        }

        private void addComponent()
        {
            AddComponentPopup addComponent = new AddComponentPopup();
            addComponent.Show();
        }

        private void modifyComponents()
        {
            ModifyComponentsPopup modifyComponents = new ModifyComponentsPopup();
            modifyComponents.Show();
        }

        private void modifyEnclosures()
        {
            ModifyEnclosuresPopup modifyEnclosures = new ModifyEnclosuresPopup();
            modifyEnclosures.Show();
        }

        private void addWireGauge()
        {
            AddWireGaugePopup addWireGauge = new AddWireGaugePopup();
            addWireGauge.Show();
        }

        private void modifyWireGauges()
        {
            ModifyWireGaugesPopup modifyWireGauges = new ModifyWireGaugesPopup();
            modifyWireGauges.Show();
        }
        
        private void viewRequests()
        {
            EngineeredRequestsView requests = new EngineeredRequestsView();
            requests.Show();
        }

        private void standardModels()
        {
            _navigationService.NavigateTo("SupervisorView", goHomeAllowed);
        }

        private void goHome()
        {
            _navigationService.NavigateTo("EngineeredHomeView");
        }
        #endregion

        #region Public Variables
        public bool goHomeAllowed
        {
            get
            {
                return _goHomeAllowed;
            }
            set
            {
                _goHomeAllowed = value;
                RaisePropertyChanged("goHomeAllowed");
            }
        }

        public ObservableCollection<Component> components
        {
            get
            {
                return _components;
            }
            set
            {
                _components = value;
                RaisePropertyChanged("components");
            }
        }

        public Component selectedComponent
        {
            get
            {
                return _selectedComponent;
            }
            set
            {
                _selectedComponent = value;
                RaisePropertyChanged("selectedComponent");
            }
        }

        /// <summary>
        /// Calls updateComponentTable
        /// </summary>
        public string componentNameFilter
        {
            get
            {
                return _componentNameFilter;
            }
            set
            {
                _componentNameFilter = value.ToUpper();
                RaisePropertyChanged("componentNameFilter");

                updateComponentTable();
            }
        }

        /// <summary>
        /// Calls updateComponentTable
        /// </summary>
        public string componentEnclosureSizeFilter
        {
            get
            {
                return _componentEnclosureSizeFilter;
            }
            set
            {
                _componentEnclosureSizeFilter = value.ToUpper();
                RaisePropertyChanged("componentEnclosureSizeFilter");

                updateComponentTable();
            }
        }

        public ObservableCollection<Enclosure> enclosures
        {
            get
            {
                return _enclosures;
            }
            set
            {
                _enclosures = value;
                RaisePropertyChanged("enclosures");
            }
        }

        public Enclosure selectedEnclosure
        {
            get
            {
                return _selectedEnclosure;
            }
            set
            {
                _selectedEnclosure = value;
                RaisePropertyChanged("selectedEnclosure");
            }
        }

        /// <summary>
        /// Calls updateEnclosureTable
        /// </summary>
        public string enclosureTypeFilter
        {
            get
            {
                return _enclosureTypeFilter;
            }
            set
            {
                _enclosureTypeFilter = value.ToUpper();
                RaisePropertyChanged("enclosureTypeFilter");

                updateEnclosureTable();
            }
        }

        /// <summary>
        /// Calls updateEnclosureTable
        /// </summary>
        public string enclosureSizeFilter
        {
            get
            {
                return _enclosureSizeFilter;
            }
            set
            {
                _enclosureSizeFilter = value.ToUpper();
                RaisePropertyChanged("enclosureSizeFilter");

                updateEnclosureTable();
            }
        }

        public ObservableCollection<WireGauge> wireGauges
        {
            get
            {
                return _wireGauges;
            }
            set
            {
                _wireGauges = value;
                RaisePropertyChanged("wireGauges");
            }
        }

        public WireGauge selectedWireGauge
        {
            get
            {
                return _selectedWireGauge;
            }
            set
            {
                _selectedWireGauge = value;
                RaisePropertyChanged("selectedWireGauge");
            }
        }

        /// <summary>
        /// Calls updateWireGaugeTable
        /// </summary>
        public string wireGaugeFilter
        {
            get
            {
                return _wireGaugeFilter;
            }
            set
            {
                _wireGaugeFilter = value.ToUpper();
                RaisePropertyChanged("wireGaugeFilter");

                updateWireGaugeTable();
            }
        }

        public string informationText 
        {
            get
            {
                return _informationText;
            }
            set
            {
                _informationText = value;
                RaisePropertyChanged("informationText");
            }
        }

        public bool loading
        {
            get
            {
                return _loading;
            }
            set
            {
                _loading = value;
                RaisePropertyChanged("loading");
            }
        }
        #endregion

        #region Private Functions
        private void updateComponentTable()
        {
            components = new ObservableCollection<Component>(_serviceProxy.getFilteredComponents(componentNameFilter, componentEnclosureSizeFilter));
        }

        private void updateEnclosureTable()
        {
            enclosures = new ObservableCollection<Enclosure>(_serviceProxy.getFilteredEnclosures(enclosureTypeFilter, enclosureSizeFilter));
        }

        private void updateWireGaugeTable()
        {
            wireGauges = new ObservableCollection<WireGauge>(_serviceProxy.getFilteredWireGauges(wireGaugeFilter));
        }
        #endregion
    }
}
