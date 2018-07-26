using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Design;
using RouteConfigurator.Model;
using RouteConfigurator.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RouteConfigurator.ViewModel
{
    public class SupervisorViewModel : ViewModelBase
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

        // Model Table
        private static ObservableCollection<Model.Model> _models;
        private Model.Model _selectedModel;
        private string _modelFilter = "";
        private string _boxSizeFilter = "";

        // Options Table
        private static ObservableCollection<Option> _options;
        private string _optionFilter = "";
        private string _optionBoxSizeFilter = "";

        // Time Trial Table
        private bool _TTVisible = false;
        private ObservableCollection<TimeTrial> _timeTrials;
        private TimeTrial _selectedTimeTrial;
        private bool _optionTextChecked = false;
        private string _optionTextFilter = "";
        private string _salesFilter = "";
        private string _productionNumFilter = "";
        private decimal _averageProdTime;
        private decimal _averageDriveTime;
        private decimal _averageAVTime;

        // Override Table
        private static ObservableCollection<Override> _overrides;
        private Override _selectedOverride;
        private string _overrideFilter = "";

        private string _informationText = "";
        #endregion

        #region RelayCommands
        public RelayCommand loadModelsCommand { get; set; }
        public RelayCommand addModelCommand { get; set; }
        public RelayCommand modifyModelCommand { get; set; }

        public RelayCommand addTimeTrialCommand { get; set; }
        public RelayCommand deleteTTCommand { get; set; }

        public RelayCommand loadOptionsCommand { get; set; }
        public RelayCommand addOptionCommand { get; set; }
        public RelayCommand modifyOptionCommand { get; set; }

        public RelayCommand loadOverridesCommand { get; set; }
        public RelayCommand overrideModelCommand { get; set; }
        public RelayCommand deleteOverrideCommand { get; set; }

        public RelayCommand goHomeCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public SupervisorViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            if(navigationService.Parameter != null)
            {
                goHomeAllowed = true;
            }

            loadModelsCommand = new RelayCommand(updateModelTable);
            addModelCommand = new RelayCommand(addModel);
            modifyModelCommand = new RelayCommand(modifyModel);

            addTimeTrialCommand = new RelayCommand(addTimeTrial);
            deleteTTCommand = new RelayCommand(deleteTT);

            loadOptionsCommand = new RelayCommand(loadOptions);
            addOptionCommand = new RelayCommand(addOption);
            modifyOptionCommand = new RelayCommand(modifyOption);

            loadOverridesCommand = new RelayCommand(loadOverrides);
            overrideModelCommand = new RelayCommand(overrideModel);
            deleteOverrideCommand = new RelayCommand(deleteOverride);

            goHomeCommand = new RelayCommand(goHome);
        }
        #endregion

        #region Commands
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

        private void loadModels()
        {
            updateModelTable();
        }

        private void addModel()
        {
            AddModelPopup addModel = new AddModelPopup();
            addModel.Show();
        }

        private void modifyModel()
        {
            ModifyModelPopup modifyModel = new ModifyModelPopup();
            modifyModel.Show();
        }

        private void addTimeTrial()
        {
            AddTimeTrialPopup addTimeTrial = new AddTimeTrialPopup();
            addTimeTrial.Show();
        }

        private void deleteTT()
        {
            if (selectedTimeTrial != null)
            {
                MessageBox.Show(
                    string.Format("Placeholder for deleting: Time Trial {0} {1} \n" +
                    "Not implemented yet", selectedTimeTrial.Model.Base, selectedTimeTrial.ProductionNumber));
            }
        }

        private void loadOptions()
        {
            updateOptionTable();
        }

        private void addOption()
        {
            AddOptionPopup addOption = new AddOptionPopup();
            addOption.Show();
        }

        private void modifyOption()
        {
            ModifyOptionPopup modifyOption = new ModifyOptionPopup();
            modifyOption.Show();
        }

        private void loadOverrides()
        {
            updateOverrideTable();
        }

        private void overrideModel()
        {
            OverrideModelPopup overrideModel = new OverrideModelPopup();
            overrideModel.Show();
        }

        private void deleteOverride()
        {
            MessageBox.Show(string.Format("Placeholder for deleting override for {0}", selectedOverride.ModelNum));
            //Send to director for approval
        }

        private void goHome()
        {
            _navigationService.NavigateTo("HomeView");
        }

        #endregion

        #region Public Variables
        public ObservableCollection<Model.Model> models
        {
            get
            {
                return _models;
            }
            set
            {
                _models = value;
                RaisePropertyChanged("models");
            }
        }

        /// <summary>
        /// Sets TTVisible based on if model is null
        /// Updates time trials list
        /// </summary>
        public Model.Model selectedModel
        {
            get
            {
                return _selectedModel;
            }
            set
            {
                _selectedModel = value;
                RaisePropertyChanged("selectedModel");

                // Reset Time Trial Filters
                if(!string.IsNullOrWhiteSpace(optionTextFilter))
                    optionTextFilter = "";
                if(!string.IsNullOrWhiteSpace(salesFilter))
                    salesFilter = "";
                if(!string.IsNullOrWhiteSpace(productionNumFilter))
                    productionNumFilter = "";

                if (value != null)
                {
                    TTVisible = true;
                    timeTrials = new ObservableCollection<TimeTrial>(_serviceProxy.getTimeTrials(selectedModel.Base));

                    calcTTAverages();
                }
                else
                {
                    TTVisible = false;
                    timeTrials.Clear();
                }
            }
        }

        /// <summary>
        /// Calls updateFilter
        /// </summary>
        public string modelFilter
        {
            get
            {
                return _modelFilter;
            }
            set
            {
                _modelFilter = value.ToUpper();
                RaisePropertyChanged("modelFilter");
                informationText = "";
                updateModelTable();
            }
        }

        /// <summary>
        /// Calls updateFilter
        /// </summary>
        public string boxSizeFilter
        {
            get
            {
                return _boxSizeFilter;
            }
            set
            {
                _boxSizeFilter = value.ToUpper();
                RaisePropertyChanged("boxSizeFilter");
                informationText = "";
                updateModelTable();
            }
        }

        public ObservableCollection<Option> options 
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value;
                RaisePropertyChanged("options");
            }
        }

        /// <summary>
        /// Calls updateOptionFilter
        /// </summary>
        public string optionFilter
        {
            get
            {
                return _optionFilter;
            }
            set
            {
                _optionFilter = value.ToUpper();
                RaisePropertyChanged("optionFilter");
                informationText = "";
                updateOptionTable();
            }
        }

        /// <summary>
        /// Calls updateOptionFilter
        /// </summary>
        public string optionBoxSizeFilter
        {
            get
            {
                return _optionBoxSizeFilter;
            }
            set
            {
                _optionBoxSizeFilter = value.ToUpper();
                RaisePropertyChanged("optionBoxSizeFilter");
                informationText = "";
                updateOptionTable();
            }
        }

        public bool TTVisible
        {
            get
            {
                return _TTVisible;
            }
            set
            {
                _TTVisible = value;
                RaisePropertyChanged("TTVisible");
            }
        }

        public ObservableCollection<TimeTrial> timeTrials
        {
            get
            {
                return _timeTrials;
            }
            set
            {
                _timeTrials = value;
                RaisePropertyChanged("timeTrials");
            }
        }

        public TimeTrial selectedTimeTrial
        {
            get
            {
                return _selectedTimeTrial;
            }
            set
            {
                _selectedTimeTrial = value;
                RaisePropertyChanged("selectedTimeTrial");
            }
        }

        /// <summary>
        /// Calls updateTTFilter
        /// </summary>
        public bool optionTextChecked
        {
            get
            {
                return _optionTextChecked;
            }
            set
            {
                _optionTextChecked = value;
                RaisePropertyChanged("optionTextChecked");
                informationText = "";
                updateTTTable();
            }
        }

        /// <summary>
        /// Calls updateTTFilter
        /// </summary>
        public string optionTextFilter
        {
            get
            {
                return _optionTextFilter;
            }
            set
            {
                _optionTextFilter = value.ToUpper();
                RaisePropertyChanged("optionTextFilter");
                informationText = "";
                updateTTTable();
            }
        }

        /// <summary>
        /// Calls updateTTFilter
        /// </summary>
        public string salesFilter
        {
            get
            {
                return _salesFilter;
            }
            set
            {
                _salesFilter = value;
                RaisePropertyChanged("salesFilter");
                informationText = "";
                updateTTTable();
            }
        }

        /// <summary>
        /// Calls updateTTFilter
        /// </summary>
        public string productionNumFilter
        {
            get
            {
                return _productionNumFilter;
            }
            set
            {
                _productionNumFilter = value;
                RaisePropertyChanged("productionNumFilter");
                informationText = "";
                updateTTTable();
            }
        }

        public decimal averageProdTime
        {
            get
            {
                return _averageProdTime;
            }
            set
            {
                _averageProdTime = value;
                RaisePropertyChanged("averageProdTime");
            }
        }

        public decimal averageDriveTime
        {
            get
            {
                return _averageDriveTime;
            }
            set
            {
                _averageDriveTime = value;
                RaisePropertyChanged("averageDriveTime");
            }
        }

        public decimal averageAVTime
        {
            get
            {
                return _averageAVTime;
            }
            set
            {
                _averageAVTime = value;
                RaisePropertyChanged("averageAVTime");
            }
        }

        public ObservableCollection<Override> overrides
        {
            get
            {
                return _overrides;
            }
            set
            {
                _overrides = value;
                RaisePropertyChanged("overrides");
            }
        }

        public Override selectedOverride
        {
            get
            {
                return _selectedOverride;
            }
            set
            {
                _selectedOverride = value;
                RaisePropertyChanged("selectedOverride");
            }
        }

        /// <summary>
        /// Updates active overrides list
        /// </summary>
        public string overrideFilter
        {
            get
            {
                return _overrideFilter;
            }
            set
            {
                _overrideFilter = value.ToUpper();
                RaisePropertyChanged("overrideFilter");
                informationText = "";

                updateOverrideTable();
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
        #endregion

        #region Private Functions
        /// <summary>
        /// Updates the model list to only show models with the specified filters
        /// </summary>
        private void updateModelTable()
        {
            models = new ObservableCollection<Model.Model>(_serviceProxy.getFilteredModels(modelFilter, boxSizeFilter));
        }

        /// <summary>
        /// Updates the options list to only show options with the specified filters
        /// </summary>
        private void updateOptionTable()
        {
            options = new ObservableCollection<Option>(_serviceProxy.getFilteredOptions(optionFilter, optionBoxSizeFilter));
        }

        /// <summary>
        /// Updates the time trials list to only show time trials with the specified filters
        /// Calls calcTTAverages
        /// </summary>
        private void updateTTTable()
        {
            if (selectedModel != null)
            {
                if (optionTextChecked)
                {
                    timeTrials = new ObservableCollection<TimeTrial>(_serviceProxy.getStrictFilteredTimeTrials(selectedModel.Base, optionTextFilter, salesFilter, productionNumFilter));
                }
                else
                {
                    timeTrials = new ObservableCollection<TimeTrial>(_serviceProxy.getFilteredTimeTrials(selectedModel.Base, optionTextFilter, salesFilter, productionNumFilter));
                }
                calcTTAverages();
            }
        }

        private void calcTTAverages()
        {
            averageProdTime = 0;
            averageDriveTime = 0;
            averageAVTime = 0;

            decimal count = 0;

            foreach(TimeTrial timeTrial in timeTrials)
            {
                averageProdTime += timeTrial.TotalTime;
                averageDriveTime += timeTrial.DriveTime;
                averageAVTime += timeTrial.AVTime;
                count++;
            }

            if (count != 0)
            {
                averageProdTime = averageProdTime / count;
                averageDriveTime = averageDriveTime / count;
                averageAVTime = averageAVTime / count;
            }
        }

        private void updateOverrideTable()
        {
            overrides =new ObservableCollection<Override>(_serviceProxy.getFilteredOverrides(overrideFilter));
        }
        #endregion
    }
}
