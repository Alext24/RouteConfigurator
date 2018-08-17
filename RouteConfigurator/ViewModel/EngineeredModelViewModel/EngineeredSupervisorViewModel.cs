using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Model.EF_EngineeredModels;
using RouteConfigurator.Services;
using RouteConfigurator.Services.Interface;
using RouteConfigurator.View.EngineeredModelView;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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

        /// <summary>
        /// Boolean to determine if the go home button is visible and enabled or not
        /// True means button is enabled and visible, false means button is disabled and collapsed
        /// Button needs to be disabled when the user opens the supervisor screen in a new window,
        /// because the go home button of the new window changes the old screen
        /// </summary>
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
            updateComponentTableAsync();
            updateEnclosureTableAsync();
            updateWireGaugeTableAsync();
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
                informationText = "";
            }
        }

        /// <summary>
        /// Calls updateComponentTableAsync
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
                informationText = "";

                updateComponentTableAsync();
            }
        }

        /// <summary>
        /// Calls updateComponentTableAsync
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
                informationText = "";

                updateComponentTableAsync();
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
                informationText = "";
            }
        }

        /// <summary>
        /// Calls updateEnclosureTableAsync
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
                informationText = "";

                updateEnclosureTableAsync();
            }
        }

        /// <summary>
        /// Calls updateEnclosureTableAsync
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
                informationText = "";

                updateEnclosureTableAsync();
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
                informationText = "";
            }
        }

        /// <summary>
        /// Calls updateWireGaugeTableAsync
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
                informationText = "";

                updateWireGaugeTableAsync();
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
        private async void updateComponentTableAsync()
        {
            loading = true;
            await Task.Run(() => updateComponentTable());
            loading = false;
        }

        private void updateComponentTable()
        {
            try
            {
                informationText = "Retrieving components...";
                components = new ObservableCollection<Component>(_serviceProxy.getFilteredComponents(componentNameFilter, componentEnclosureSizeFilter));
                informationText = "";
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        private async void updateEnclosureTableAsync()
        {
            loading = true;
            await Task.Run(() => updateEnclosureTable());
            loading = false;
        }

        private void updateEnclosureTable()
        {
            try
            {
                informationText = "Retrieving enclosures...";
                enclosures = new ObservableCollection<Enclosure>(_serviceProxy.getFilteredEnclosures(enclosureTypeFilter, enclosureSizeFilter));
                informationText = "";
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        private async void updateWireGaugeTableAsync()
        {
            loading = true;
            await Task.Run(() => updateWireGaugeTable());
            loading = false;
        }

        private void updateWireGaugeTable()
        {
            try
            {
                informationText = "Retrieving wire gauges...";
                wireGauges = new ObservableCollection<WireGauge>(_serviceProxy.getFilteredWireGauges(wireGaugeFilter));
                informationText = "";
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }
        #endregion
    }
}
