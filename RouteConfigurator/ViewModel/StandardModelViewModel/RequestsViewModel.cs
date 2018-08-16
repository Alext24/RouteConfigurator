using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Model.EF_StandardModels;
using RouteConfigurator.Services;
using RouteConfigurator.Services.Interface;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RouteConfigurator.ViewModel.StandardModelViewModel
{
    public class RequestsViewModel : ViewModelBase
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

        private ObservableCollection<Modification> _modifications = new ObservableCollection<Modification>();

        private Modification _selectedModification;

        private string _MStateFilter = "";
        private string _MBaseFilter = "";
        private string _MBoxSizeFilter = "";
        private string _MOptionCodeFilter = "";
        private string _MSenderFilter = "";
        private string _MReviewerFilter = "";

        private ObservableCollection<OverrideRequest> _overrides = new ObservableCollection<OverrideRequest>();
        private OverrideRequest _selectedOverride;

        private string _ORStateFilter = "";
        private string _ORModelNameFilter = "";
        private string _ORSenderFilter = "";
        private string _ORReviewerFilter = "";

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
        public RequestsViewModel (IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            loadedCommand = new RelayCommand(loaded);
        }
        #endregion

        #region Commands
        private void loaded()
        {
            updateModificationsTableAsync();
            updateOverridesTableAsync();
        }
        #endregion

        #region Public Variables
        public ObservableCollection<Modification> modifications
        {
            get { return _modifications; }
            set
            {
                _modifications = value;
                RaisePropertyChanged("modifications");
            }
        }

        public Modification selectedModification
        {
            get { return _selectedModification; }
            set
            {
                _selectedModification = value;
                RaisePropertyChanged("selectedModification");
            }
        }

        /// <summary>
        /// Calls updateModificationsTableAsync
        /// </summary>
        public string MStateFilter
        {
            get { return _MStateFilter; }
            set
            {
                _MStateFilter = value.ToUpper();
                RaisePropertyChanged("MStateFilter");
                informationText = "";

                updateModificationsTableAsync();
            }
        }

        /// <summary>
        /// Calls updateModificationsTableAsync
        /// </summary>
        public string MBaseFilter
        {
            get { return _MBaseFilter; }
            set
            {
                _MBaseFilter = value.ToUpper();
                RaisePropertyChanged("MBaseFilter");
                informationText = "";

                updateModificationsTableAsync();
            }
        }
        
        /// <summary>
        /// Calls updateModificationsTableAsync
        /// </summary>
        public string MBoxSizeFilter
        {
            get { return _MBoxSizeFilter; }
            set
            {
                _MBoxSizeFilter = value.ToUpper();
                RaisePropertyChanged("MBoxSizeFilter");
                informationText = "";

                updateModificationsTableAsync();
            }
        }

        /// <summary>
        /// Calls updateModificationsTableAsync
        /// </summary>
        public string MOptionCodeFilter
        {
            get { return _MOptionCodeFilter; }
            set
            {
                _MOptionCodeFilter = value.ToUpper();
                RaisePropertyChanged("MOptionCodeFilter");
                informationText = "";

                updateModificationsTableAsync();
            }
        }

        /// <summary>
        /// Calls updateModificationsTableAsync
        /// </summary>
        public string MSenderFilter
        {
            get { return _MSenderFilter; }
            set
            {
                _MSenderFilter = value.ToUpper();
                RaisePropertyChanged("MSenderFilter");
                informationText = "";

                updateModificationsTableAsync();
            }
        }

        /// <summary>
        /// Calls updateModificationsTableAsync
        /// </summary>
        public string MReviewerFilter
        {
            get { return _MReviewerFilter; }
            set
            {
                _MReviewerFilter = value.ToUpper();
                RaisePropertyChanged("MReviewerFilter");
                informationText = "";

                updateModificationsTableAsync();
            }
        }

        public ObservableCollection<OverrideRequest> overrides 
        {
            get { return _overrides; }
            set
            {
                _overrides = value;
                RaisePropertyChanged("overrides");
            }
        }

        public OverrideRequest selectedOverride
        {
            get { return _selectedOverride; }
            set
            {
                _selectedOverride = value;
                RaisePropertyChanged("selectedOverride");
            }
        }

        /// <summary>
        /// Calls updateOverridesTableAsync
        /// </summary>
        public string ORStateFilter
        {
            get { return _ORStateFilter; }
            set
            {
                _ORStateFilter = value.ToUpper();
                RaisePropertyChanged("ORStateFilter");
                informationText = "";

                updateOverridesTableAsync();
            }
        }

        /// <summary>
        /// Calls updateOverridesTableAsync
        /// </summary>
        public string ORModelNameFilter
        {
            get { return _ORModelNameFilter; }
            set
            {
                _ORModelNameFilter = value.ToUpper();
                RaisePropertyChanged("ORModelNameFilter");
                informationText = "";

                updateOverridesTableAsync();
            }
        }

        /// <summary>
        /// Calls updateOverridesTableAsync
        /// </summary>
        public string ORSenderFilter
        {
            get { return _ORSenderFilter; }
            set
            {
                _ORSenderFilter = value.ToUpper();
                RaisePropertyChanged("ORSenderFilter");
                informationText = "";

                updateOverridesTableAsync();
            }
        }

        /// <summary>
        /// Calls updateOverridesTableAsync
        /// </summary>
        public string ORReviewerFilter
        {
            get { return _ORReviewerFilter; }
            set
            {
                _ORReviewerFilter = value.ToUpper();
                RaisePropertyChanged("ORReviewerFilter");
                informationText = "";

                updateOverridesTableAsync();
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
            informationText = "Loading tables...";
            await Task.Run(() => updateModificationsTable());
            loading = false;
            informationText = "";
        }

        private void updateModificationsTable()
        {
            int stateFilter = getStateFilter(MStateFilter);

            try
            {
                if (stateFilter == -1)
                {
                    modifications = new ObservableCollection<Modification>(
                        _serviceProxy.getFilteredModifications(MBaseFilter, MBoxSizeFilter, MOptionCodeFilter, MSenderFilter, MReviewerFilter));
                }
                else if (stateFilter == 0)
                {
                    modifications = new ObservableCollection<Modification>(
                        _serviceProxy.getFilteredWaitingModifications(MBaseFilter, MBoxSizeFilter, MOptionCodeFilter, MSenderFilter, MReviewerFilter));
                }
                else
                {
                    modifications = new ObservableCollection<Modification>(
                        _serviceProxy.getFilteredStateModifications(stateFilter, MBaseFilter, MBoxSizeFilter, MOptionCodeFilter, MSenderFilter, MReviewerFilter));
                }
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        private async void updateOverridesTableAsync()
        {
            loading = true;
            informationText = "Loading tables...";
            await Task.Run(() => updateOverridesTable());
            loading = false;
            informationText = "";
        }

        private void updateOverridesTable()
        {
            int stateFilter = getStateFilter(ORStateFilter);

            try
            {
                overrides = new ObservableCollection<OverrideRequest>(
                    _serviceProxy.getFilteredOverrideRequests(stateFilter, ORModelNameFilter, ORSenderFilter, ORReviewerFilter));
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

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
