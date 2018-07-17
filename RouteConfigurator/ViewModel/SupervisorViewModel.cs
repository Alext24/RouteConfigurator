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
        private string _optionTextFilter = "";
        private string _salesFilter = "";
        private string _productionNumFilter = "";

        // Override Table
        private static ObservableCollection<Override> _overrides;
        private Override _selectedOverride;
        private string _overrideFilter = "";

        private string _informationText = "";
        #endregion

        #region RelayCommands
        public RelayCommand addModelCommand { get; set; }
        public RelayCommand addOptionCommand { get; set; }
        public RelayCommand addTimeTrialCommand { get; set; }
        public RelayCommand loadModelsCommand { get; set; }
        public RelayCommand loadOptionsCommand { get; set; }
        public RelayCommand loadOverridesCommand { get; set; }
        public RelayCommand modifyModelCommand { get; set; }
        public RelayCommand overrideModelCommand { get; set; }
        public RelayCommand deleteTTCommand { get; set; }
        public RelayCommand deleteOverrideCommand { get; set; }
        public RelayCommand goBackCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public SupervisorViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            addModelCommand = new RelayCommand(addModel);
            addOptionCommand = new RelayCommand(addOption);
            addTimeTrialCommand = new RelayCommand(addTimeTrial);
            loadModelsCommand = new RelayCommand(loadModels);
            loadOptionsCommand = new RelayCommand(loadOptions);
            loadOverridesCommand = new RelayCommand(loadOverrides);
            modifyModelCommand = new RelayCommand(modifyModel);
            overrideModelCommand = new RelayCommand(overrideModel);
            deleteTTCommand = new RelayCommand(deleteTT);
            deleteOverrideCommand = new RelayCommand(deleteOverride);
            goBackCommand = new RelayCommand(goBack);
        }
        #endregion

        #region Commands
        private void addModel()
        {
            AddModelPopup addModel = new AddModelPopup();
            addModel.Show();
        }

        private void addOption()
        {
            AddOptionPopup addOption = new AddOptionPopup();
            addOption.Show();
        }

        private void addTimeTrial()
        {
            AddTimeTrialPopup addTimeTrial = new AddTimeTrialPopup();
            addTimeTrial.Show();
        }

        private void loadModels()
        {
            models = _serviceProxy.getFilteredModels(modelFilter, boxSizeFilter);
        }

        private void loadOptions()
        {
            options = _serviceProxy.getFilteredOptions(optionFilter, optionBoxSizeFilter);
        }

        private void loadOverrides()
        {
            overrides = _serviceProxy.getFilteredOverrides(overrideFilter);
        }

        private void modifyModel()
        {
            ModifyModelPopup modifyModel = new ModifyModelPopup();
            modifyModel.Show();
        }

        private void overrideModel()
        {
            OverrideModelPopup overrideModel = new OverrideModelPopup();
            overrideModel.Show();
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

        private void deleteOverride()
        {
            MessageBox.Show(string.Format("Placeholder for deleting override for {0}", selectedOverride.ModelNum));
        }

        private void goBack()
        {
            _navigationService.GoBack();
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
                optionTextFilter = "";
                salesFilter = "";
                productionNumFilter = "";

                if (value != null)
                {
                    TTVisible = true;
                    timeTrials = _serviceProxy.getTimeTrials(selectedModel.Base);
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
                updateFilter();
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
                updateFilter();
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
                updateOptionFilter();
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
                updateOptionFilter();
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
                updateTTFilter();
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
                updateTTFilter();
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
                updateTTFilter();
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

                updateOverrideFilter();
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
        private void updateFilter()
        {
            models = _serviceProxy.getFilteredModels(modelFilter, boxSizeFilter);
        }

        /// <summary>
        /// Updates the options list to only show options with the specified filters
        /// </summary>
        private void updateOptionFilter()
        {
            options = _serviceProxy.getFilteredOptions(optionFilter, optionBoxSizeFilter);
        }

        /// <summary>
        /// Updates the time trials list to only show time trials with the specified filters
        /// </summary>
        private void updateTTFilter()
        {
            if (selectedModel != null)
            {
                timeTrials = _serviceProxy.getFilteredTimeTrials(selectedModel.Base, optionTextFilter, salesFilter, productionNumFilter);
            }
        }

        private void updateOverrideFilter()
        {
            overrides = _serviceProxy.getFilteredOverrides(overrideFilter);
        }
        #endregion
    }
}
