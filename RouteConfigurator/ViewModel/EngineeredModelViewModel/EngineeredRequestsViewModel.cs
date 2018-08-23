using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Model.EF_EngineeredModels;
using RouteConfigurator.Services;
using RouteConfigurator.Services.Interface;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RouteConfigurator.ViewModel.EngineeredModelViewModel
{
    public class EngineeredRequestsViewModel : ViewModelBase
    {
        #region PrivateVariables
        /// <summary>
        /// Navigation service to help navigate to other pages
        /// </summary>
        private readonly IFrameNavigationService _navigationService;

        /// <summary>
        /// Data access service to retrieve data from a data source
        /// </summary>
        private IDataAccessService _serviceProxy = new DataAccessService();

        private ObservableCollection<EngineeredModification> _modifications = new ObservableCollection<EngineeredModification>();

        private EngineeredModification _selectedModification;

        private string _StateFilter = "";
        private string _ComponentNameFilter = "";
        private string _EnclosureTypeFilter = "";
        private string _EnclosureSizeFilter = "";
        private string _WireGaugeFilter = "";
        private string _SenderFilter = "";
        private string _ReviewerFilter = "";

        private string _informationText;

        private bool _loading = false;
        #endregion

        #region RelayCommands
        public RelayCommand loadedCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public EngineeredRequestsViewModel (IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            loadedCommand = new RelayCommand(loaded);
        }
        #endregion

        #region Commands
        /// <summary>
        /// Calls updateModificationsTableAsync
        /// </summary>
        private void loaded()
        {
            updateModificationsTableAsync();
        }
        #endregion

        #region Public Variables
        public ObservableCollection<EngineeredModification> modifications
        {
            get { return _modifications; }
            set
            {
                _modifications = value;
                RaisePropertyChanged("modifications");
            }
        }

        public EngineeredModification selectedModification
        {
            get { return _selectedModification; }
            set
            {
                _selectedModification = value;
                RaisePropertyChanged("selectedModification");
                informationText = "";
            }
        }

        /// <summary>
        /// Calls updateModificationsTableAsync
        /// </summary>
        public string StateFilter
        {
            get { return _StateFilter; }
            set
            {
                _StateFilter = value.ToUpper();
                RaisePropertyChanged("StateFilter");
                informationText = "";

                updateModificationsTableAsync();
            }
        }

        /// <summary>
        /// Calls updateModificationsTableAsync
        /// </summary>
        public string ComponentNameFilter
        {
            get { return _ComponentNameFilter; }
            set
            {
                _ComponentNameFilter = value.ToUpper();
                RaisePropertyChanged("ComponentNameFilter");
                informationText = "";

                updateModificationsTableAsync();
            }
        }
        
        /// <summary>
        /// Calls updateModificationsTableAsync
        /// </summary>
        public string EnclosureSizeFilter
        {
            get { return _EnclosureSizeFilter; }
            set
            {
                _EnclosureSizeFilter = value.ToUpper();
                RaisePropertyChanged("EnclosureSizeFilter");
                informationText = "";

                updateModificationsTableAsync();
            }
        }

        /// <summary>
        /// Calls updateModificationsTableAsync
        /// </summary>
        public string EnclosureTypeFilter
        {
            get { return _EnclosureTypeFilter; }
            set
            {
                _EnclosureTypeFilter = value.ToUpper();
                RaisePropertyChanged("EnclosureTypeFilter");
                informationText = "";

                updateModificationsTableAsync();
            }
        }

        /// <summary>
        /// Calls updateModificationsTableAsync
        /// </summary>
        public string WireGaugeFilter
        {
            get { return _WireGaugeFilter; }
            set
            {
                _WireGaugeFilter = value.ToUpper();
                RaisePropertyChanged("WireGaugeFilter");
                informationText = "";

                updateModificationsTableAsync();
            }
        }

        /// <summary>
        /// Calls updateModificationsTableAsync
        /// </summary>
        public string SenderFilter
        {
            get { return _SenderFilter; }
            set
            {
                _SenderFilter = value.ToUpper();
                RaisePropertyChanged("SenderFilter");
                informationText = "";

                updateModificationsTableAsync();
            }
        }

        /// <summary>
        /// Calls updateModificationsTableAsync
        /// </summary>
        public string ReviewerFilter
        {
            get { return _ReviewerFilter; }
            set
            {
                _ReviewerFilter = value.ToUpper();
                RaisePropertyChanged("ReviewerFilter");
                informationText = "";

                updateModificationsTableAsync();
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
        private async void updateModificationsTableAsync()
        {
            loading = true;
            informationText = "Loading table...";
            await Task.Run(() => updateModificationsTable());
            loading = false;
            informationText = "";
        }

        /// <summary>
        /// Updates the modifications with the entered filters
        /// Calls getStateFilter
        /// </summary>
        private void updateModificationsTable()
        {
            int stateFilter = getStateFilter(StateFilter);

            try
            {
                if (stateFilter == -1)
                {
                    modifications = new ObservableCollection<EngineeredModification>(
                        _serviceProxy.getFilteredEngineeredModifications(ComponentNameFilter, EnclosureSizeFilter, EnclosureTypeFilter, WireGaugeFilter, SenderFilter, ReviewerFilter));
                }
                else if (stateFilter == 0)
                {
                    modifications = new ObservableCollection<EngineeredModification>(
                        _serviceProxy.getFilteredWaitingEngineeredModifications(ComponentNameFilter, EnclosureSizeFilter, EnclosureTypeFilter, WireGaugeFilter, SenderFilter, ReviewerFilter));
                }
                else
                {
                    modifications = new ObservableCollection<EngineeredModification>(
                        _serviceProxy.getFilteredStateEngineeredModifications(stateFilter, ComponentNameFilter, EnclosureSizeFilter, EnclosureTypeFilter, WireGaugeFilter, SenderFilter, ReviewerFilter));
                }
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Associates an integer with the entered state filter
        /// </summary>
        /// <param name="stateText"> the entered state filter </param>
        /// <returns> an integer corresponding to the state filter </returns>
        private int getStateFilter(string stateText)
        {
            int stateFilter = -1;
            if (string.IsNullOrWhiteSpace(stateText))
                return stateFilter;

            switch (stateText.ElementAt(0))
            {
                case ('W'): //Waiting
                    {
                        stateFilter = 0; //Also includes 3 and 4
                        break;
                    }
                case ('A'): //Approved
                    {
                        stateFilter = 1;
                        break;
                    }
                case ('D'): //Declined
                    {
                        stateFilter = 2;
                        break;
                    }
                default:
                    {
                        stateFilter = -1;
                        break;
                    }
            }

            return stateFilter;
        }

        #endregion
    }
}
