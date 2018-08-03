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
using System.Windows.Controls;

namespace RouteConfigurator.ViewModel
{
    public class ManagerViewModel : ViewModelBase
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

        private ObservableCollection<Modification> _newModels = new ObservableCollection<Modification>();
        private ObservableCollection<Modification> _newOptions = new ObservableCollection<Modification>();
        private ObservableCollection<Modification> _modifiedModels = new ObservableCollection<Modification>();
        private ObservableCollection<Modification> _modifiedOptions = new ObservableCollection<Modification>();
        private ObservableCollection<OverrideRequest> _overrides = new ObservableCollection<OverrideRequest>();

        private string _NMSenderFilter = "";
        private string _NMBaseFilter = "";
        private string _NMBoxSizeFilter = "";
        private Modification _selectedNewModel;

        private string _NOSenderFilter = "";
        private string _NOOptionCodeFilter = "";
        private string _NOBoxSizeFilter = "";
        private Modification _selectedNewOption;

        private string _MMSenderFilter = "";
        private string _MMModelNameFilter = "";
        private Modification _selectedModifiedModel;

        private string _OMSenderFilter = "";
        private string _OMOptionCodeFilter = "";
        private string _OMBoxSizeFilter = "";
        private Modification _selectedModifiedOption;

        private string _ORSenderFilter = "";
        private string _ORModelNameFilter = "";
        private OverrideRequest _selectedOverride;

        private string _informationText;

        private bool _loading = false;
        #endregion

