﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Model.EF_StandardModels;
using RouteConfigurator.Services;
using RouteConfigurator.Services.Interface;
using RouteConfigurator.View.StandardModelView;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace RouteConfigurator.ViewModel.StandardModelViewModel
{
    public class SupervisorViewModel : ViewModelBase
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

        /// <summary>
        /// Boolean to determine if the go home button is visible and enabled or not
        /// True means button is enabled and visible, false means button is disabled and collapsed
        /// Button needs to be disabled when the user opens the supervisor screen in a new window,
        /// because the go home button of the new window changes the old screen
        /// </summary>
        private bool _goHomeAllowed = false;

        // Model Table list and filters
        private static ObservableCollection<StandardModel> _models;
        private StandardModel _selectedModel;
        private string _modelFilter = "";
        private ObservableCollection<string> _boxSizes;
        private string _boxSizeFilter = "";

        // Options Table list and filters
        private static ObservableCollection<Option> _options;
        private string _optionFilter = "";
        private ObservableCollection<string> _optionBoxSizes;
        private string _optionBoxSizeFilter = "";

        // Time Trial Table list, filters, and helpers
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

        // Override Table list and filters
        private static ObservableCollection<Override> _overrides;
        private Override _selectedOverride;
        private string _overrideFilter = "";

        private string _informationText = "";

        private bool _loading = false;
        #endregion

        #region RelayCommands
        public RelayCommand loadedCommand { get; set; }

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

            loadedCommand = new RelayCommand(loaded);

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
        private async void loaded()
        {
            loading = true;
            await Task.Run(() => getInformation());
            loading = false;
        }

        /// <summary>
        /// Loads the information, box sizes, needed for the page
        /// </summary>
        private void getInformation()
        {
            try
            {
                informationText = "Loading Information...";
                boxSizes = new ObservableCollection<string>(_serviceProxy.getModelBoxSizes());
                optionBoxSizes = new ObservableCollection<string>(_serviceProxy.getOptionBoxSizes());
                informationText = "";
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Calls updateModelTableAsync
        /// </summary>
        private void loadModels()
        {
            informationText = "";
            updateModelTableAsync();
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

        /// <summary>
        /// Deletes the time trial from the database
        /// </summary>
        /// <returns> returns if the time trial was successfully deleted </returns>
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

        /// <summary>
        /// Calls updateOverrideTableAsync
        /// </summary>
        private void loadOverrides()
        {
            informationText = "";
            updateOverrideTableAsync();
        }

        /// <summary>
        /// Sends a model modification to the manager for deleting the override 
        /// </summary>
        private void deleteOverride()
        {
            informationText = "";

            try
            {
                if (selectedOverride != null)
                {
                    //Check if override is waiting to be deleted.
                    //If there is an override already waiting to be deleted it will have the
                    //same description as the new modification request
                    Modification mod = new Modification()
                    {
                        RequestDate = DateTime.Now,
                        ReviewDate = new DateTime(1900, 1, 1),
                        Description = string.Format("Deleting override for {0}.  Override Time was: {1}.  " +
                        "Override Route was: {2}", selectedOverride.ModelNum, selectedOverride.OverrideTime, selectedOverride.OverrideRoute),
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

        private void overrideModel()
        {
            OverrideModelPopup overrideModel = new OverrideModelPopup();
            overrideModel.Show();
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

        public ObservableCollection<StandardModel> models
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
        public StandardModel selectedModel
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

        public ObservableCollection<string> boxSizes
        {
            get
            {
                return _boxSizes;
            }
            set
            {
                _boxSizes = value;
                RaisePropertyChanged("boxSizes");
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
                _boxSizeFilter = value == null ? value : value.ToUpper();
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

        public ObservableCollection<string> optionBoxSizes
        {
            get
            {
                return _optionBoxSizes;
            }
            set
            {
                _optionBoxSizes = value;
                RaisePropertyChanged("optionBoxSizes");
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
                _optionBoxSizeFilter = value == null ? value : value.ToUpper();
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
                if (string.IsNullOrWhiteSpace(boxSizeFilter))
                {
                    models = new ObservableCollection<StandardModel>(_serviceProxy.getFilteredModels(modelFilter, "", false));
                }
                else
                {
                    models = new ObservableCollection<StandardModel>(_serviceProxy.getFilteredModels(modelFilter, boxSizeFilter, true));
                }
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
                if (string.IsNullOrWhiteSpace(optionBoxSizeFilter))
                {
                    options = new ObservableCollection<Option>(_serviceProxy.getFilteredOptions(optionFilter, "", false));
                }
                else
                {
                    options = new ObservableCollection<Option>(_serviceProxy.getFilteredOptions(optionFilter, optionBoxSizeFilter, true));
                }
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
            if (selectedModel != null)
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
        }

        /// <summary>
        /// Calculates the average production time of the shown time trials
        /// </summary>
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
                if(!(timeTrial.DriveTime == 0 && timeTrial.AVTime == 0))
                {
                    count++;
                }
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

        /// <summary>
        /// Updates the overrides list to only show overrides with the specified filters
        /// </summary>
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
