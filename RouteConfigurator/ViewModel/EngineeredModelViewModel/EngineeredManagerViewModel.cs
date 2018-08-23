using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Model.EF_EngineeredModels;
using RouteConfigurator.Services;
using RouteConfigurator.Services.Interface;
using RouteConfigurator.View.EngineeredModelView;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RouteConfigurator.ViewModel.EngineeredModelViewModel
{
    public class EngineeredManagerViewModel : ViewModelBase
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

        private ObservableCollection<EngineeredModification> _newComponents = new ObservableCollection<EngineeredModification>();
        private ObservableCollection<EngineeredModification> _modifiedComponents = new ObservableCollection<EngineeredModification>();
        private ObservableCollection<EngineeredModification> _modifiedEnclosures = new ObservableCollection<EngineeredModification>();
        private ObservableCollection<EngineeredModification> _wireGaugeMods = new ObservableCollection<EngineeredModification>();

        // New component modification table helpers
        private string _NCSenderFilter = "";
        private string _NCNameFilter = "";
        private string _NCEnclosureSizeFilter = "";
        private EngineeredModification _selectedNewComponent;

        // Modified component modification table helpers
        private string _MCSenderFilter = "";
        private string _MCNameFilter = "";
        private string _MCEnclosureSizeFilter = "";
        private EngineeredModification _selectedModifiedComponent;

        // Modified enclosure modification table helpers
        private string _MESenderFilter = "";
        private string _MEEnclosureSizeFilter = "";
        private string _MEEnclosureTypeFilter = "";
        private EngineeredModification _selectedModifiedEnclosure;

        // Wire gauge modification table helpers
        private string _WGSenderFilter = "";
        private string _WGGaugeFilter = "";
        private bool _WGIsNewFilter = false;
        private EngineeredModification _selectedWireGaugeMod;

        private string _informationText;

        private bool _loading = false;
        #endregion

        #region RelayCommands
        public RelayCommand loadedCommand { get; set; }
        public RelayCommand openSupervisorCommand { get; set; }
        public RelayCommand addUserCommand { get; set; }
        public RelayCommand refreshTablesCommand { get; set; }
        public RelayCommand submitCheckedCommand { get; set; }
        public RelayCommand standardOrdersCommand { get; set; }
        public RelayCommand goHomeCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public EngineeredManagerViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            loadedCommand = new RelayCommand(loaded);
            openSupervisorCommand = new RelayCommand(openSupervisorView);
            addUserCommand = new RelayCommand(addUser);
            refreshTablesCommand = new RelayCommand(refreshTables);
            submitCheckedCommand = new RelayCommand(submitCheckedAsync);
            standardOrdersCommand = new RelayCommand(standardOrders);
            goHomeCommand = new RelayCommand(goHome);
        }
        #endregion

        #region Commands
        /// <summary>
        /// Loads all tables
        /// </summary>
        private void loaded()
        {
            updateNewComponentTableAsync();
            updateModifiedComponentTableAsync();
            updateModifiedEnclosureTableAsync();
            updateWireGaugeModTableAsync();
        }

        /// <summary>
        /// Opens the engineered supervisor screen in a new window
        /// </summary>
        private void openSupervisorView()
        {
            MainWindow secondWindow = new MainWindow();
            Page sup = new EngineeredSupervisorView();

            secondWindow.Show();
            secondWindow.Content = sup;
            secondWindow.MinHeight = 700;
            secondWindow.MinWidth = 1400;
        }

        private void addUser()
        {
            _navigationService.NavigateTo("AddUserView");
        }

        /// <summary>
        /// Clears all table filters and reloads the tables
        /// Using the private variables and raisePropertyChanged to 
        /// avoid updating each table multiple times.
        /// </summary>
        private void refreshTables()
        {
            informationText = "";

            //Clear all filters
            _NCSenderFilter = "";
            RaisePropertyChanged("NCSenderFilter");
            _NCNameFilter = "";
            RaisePropertyChanged("NCNameFilter");
            _NCEnclosureSizeFilter = "";
            RaisePropertyChanged("NCEnclosureSizeFilter");

            _MCSenderFilter = "";
            RaisePropertyChanged("MCSenderFilter");
            _MCNameFilter = "";
            RaisePropertyChanged("MCNameFilter");
            _MCEnclosureSizeFilter = "";
            RaisePropertyChanged("MCEnclosureSizeFilter");

            _MESenderFilter = "";
            RaisePropertyChanged("MESenderFilter");
            _MEEnclosureSizeFilter = "";
            RaisePropertyChanged("MEEnclosureSizeFilter");
            _MEEnclosureTypeFilter = "";
            RaisePropertyChanged("MEEnclosureTypeFilter");

            _WGSenderFilter = "";
            RaisePropertyChanged("WGSenderFilter");
            _WGGaugeFilter = "";
            RaisePropertyChanged("WGGaugeFilter");
            _WGIsNewFilter = false;
            RaisePropertyChanged("WGIsNewFilter");

            loaded();
        }

        private async void submitCheckedAsync()
        {
            loading = true;
            await Task.Run(() => submitChecked());
            loading = false;
            refreshTables();
        }

        /// <summary>
        /// Submits the checked modifications to the database and creates or modifies the
        /// information as needed
        /// Calls updateModification
        /// </summary>
        private void submitChecked()
        {
            informationText = "Submitting changes...";

            int numApproved = 0;
            int numDenied = 0;
            int numError = 0;
            string errorText = "";

            //Go through each table
            //check for state to equal 3 (checked to approve) or 4 (checked to decline)
            foreach (EngineeredModification mod in newComponents)
            {
                //If modification is checked to approve
                if (mod.State == 3)
                {
                    mod.ComponentName = mod.ComponentName.ToUpper();
                    mod.EnclosureSize = mod.EnclosureSize.ToUpper();

                    try
                    {
                        //Ensure information is still valid
                        if (string.IsNullOrWhiteSpace(mod.ComponentName))
                        {
                            errorText += string.Format("Error adding component {0}. Invalid component.\n", mod.ComponentName);
                            numError++;
                        }
                        else if (_serviceProxy.getComponent(mod.ComponentName, mod.EnclosureSize) != null)
                        {
                            errorText += string.Format("Error adding component {0} - {1}. Component already exists.\n", mod.ComponentName, mod.EnclosureSize);
                            numError++;
                        }
                        else if (string.IsNullOrWhiteSpace(mod.EnclosureSize) || !(_serviceProxy.getEnclosureSizes().Contains(mod.EnclosureSize)))
                        {
                            errorText += string.Format("Error adding component {0} - {1}. Invalid Enclosure Size.\n", mod.ComponentName, mod.EnclosureSize);
                            numError++;
                        }
                        else if (mod.NewTime < 0)
                        {
                            errorText += string.Format("Error adding component {0}. Invalid Time: {1}.\n.", mod.ComponentName, mod.NewTime);
                            numError++;
                        }
                        else
                        {
                            Component component = new Component()
                            {
                                ComponentName = mod.ComponentName,
                                EnclosureSize = mod.EnclosureSize,
                                Time = mod.NewTime
                            };

                            _serviceProxy.addComponent(component);
                            numApproved++;

                            updateModification(mod);
                        }
                    }
                    catch (Exception e)
                    {
                        errorText += string.Format("Error adding component {0}. Problem accessing the database\n", mod.ComponentName);
                        numError++;

                        Console.WriteLine(e.Message);
                    }
                }
                //If modification is checked to deny
                //Update the modification in the database to save the state to denied
                else if (mod.State == 4)
                {
                    try
                    {
                        updateModification(mod);
                        numDenied++;
                    }
                    catch (Exception e)
                    {
                        informationText = "There was a problem accessing the database.";
                        Console.WriteLine(e.Message);
                    }
                }
            }

            foreach (EngineeredModification mod in modifiedComponents)
            {
                //If modification is checked to approve
                if (mod.State == 3)
                {
                    try
                    {
                        //Ensure information is still valid
                        if (mod.NewTime < 0)
                        {
                            errorText += string.Format("Error modifying component {0}.  Invalid Time: {1}.\n", mod.ComponentName, mod.NewTime);
                            numError++;
                        }
                        else
                        {
                            _serviceProxy.updateComponent(mod.ComponentName, mod.EnclosureSize, mod.NewTime);
                            numApproved++;

                            updateModification(mod);
                        }
                    }
                    catch (Exception e)
                    {
                        errorText += string.Format("Error modifying component {0}.\n", mod.ComponentName);
                        numError++;

                        informationText = "There was a problem accessing the database.";
                        Console.WriteLine(e.Message);
                    }
                }
                //If checked to deny
                else if (mod.State == 4)
                {
                    try
                    {
                        updateModification(mod);
                        numDenied++;
                    }
                    catch (Exception e)
                    {
                        informationText = "There was a problem accessing the database.";
                        Console.WriteLine(e.Message);
                    }
                }
            }

            foreach (EngineeredModification mod in modifiedEnclosures)
            {
                //If checked to approve
                if (mod.State == 3)
                {
                    try
                    {
                        //Ensure information is still valid
                        if (mod.NewTime < 0)
                        {
                            errorText += string.Format("Error modifying enclosure {0} - {1}.  Invalid Time: {2}.\n", mod.EnclosureType, mod.EnclosureSize, mod.NewTime);
                            numError++;
                        }
                        else
                        {
                            _serviceProxy.updateEnclosure(mod.EnclosureType, mod.EnclosureSize, mod.NewTime);
                            numApproved++;

                            updateModification(mod);
                        }
                    }
                    catch (Exception e)
                    {
                        errorText += string.Format("Error modifying enclosure {0} - {1}.\n", mod.EnclosureType, mod.EnclosureSize);
                        numError++;

                        informationText = "There was a problem accessing the database.";
                        Console.WriteLine(e.Message);
                    }
                }
                //If checked to deny
                else if (mod.State == 4)
                {
                    try
                    {
                        updateModification(mod);
                        numDenied++;
                    }
                    catch (Exception e)
                    {
                        informationText = "There was a problem accessing the database.";
                        Console.WriteLine(e.Message);
                    }
                }
            }

            foreach(EngineeredModification mod in wireGaugeMods)
            {
                //Check if the wire gauge modification is adding a new wire gauge or modifying an old wire gauge
                if (mod.IsNew)
                {
                    //If checked to approve
                    if (mod.State == 3)
                    {
                        mod.Gauge = mod.Gauge.ToUpper();

                        try
                        {
                            //Ensure information is still valid
                            if (string.IsNullOrWhiteSpace(mod.Gauge))
                            {
                                errorText += string.Format("Error adding wire gauge {0}. Invalid gauge.\n", mod.Gauge);
                                numError++;
                            }
                            else if (_serviceProxy.getWireGauge(mod.Gauge) != null)
                            {
                                errorText += string.Format("Error adding wire gauge {0}. Gauge already exists.\n", mod.Gauge);
                                numError++;
                            }
                            else if (mod.NewTimePercentage <= 0)
                            {
                                errorText += string.Format("Error adding wire gauge {0}. Invalid Time: {1}.\n", mod.Gauge, mod.NewTimePercentage);
                                numError++;
                            }
                            else
                            {
                                WireGauge wireGauge = new WireGauge()
                                {
                                    Gauge = mod.Gauge,
                                    TimePercentage = mod.NewTimePercentage
                                };

                                _serviceProxy.addWireGauge(wireGauge);
                                numApproved++;

                                updateModification(mod);
                            }
                        }
                        catch (Exception e)
                        {
                            errorText += string.Format("Error adding wire gauge {0}. Problem accessing the database\n", mod.Gauge);
                            numError++;

                            Console.WriteLine(e.Message);
                        }
                    }
                    //If checked to deny
                    else if (mod.State == 4)
                    {
                        try
                        {
                            updateModification(mod);
                            numDenied++;
                        }
                        catch (Exception e)
                        {
                            informationText = "There was a problem accessing the database.";
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                //If the modification is for modyfing a wire gauge
                else
                {
                    //If checked to approve
                    if (mod.State == 3)
                    {
                        try
                        {
                            //Ensure information is still valid
                            if (string.IsNullOrWhiteSpace(mod.Gauge))
                            {
                                errorText += string.Format("Error modifying wire gauge {0}. Invalid gauge.\n", mod.Gauge);
                                numError++;
                            }
                            else if (_serviceProxy.getWireGauge(mod.Gauge) == null)
                            {
                                errorText += string.Format("Error modifying wire gauge {0}. Gauge does not exist.\n", mod.Gauge);
                                numError++;
                            }
                            else if (mod.NewTimePercentage <= 0)
                            {
                                errorText += string.Format("Error modifying wire gauge {0}. Invalid Time: {1}.\n", mod.Gauge, mod.NewTimePercentage);
                                numError++;
                            }
                            else
                            {
                                _serviceProxy.updateWireGauge(mod.Gauge, mod.NewTimePercentage);
                                numApproved++;

                                updateModification(mod);
                            }
                        }
                        catch (Exception e)
                        {
                            errorText += string.Format("Error modifying wire gauge {0}.\n", mod.Gauge);
                            numError++;

                            informationText = "There was a problem accessing the database.";
                            Console.WriteLine(e.Message);
                        }
                    }
                    //If checked to deny
                    else if (mod.State == 4)
                    {
                        try
                        {
                            updateModification(mod);
                            numDenied++;
                        }
                        catch (Exception e)
                        {
                            informationText = "There was a problem accessing the database.";
                            Console.WriteLine(e.Message);
                        }
                    }
                }
            }

            //Display a summary message
            if (numError > 0)
            {
                MessageBox.Show(string.Format("Approved: {0}\n" +
                                              "Denied: {1}\n" +
                                              "Errors: {2}\n" +
                                              "{3}", numApproved, numDenied, numError, errorText));
            }
            else
            {
                MessageBox.Show(string.Format("Approved: {0}\n" +
                                              "Denied: {1}", numApproved, numDenied));
            }

            informationText = "";
        }

        private void standardOrders()
        {
            _navigationService.NavigateTo("ManagerView");
        }

        private void goHome()
        {
            _navigationService.NavigateTo("EngineeredHomeView");
        }
        #endregion

        #region Public Variables
        public ObservableCollection<EngineeredModification> newComponents
        {
            get
            {
                return _newComponents;
            }
            set
            {
                _newComponents = value;
                RaisePropertyChanged("newComponents");
            }
        }

        public ObservableCollection<EngineeredModification> modifiedComponents
        {
            get
            {
                return _modifiedComponents;
            }
            set
            {
                _modifiedComponents = value;
                RaisePropertyChanged("modifiedComponents");
            }
        }

        public ObservableCollection<EngineeredModification> modifiedEnclosures
        {
            get
            {
                return _modifiedEnclosures;
            }
            set
            {
                _modifiedEnclosures = value;
                RaisePropertyChanged("modifiedEnclosures");
            }
        }

        public ObservableCollection<EngineeredModification> wireGaugeMods
        {
            get
            {
                return _wireGaugeMods;
            }
            set
            {
                _wireGaugeMods = value;
                RaisePropertyChanged("wireGaugeMods");
            }
        }

        /// <summary>
        /// Calls updateNewComponentTableAsync
        /// </summary>
        public string NCSenderFilter
        {
            get
            {
                return _NCSenderFilter;
            }
            set
            {
                _NCSenderFilter = value.ToUpper();
                RaisePropertyChanged("NCSenderFilter");
                informationText = "";

                updateNewComponentTableAsync();
            }
        }

        /// <summary>
        /// Calls updateNewComponentTableAsync
        /// </summary>
        public string NCNameFilter
        {
            get
            {
                return _NCNameFilter;
            }
            set
            {
                _NCNameFilter = value.ToUpper();
                RaisePropertyChanged("NCNameFilter");
                informationText = "";

                updateNewComponentTableAsync();
            }
        }

        /// <summary>
        /// Calls updateNewComponentTableAsync
        /// </summary>
        public string NCEnclosureSizeFilter
        {
            get
            {
                return _NCEnclosureSizeFilter;
            }
            set
            {
                _NCEnclosureSizeFilter = value.ToUpper();
                RaisePropertyChanged("NCEnclosureSizeFilter");
                informationText = "";

                updateNewComponentTableAsync();
            }
        }

        public EngineeredModification selectedNewComponent
        {
            get
            {
                return _selectedNewComponent;
            }
            set
            {
                _selectedNewComponent = value;
                RaisePropertyChanged("selectedNewComponent");
                informationText = "";
            }
        }

        /// <summary>
        /// Calls updateModifiedComponentTableAsync
        /// </summary>
        public string MCSenderFilter
        {
            get
            {
                return _MCSenderFilter;
            }
            set
            {
                _MCSenderFilter = value.ToUpper();
                RaisePropertyChanged("MCSenderFilter");
                informationText = "";

                updateModifiedComponentTableAsync();
            }
        }

        /// <summary>
        /// Calls updateModifiedComponentTableAsync
        /// </summary>
        public string MCNameFilter
        {
            get
            {
                return _MCNameFilter;
            }
            set
            {
                _MCNameFilter = value.ToUpper();
                RaisePropertyChanged("MCNameFilter");
                informationText = "";

                updateModifiedComponentTableAsync();
            }
        }

        /// <summary>
        /// Calls updateModifiedComponentTableAsync
        /// </summary>
        public string MCEnclosureSizeFilter
        {
            get
            {
                return _MCEnclosureSizeFilter;
            }
            set
            {
                _MCEnclosureSizeFilter = value.ToUpper();
                RaisePropertyChanged("MCEnclosureSizeFilter");
                informationText = "";

                updateModifiedComponentTableAsync();
            }
        }

        public EngineeredModification selectedModifiedComponent
        {
            get
            {
                return _selectedModifiedComponent;
            }
            set
            {
                _selectedModifiedComponent = value;
                RaisePropertyChanged("selectedModifiedComponent");
                informationText = "";
            }
        }

        /// <summary>
        /// Calls updateModifiedEnclosureTableAsync
        /// </summary>
        public string MESenderFilter
        {
            get
            {
                return _MESenderFilter;
            }
            set
            {
                _MESenderFilter = value.ToUpper();
                RaisePropertyChanged("MESenderFilter");
                informationText = "";

                updateModifiedEnclosureTableAsync();
            }
        }

        /// <summary>
        /// Calls updateModifiedEnclosureTableAsync
        /// </summary>
        public string MEEnclosureSizeFilter
        {
            get
            {
                return _MEEnclosureSizeFilter;
            }
            set
            {
                _MEEnclosureSizeFilter = value.ToUpper();
                RaisePropertyChanged("MEEnclosureSizeFilter");
                informationText = "";

                updateModifiedEnclosureTableAsync();
            }
        }

        /// <summary>
        /// Calls updateModifiedEnclosureTableAsync
        /// </summary>
        public string MEEnclosureTypeFilter
        {
            get
            {
                return _MEEnclosureTypeFilter;
            }
            set
            {
                _MEEnclosureTypeFilter = value.ToUpper();
                RaisePropertyChanged("MEEnclosureTypeFilter");
                informationText = "";

                updateModifiedEnclosureTableAsync();
            }
        }

        public EngineeredModification selectedModifiedEnclosure
        {
            get
            {
                return _selectedModifiedEnclosure;
            }
            set
            {
                _selectedModifiedEnclosure = value;
                RaisePropertyChanged("selectedModifiedEnclosure");
                informationText = "";
            }
        }

        /// <summary>
        /// Calls updateWireGaugeModTableAsync
        /// </summary>
        public string WGSenderFilter
        {
            get
            {
                return _WGSenderFilter;
            }
            set
            {
                _WGSenderFilter = value.ToUpper();
                RaisePropertyChanged("WGSenderFilter");
                informationText = "";

                updateWireGaugeModTableAsync();
            }
        }

        /// <summary>
        /// Calls updateWireGaugeModTableAsync
        /// </summary>
        public string WGGaugeFilter
        {
            get
            {
                return _WGGaugeFilter;
            }
            set
            {
                _WGGaugeFilter = value.ToUpper();
                RaisePropertyChanged("WGGaugeFilter");
                informationText = "";

                updateWireGaugeModTableAsync();
            }
        }

        /// <summary>
        /// Calls updateWireGaugeModTableAsync
        /// </summary>
        public bool WGIsNewFilter
        {
            get
            {
                return _WGIsNewFilter;
            }
            set
            {
                _WGIsNewFilter = value;
                RaisePropertyChanged("WGIsNewFilter");
                informationText = "";

                updateWireGaugeModTableAsync();
            }
        }

        public EngineeredModification selectedWireGaugeMod
        {
            get
            {
                return _selectedWireGaugeMod;
            }
            set
            {
                _selectedWireGaugeMod = value;
                RaisePropertyChanged("selectedWireGaugeMod");
                informationText = "";
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
        private async void updateNewComponentTableAsync()
        {
            loading = true;
            informationText = "Loading tables...";
            await Task.Run(() => updateNewComponentTable());
            loading = false;
            informationText = "";
        }

        /// <summary>
        /// Updates the new component modification table
        /// </summary>
        private void updateNewComponentTable()
        {
            try
            {
                newComponents = new ObservableCollection<EngineeredModification>(_serviceProxy.getFilteredNewComponents(NCSenderFilter, NCNameFilter, NCEnclosureSizeFilter));
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        private async void updateModifiedComponentTableAsync()
        {
            loading = true;
            informationText = "Loading tables...";
            await Task.Run(() => updateModifiedComponentTable());
            loading = false;
            informationText = "";
        }

        /// <summary>
        /// Updates the modified component modification table
        /// </summary>
        private void updateModifiedComponentTable()
        {
            try
            {
                modifiedComponents = new ObservableCollection<EngineeredModification>(_serviceProxy.getFilteredModifiedComponents(MCSenderFilter, MCNameFilter, MCEnclosureSizeFilter));
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        private async void updateModifiedEnclosureTableAsync()
        {
            loading = true;
            informationText = "Loading tables...";
            await Task.Run(() => updateModifiedEnclosureTable());
            loading = false;
            informationText = "";
        }

        /// <summary>
        /// Updates the modified enclosure modification table
        /// </summary>
        private void updateModifiedEnclosureTable()
        {
            try
            {
                modifiedEnclosures = new ObservableCollection<EngineeredModification>(_serviceProxy.getFilteredModifiedEnclosures(MESenderFilter, MEEnclosureSizeFilter, MEEnclosureTypeFilter));
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        private async void updateWireGaugeModTableAsync()
        {
            loading = true;
            informationText = "Loading tables...";
            await Task.Run(() => updateWireGaugeModTable());
            loading = false;
            informationText = "";
        }

        /// <summary>
        /// Updates the wire gauge modification table
        /// </summary>
        private void updateWireGaugeModTable()
        {
            try
            {
                wireGaugeMods = new ObservableCollection<EngineeredModification>(_serviceProxy.getFilteredWireGaugeMods(WGSenderFilter, WGGaugeFilter, WGIsNewFilter));
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Sets the modification review date and reviewer and updates the database with the new information
        /// Should be surrounded by a try catch
        /// </summary>
        /// <param name="mod"></param>
        private void updateModification(EngineeredModification mod)
        {
            mod.ReviewedDate = DateTime.Now;
            mod.Reviewer = string.Format("{0} {1}", _navigationService.user.FirstName, _navigationService.user.LastName);

            _serviceProxy.updateEngineeredModification(mod);
        }
        #endregion
    }
}
