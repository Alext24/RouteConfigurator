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

        private bool _loading = false;
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

        public RelayCommand viewRequestsCommand { get; set; }
        public RelayCommand engineeredModelsCommand { get; set; }
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
                _goHomeAllowed = true;
            }

            loadModelsCommand = new RelayCommand(loadModels);
            addModelCommand = new RelayCommand(addModel);
            modifyModelCommand = new RelayCommand(modifyModel);

            addTimeTrialCommand = new RelayCommand(addTimeTrial);
            deleteTTCommand = new RelayCommand(deleteTTAsyncCall);

            loadOptionsCommand = new RelayCommand(loadOptions);
            addOptionCommand = new RelayCommand(addOption);
            modifyOptionCommand = new RelayCommand(modifyOption);

            loadOverridesCommand = new RelayCommand(loadOverrides);
            overrideModelCommand = new RelayCommand(overrideModel);
            deleteOverrideCommand = new RelayCommand(deleteOverride);

            viewRequestsCommand = new RelayCommand(viewRequests);
            engineeredModelsCommand = new RelayCommand(engineeredModels);
            goHomeCommand = new RelayCommand(goHome);
        }
        #endregion

        #region Commands
        /// <summary>
        /// Calls updateModelTableAsync
        /// </summary>
        private void loadModels()
        {
            informationText = "";
            updateModelTableAsync();
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

        private async void deleteTTAsyncCall()
        {
            loading = true;
            if (await Task.Run(() => deleteTT()))
            {
                await Task.Run(() => updateTTTable());
            }
            loading = false;
        }

        private bool deleteTT()
        {
            bool deleted = false;
            if (selectedTimeTrial != null)
            {
                informationText = "";
                if(MessageBox.Show(string.Format("Are you sure you would like to delete\n" +
                                "Time Trial {0}{1} {2} \n", selectedTimeTrial.Model.Base, selectedTimeTrial.OptionsText, selectedTimeTrial.ProductionNumber), 
                                "Delete Time Trial", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                {
                    try
                    {
                        informationText = "Deleting Time Trial...";
                        _serviceProxy.deleteTimeTrial(selectedTimeTrial);
                        deleted = true;
                        informationText = "";
                    }
                    catch (Exception e)
                    {
                        informationText = "There was a problem accessing the database";
                        Console.WriteLine(e);
                    }
                }
            }
            return deleted;
        }

        /// <summary>
        /// Calls updateOptionTableAsync
        /// </summary>
        private void loadOptions()
        {
            informationText = "";
            updateOptionTableAsync();
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

        /// <summary>
        /// Calls updateOverrideTableAsync
        /// </summary>
        private void loadOverrides()
        {
            informationText = "";
            updateOverrideTableAsync();
        }

        private void overrideModel()
        {
            OverrideModelPopup overrideModel = new OverrideModelPopup();
            overrideModel.Show();
        }

        private void deleteOverride()
        {
            informationText = "";

            try
            {
                if (selectedOverride != null)
                {
                    //Check if override is waiting to be deleted.
                    Modification mod = new Modification()
                    {
                        RequestDate = DateTime.Now,
                        ReviewDate = new DateTime(1900, 1, 1),
                        Description = string.Format("Deleting override for {0}.  Override Time was: {1}.  Override Route was: {2}", selectedOverride.ModelNum, selectedOverride.OverrideTime, selectedOverride.OverrideRoute),
                        State = 0,
                        Sender = "TEMPORARY SENDER",
                        Reviewer = "",
                        IsOption = false,
                        IsNew = false,
                        BoxSize = selectedOverride.Model.BoxSize,
                        ModelBase = selectedOverride.Model.Base,
                        NewDriveTime = selectedOverride.Model.DriveTime,
                        NewAVTime = selectedOverride.Model.AVTime,
                        OldModelDriveTime = 0,
                        OldModelAVTime = 0,
                        OptionCode = "",
                        NewTime = 0,
                        NewName = "",
                        OldOptionTime = 0,
                        OldOptionName = ""
                    };

                    if (_serviceProxy.checkDuplicateOverrideDeletion(mod))
                    {
                        informationText = "Override deletion already waiting for manager approval.";
                    }
                    else
                    {
                        _serviceProxy.addModificationRequest(mod);
                        informationText = "Override deletion sent to manager for approval.";
                    }
                }
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        private void viewRequests()
        {
            RequestsView requests = new RequestsView();
            requests.Show();
        }

        private void engineeredModels()
        {
            _navigationService.NavigateTo("EngineeredSupervisorView", goHomeAllowed);
        }

        private void goHome()
        {
            _navigationService.NavigateTo("HomeView");
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
        /// Calls updateTTTableAsync 
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

                if (value != null)
                {
                    TTVisible = true;
                    updateTTTableAsync();
                }
                else
                {
                    TTVisible = false;
                    timeTrials.Clear();

                    averageProdTime = 0;
                    averageDriveTime = 0;
                    averageAVTime = 0;
                }
            }
        }

        /// <summary>
        /// Calls updateModelTableAsync
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

                updateModelTableAsync();
            }
        }

        /// <summary>
        /// Calls updateModelTableAsync
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

                updateModelTableAsync();
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
        /// Calls updateOptionTableAsync
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

                updateOptionTableAsync();
            }
        }

        /// <summary>
        /// Calls updateOptionTableAsync
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

                updateOptionTableAsync();
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
        /// Calls updateTTTableAsync
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

                updateTTTableAsync();
            }
        }

        /// <summary>
        /// Calls updateTTTableAsync
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

                updateTTTableAsync();
            }
        }

        /// <summary>
        /// Calls updateTTTableAsync
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

                updateTTTableAsync();
            }
        }

        /// <summary>
        /// Calls updateTTTableAsync
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

                updateTTTableAsync();
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
        /// Calls updateOverrideTableAsync 
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

                updateOverrideTableAsync();
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
        private async void updateModelTableAsync()
        {
            loading = true;
            await Task.Run(() => updateModelTable());
            loading = false;
        }

        /// <summary>
        /// Updates the model list to only show models with the specified filters
        /// </summary>
        private void updateModelTable()
        {
            try
            {
                informationText = "Retrieving models...";
                models = new ObservableCollection<Model.Model>(_serviceProxy.getFilteredModels(modelFilter, boxSizeFilter));
                informationText = "";
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        private async void updateOptionTableAsync()
        {
            loading = true;
            await Task.Run(() => updateOptionTable());
            loading = false;
        }

        /// <summary>
        /// Updates the options list to only show options with the specified filters
        /// </summary>
        private void updateOptionTable()
        {
            try
            {
                informationText = "Retrieving options...";
                options = new ObservableCollection<Option>(_serviceProxy.getFilteredOptions(optionFilter, optionBoxSizeFilter, false));
                informationText = "";
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        private async void updateTTTableAsync()
        {
            loading = true;
            await Task.Run(() => updateTTTable());
            loading = false;
        }

        /// <summary>
        /// Updates the time trials list to only show time trials with the specified filters
        /// Calls calcTTAverages
        /// </summary>
        private void updateTTTable()
        {
            try
            {
                informationText = "Retrieving time trials...";
                timeTrials = new ObservableCollection<TimeTrial>(_serviceProxy.getFilteredTimeTrials(selectedModel.Base, optionTextFilter, salesFilter, productionNumFilter, optionTextChecked));
                calcTTAverages();
                informationText = "";
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
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

        private async void updateOverrideTableAsync()
        {
            loading = true;
            await Task.Run(() => updateOverrideTable());
            loading = false;
        }

        private void updateOverrideTable()
        {
            try
            {
                informationText = "Retrieving overrides...";
                overrides = new ObservableCollection<Override>(_serviceProxy.getFilteredOverrides(overrideFilter));
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