        #region RelayCommands
        public RelayCommand loadedCommand { get; set; }
        public RelayCommand openSupervisorCommand { get; set; }
        public RelayCommand refreshTablesCommand { get; set; }
        public RelayCommand submitCheckedCommand { get; set; }
        public RelayCommand goBackCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public ManagerViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            loadedCommand = new RelayCommand(loaded);
            openSupervisorCommand = new RelayCommand(openSupervisorView);
            refreshTablesCommand = new RelayCommand(refreshTables);
            submitCheckedCommand = new RelayCommand(submitCheckedAsync);
            goBackCommand = new RelayCommand(goBack);
        }
        #endregion

        #region Commands
        private void loaded()
        {
            updateNewModelTableAsync();
            updateNewOptionTableAsync();
            updateModelModificationTableAsync();
            updateOptionModificationTableAsync();
            updateOverrideTableAsync();
        }

        private void openSupervisorView()
        {
            MainWindow secondWindow = new MainWindow();
            Page sup = new SupervisorView();

            secondWindow.Show();
            secondWindow.Content = sup;
            secondWindow.MinHeight = 700;
            secondWindow.MinWidth = 1400;
        }

        private void refreshTables()
        {
            informationText = "";

            //Clear all filters
            _NMBaseFilter = "";
            RaisePropertyChanged("NMBaseFilter");
            _NMBoxSizeFilter = "";
            RaisePropertyChanged("NMBoxSizeFilter");
            _NMSenderFilter = "";
            RaisePropertyChanged("NMSenderFilter");

            _NOBoxSizeFilter = "";
            RaisePropertyChanged("NOBoxSizeFilter");
            _NOOptionCodeFilter = "";
            RaisePropertyChanged("NOOptionCodeFilter");
            _NOSenderFilter = "";
            RaisePropertyChanged("NOSenderFilter");

            _MMModelNameFilter = "";
            RaisePropertyChanged("MMModelNameFilter");
            _MMSenderFilter = "";
            RaisePropertyChanged("MMSenderFilter");

            _OMBoxSizeFilter = "";
            RaisePropertyChanged("OMBoxSizeFilter");
            _OMOptionCodeFilter = "";
            RaisePropertyChanged("OMOptionCodeFilter");
            _OMSenderFilter = "";
            RaisePropertyChanged("OMSenderFilter");

            _ORModelNameFilter = "";
            RaisePropertyChanged("ORModelNameFilter");
            _ORSenderFilter = "";
            RaisePropertyChanged("ORSenderFilter");

            loaded();
        }
        
        private async void submitCheckedAsync()
        {
            loading = true;
            await Task.Run(() => submitChecked());
            loading = false;
            refreshTables();
        }

        private void submitChecked()
        {
            informationText = "Submitting changes...";
            int numApproved = 0;
            int numDenied = 0;
            int numError = 0;
            string errorText = "";

            //Go through each table
            //check for state to equal 3 (approved) or 4 (declined)
            foreach (Modification mod in newModels)
            {
                if (mod.State == 3)
                {
                    mod.ModelBase = mod.ModelBase.ToUpper();
                    mod.BoxSize = mod.BoxSize.ToUpper();

                    try
                    {
                        if (mod.ModelBase.Length != 8)
                        {
                            errorText += string.Format("Error adding Model {0}. Invalid Model format.", mod.ModelBase);
                            numError++;
                        }
                        else if (_serviceProxy.getModel(mod.ModelBase) != null)
                        {
                            errorText += string.Format("Error adding Model {0}. Model already exists.", mod.ModelBase);
                            numError++;
                        }
                        else if (string.IsNullOrWhiteSpace(mod.BoxSize))
                        {
                            errorText += string.Format("Error adding Model {0}. Invalid Box Size.", mod.ModelBase);
                            numError++;
                        }
                        else if (mod.NewDriveTime <= 0)
                        {
                            errorText += string.Format("Error adding Model {0}. Invalid Drive Time: {1}.", mod.ModelBase, mod.NewDriveTime);
                            numError++;
                        }
                        else if (mod.NewAVTime <= 0)
                        {
                            errorText += string.Format("Error adding Model {0}. Invalid AV Time: {1}.", mod.ModelBase, mod.NewAVTime);
                            numError++;
                        }
                        else
                        {
                            Model.Model model = new Model.Model()
                            {
                                Base = mod.ModelBase,
                                BoxSize = mod.BoxSize,
                                DriveTime = mod.NewDriveTime,
                                AVTime = mod.NewAVTime
                            };

                            _serviceProxy.addModel(model);
                            numApproved++;

                            updateModification(mod);
                        }
                    }
                    catch (Exception e)
                    {
                        errorText += string.Format("Error adding model {0}. Problem accessing the database\n", mod.ModelBase);
                        numError++;

                        Console.WriteLine(e.Message);
                    }
                }
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

            foreach (Modification mod in newOptions)
            {
                if (mod.State == 3)
                {
                    mod.OptionCode = mod.OptionCode.ToUpper();
                    mod.BoxSize = mod.BoxSize.ToUpper();

                    try
                    {
                        if (mod.OptionCode.Length != 2)
                        {
                            errorText += string.Format("Error adding Option {0}. Option Code must be 2 characters.", mod.OptionCode);
                            numError++;
                        }
                        else if (!mod.OptionCode.ElementAt(0).Equals('P') && !mod.OptionCode.ElementAt(0).Equals('T'))
                        {
                            errorText += string.Format("Error adding Option {0}. Option Code must start with P or T.", mod.OptionCode);
                            numError++;
                        }
                        else if (mod.OptionCode.ElementAt(1).Equals('P') || mod.OptionCode.ElementAt(1).Equals('T'))
                        {
                            errorText += string.Format("Error adding Option {0}. Option Code must not end with P or T.", mod.OptionCode);
                            numError++;
                        }
                        else if (string.IsNullOrWhiteSpace(mod.BoxSize))
                        {
                            errorText += string.Format("Error adding Option {0}. Invalid Box Size.", mod.OptionCode);
                            numError++;
                        }
                        else if (_serviceProxy.getFilteredOptions(mod.OptionCode, mod.BoxSize, true).ToList().Count > 0)
                        {
                            errorText += string.Format("Error adding Option {0}. Option already exists.", mod.OptionCode);
                            numError++;
                        }
                        else if (mod.NewTime <= 0)
                        {
                            errorText += string.Format("Error adding Option {0}. Invalid Time: {1}.", mod.OptionCode, mod.NewTime);
                            numError++;
                        }
                        else
                        {
                            Option option = new Option()
                            {
                                OptionCode = mod.OptionCode,
                                BoxSize = mod.BoxSize,
                                Time = mod.NewTime,
                                Name = mod.NewName
                            };

                            _serviceProxy.addOption(option);
                            numApproved++;

                            updateModification(mod);
                        }
                    }
                    catch (Exception e)
                    {
                        errorText += string.Format("Error adding option {0}-{1}\n", mod.OptionCode, mod.BoxSize);
                        numError++;

                        informationText = "There was a problem accessing the database.";
                        Console.WriteLine(e.Message);
                    }
                }
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

            foreach (Modification mod in modifiedModels)
            {
                if (mod.State == 3)
                {
                    if (mod.NewDriveTime <= 0)
                    {
                        errorText += string.Format("Error modifying model {0}.  Invalid Drive Time: {1}", mod.ModelBase, mod.NewDriveTime);
                        numError++;
                    }
                    else if (mod.NewAVTime <= 0)
                    {
                        errorText += string.Format("Error modifying model {0}.  Invalid AV Time: {1}", mod.ModelBase, mod.NewAVTime);
                        numError++;
                    }
                    else
                    {
                        try
                        {
                            string descriptionStart = "Deleting override for ";
                            if (mod.Description.StartsWith(descriptionStart))
                            {
                                string modelNum = mod.Description.Remove(0, descriptionStart.Length);
                                modelNum = modelNum.Remove(modelNum.IndexOf("."));
                                _serviceProxy.deleteOverride(modelNum);
                                numApproved++;
                            }
                            else
                            {
                                _serviceProxy.updateModel(mod.ModelBase, mod.NewDriveTime, mod.NewAVTime);
                                numApproved++;
                            }

                            updateModification(mod);
                        }
                        catch (Exception e)
                        {
                            errorText += string.Format("Error modifying model {0}\n", mod.ModelBase);
                            numError++;

                            informationText = "There was a problem accessing the database.";
                            Console.WriteLine(e.Message);
                        }
                    }
                }
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

            foreach (Modification mod in modifiedOptions)
            {
                if (mod.State == 3)
                {
                    if (mod.NewTime <= 0)
                    {
                        errorText += string.Format("Error modifying option {0}.  Invalid time: {1}", mod.OptionCode, mod.NewTime);
                        numError++;
                    }
                    else
                    {
                        try
                        {
                            _serviceProxy.updateOption(mod.OptionCode, mod.BoxSize, mod.NewTime, mod.NewName);
                            numApproved++;

                            updateModification(mod);
                        }
                        catch (Exception e)
                        {
                            errorText += string.Format("Error modifying option {0}-{1}\n", mod.OptionCode, mod.BoxSize);
                            numError++;

                            informationText = "There was a problem accessing the database.";
                            Console.WriteLine(e.Message);
                        }
                    }
                }
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

            foreach (OverrideRequest or in overrides)
            {
                if (or.State == 3)
                {
                    or.ModelNum = or.ModelNum.ToUpper();

                    try
                    {
                        if (or.ModelNum.Length < 8)
                        {
                            errorText += string.Format("Error adding override {0}.  Invalid Model Number format.", or.ModelNum);
                            numError++;
                        }
                        else if (_serviceProxy.getModel(or.ModelNum.Substring(0, 8)) == null)
                        {
                            errorText += string.Format("Error adding override {0}.  Invalid Model: {1}.", or.ModelNum, or.ModelBase);
                            numError++;
                        }
                        else if (or.OverrideTime <= 0)
                        {
                            errorText += string.Format("Error adding override {0}.  Invalid Override Time: {1}.", or.ModelNum, or.OverrideTime);
                            numError++;
                        }
                        else if (or.OverrideRoute <= 0)
                        {
                            errorText += string.Format("Error adding override {0}.  Invalid Override Route: {1}.", or.ModelNum, or.OverrideRoute);
                            numError++;
                        }
                        else
                        {
                            Override ov = new Override()
                            {
                                ModelNum = or.ModelNum,
                                OverrideRoute = or.OverrideRoute,
                                OverrideTime = or.OverrideTime
                            };
                            _serviceProxy.addOverride(ov, or.ModelBase);
                            numApproved++;

                            updateOverride(or);
                        }
                    }
                    catch (Exception e)
                    {
                        errorText += string.Format("Error adding override {0}\n", or.ModelNum);
                        numError++;

                        informationText = "There was a problem accessing the database.";
                        Console.WriteLine(e.Message);
                    }
                }
                else if (or.State == 4)
                {
                    try
                    {
                        updateOverride(or);
                        numDenied++;
                    }
                    catch (Exception e)
                    {
                        informationText = "There was a problem accessing the database.";
                        Console.WriteLine(e.Message);
                    }
                }
            }

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

        private void goBack()
        {
            _navigationService.GoBack();
        }
        #endregion

        #region Public Variables
        public ObservableCollection<Modification> newModels
        {
            get
            {
                return _newModels;
            }
            set
            {
                _newModels = value;
                RaisePropertyChanged("newModels");
            }
        }

        public ObservableCollection<Modification> newOptions
        {
            get
            {
                return _newOptions;
            }
            set
            {
                _newOptions = value;
                RaisePropertyChanged("newOptions");
            }
        }

        public ObservableCollection<Modification> modifiedModels
        {
            get
            {
                return _modifiedModels;
            }
            set
            {
                _modifiedModels = value;
                RaisePropertyChanged("modifiedModels");
            }
        }

        public ObservableCollection<Modification> modifiedOptions
        {
            get
            {
                return _modifiedOptions;
            }
            set
            {
                _modifiedOptions = value;
                RaisePropertyChanged("modifiedOptions");
            }
        }

        public ObservableCollection<OverrideRequest> overrides
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

        /// <summary>
        /// Calls updateNewModelTableAsync
        /// </summary>
        public string NMSenderFilter
        {
            get
            {
                return _NMSenderFilter;
            }
            set
            {
                _NMSenderFilter = value.ToUpper();
                RaisePropertyChanged("NMSenderFilter");
                informationText = "";

                updateNewModelTableAsync();
            }
        }

        /// <summary>
        /// Calls updateNewModelTableAsync
        /// </summary>
        public string NMBaseFilter
        {
            get
            {
                return _NMBaseFilter;
            }
            set
            {
                _NMBaseFilter = value.ToUpper();
                RaisePropertyChanged("NMBaseFilter");
                informationText = "";

                updateNewModelTableAsync();
            }
        }

        /// <summary>
        /// Calls updateNewModelTableAsync
        /// </summary>
        public string NMBoxSizeFilter
        {
            get
            {
                return _NMBoxSizeFilter;
            }
            set
            {
                _NMBoxSizeFilter = value.ToUpper();
                RaisePropertyChanged("NMBoxSizeFilter");
                informationText = "";

                updateNewModelTableAsync();
            }
        }

        public Modification selectedNewModel
        {
            get
            {
                return _selectedNewModel;
            }
            set
            {
                _selectedNewModel = value;
                RaisePropertyChanged("selectedNewModel");
            }
        }

        /// <summary>
        /// Calls updateNewOptionTableAsync
        /// </summary>
        public string NOSenderFilter
        {
            get
            {
                return _NOSenderFilter;
            }
            set
            {
                _NOSenderFilter = value.ToUpper();
                RaisePropertyChanged("NOSenderFilter");
                informationText = "";

                updateNewOptionTableAsync();
            }
        }

        /// <summary>
        /// Calls updateNewOptionTableAsync
        /// </summary>
        public string NOOptionCodeFilter
        {
            get
            {
                return _NOOptionCodeFilter;
            }
            set
            {
                _NOOptionCodeFilter = value.ToUpper();
                RaisePropertyChanged("NOOptionCodeFilter");
                informationText = "";

                updateNewOptionTableAsync();
            }
        }

        /// <summary>
        /// Calls updateNewOptionTableAsync
        /// </summary>
        public string NOBoxSizeFilter
        {
            get
            {
                return _NOBoxSizeFilter;
            }
            set
            {
                _NOBoxSizeFilter = value.ToUpper();
                RaisePropertyChanged("NOBoxSizeFilter");
                informationText = "";

                updateNewOptionTableAsync();
            }
        }

        public Modification selectedNewOption
        {
            get
            {
                return _selectedNewOption;
            }
            set
            {
                _selectedNewOption = value;
                RaisePropertyChanged("selectedNewOption");
            }
        }

        /// <summary>
        /// Calls updateModelModificationTableAsync
        /// </summary>
        public string MMSenderFilter
        {
            get
            {
                return _MMSenderFilter;
            }
            set
            {
                _MMSenderFilter = value.ToUpper();
                RaisePropertyChanged("MMSenderFilter");
                informationText = "";

                updateModelModificationTableAsync();
            }
        }

        /// <summary>
        /// Calls updateModelModificationTableAsync
        /// </summary>
        public string MMModelNameFilter
        {
            get
            {
                return _MMModelNameFilter;
            }
            set
            {
                _MMModelNameFilter = value.ToUpper();
                RaisePropertyChanged("MMModelNameFilter");
                informationText = "";

                updateModelModificationTableAsync();
            }
        }

        public Modification selectedModifiedModel
        {
            get
            {
                return _selectedModifiedModel;
            }
            set
            {
                _selectedModifiedModel = value;
                RaisePropertyChanged("selectedModifiedModel");
            }
        }

        /// <summary>
        /// Calls updateOptionModificationTableAsync
        /// </summary>
        public string OMSenderFilter
        {
            get
            {
                return _OMSenderFilter;
            }
            set
            {
                _OMSenderFilter = value.ToUpper();
                RaisePropertyChanged("OMSenderFilter");
                informationText = "";

                updateOptionModificationTableAsync();
            }
        }

        /// <summary>
        /// Calls updateOptionModificationTableAsync
        /// </summary>
        public string OMOptionCodeFilter
        {
            get
            {
                return _OMOptionCodeFilter;
            }
            set
            {
                _OMOptionCodeFilter = value.ToUpper();
                RaisePropertyChanged("OMOptionCodeFilter");
                informationText = "";

                updateOptionModificationTableAsync();
            }
        }

        /// <summary>
        /// Calls updateOptionModificationTableAsync
        /// </summary>
        public string OMBoxSizeFilter
        {
            get
            {
                return _OMBoxSizeFilter;
            }
            set
            {
                _OMBoxSizeFilter = value.ToUpper();
                RaisePropertyChanged("OMBoxSizeFilter");
                informationText = "";

                updateOptionModificationTableAsync();
            }
        }

        public Modification selectedModifiedOption
        {
            get
            {
                return _selectedModifiedOption;
            }
            set
            {
                _selectedModifiedOption = value;
                RaisePropertyChanged("selectedModifiedOption");
            }
        }

        /// <summary>
        /// Calls updateOverrideTableAsync
        /// </summary>
        public string ORSenderFilter
        {
            get
            {
                return _ORSenderFilter;
            }
            set
            {
                _ORSenderFilter = value.ToUpper();
                RaisePropertyChanged("ORSenderFilter");
                informationText = "";

                updateOverrideTableAsync();
            }
        }

        /// <summary>
        /// Calls updateOverrideTableAsync
        /// </summary>
        public string ORModelNameFilter
        {
            get
            {
                return _ORModelNameFilter;
            }
            set
            {
                _ORModelNameFilter = value.ToUpper();
                RaisePropertyChanged("ORModelNameFilter");
                informationText = "";

                updateOverrideTableAsync();
            }
        }

        public OverrideRequest selectedOverride
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
        private async void updateNewModelTableAsync()
        {
            loading = true;
            informationText = "Loading tables...";
            await Task.Run(() => updateNewModelTable());
            loading = false;
            informationText = "";
        }

        private void updateNewModelTable()
        {
            try
            {
                newModels = new ObservableCollection<Modification>(_serviceProxy.getFilteredNewModels(NMSenderFilter, NMBaseFilter, NMBoxSizeFilter));
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        private async void updateNewOptionTableAsync()
        {
            loading = true;
            informationText = "Loading tables...";
            await Task.Run(() => updateNewOptionTable());
            loading = false;
            informationText = "";
        }

        private void updateNewOptionTable()
        {
            try
            {
                newOptions = new ObservableCollection<Modification>(_serviceProxy.getFilteredNewOptions(NOSenderFilter, NOOptionCodeFilter, NOBoxSizeFilter));
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        private async void updateModelModificationTableAsync()
        {
            loading = true;
            informationText = "Loading tables...";
            await Task.Run(() => updateModelModificationTable());
            loading = false;
            informationText = "";
        }

        private void updateModelModificationTable()
        {
            try
            {
                modifiedModels = new ObservableCollection<Modification>(_serviceProxy.getFilteredModifiedModels(MMSenderFilter, MMModelNameFilter));
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        private async void updateOptionModificationTableAsync()
        {
            loading = true;
            informationText = "Loading tables...";
            await Task.Run(() => updateOptionModificationTable());
            loading = false;
            informationText = "";
        }

        private void updateOptionModificationTable()
        {
            try
            {
                modifiedOptions = new ObservableCollection<Modification>(_serviceProxy.getFilteredModifiedOptions(OMSenderFilter, OMOptionCodeFilter, OMBoxSizeFilter));
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        private async void updateOverrideTableAsync()
        {
            loading = true;
            informationText = "Loading tables...";
            await Task.Run(() => updateOverrideTable());
            loading = false;
            informationText = "";
        }

        private void updateOverrideTable()
        {
            try
            {
                overrides = new ObservableCollection<OverrideRequest>(_serviceProxy.getFilteredOverrideRequests(ORSenderFilter, ORModelNameFilter));
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Updates the modification review date and reviewer.
        /// Should be surrounded by a try catch
        /// </summary>
        /// <param name="mod"></param>
        private void updateModification(Modification mod)
        {
            mod.ReviewDate = DateTime.Now;
            mod.Reviewer = "TEMPORARY REVIEWER";

            _serviceProxy.updateModification(mod);
        }

        /// <summary>
        /// Updates the override request review date and reviewer.
        /// Should be surrounded by a try catch
        /// </summary>
        /// <param name="mod"></param>
        private void updateOverride(OverrideRequest or)
        {
            or.ReviewDate = DateTime.Now;
            or.Reviewer = "TEMPORARY REVIEWER";

            _serviceProxy.updateOverrideRequest(or);
        }
        #endregion
    }
}
