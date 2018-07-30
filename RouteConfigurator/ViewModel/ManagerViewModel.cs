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
        #endregion

        #region RelayCommands
        public RelayCommand loadedCommand { get; set; }
        public RelayCommand openSupervisorCommand { get; set; }
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
            submitCheckedCommand = new RelayCommand(submitChecked);
            goBackCommand = new RelayCommand(goBack);
        }
        #endregion

        #region Commands
        private void loaded()
        {
            updateNewModelTable();
            updateNewOptionTable();
            updateModelModificationTable();
            updateOptionModificationTable();
            updateOverrideTable();
        }

        private void openSupervisorView()
        {
            MainWindow secondWindow = new MainWindow();
            Page sup = new SupervisorView();

            secondWindow.Show();
            secondWindow.Content = sup;
        }

        private void submitChecked()
        {
            informationText = "";
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
                    Model.Model model = new Model.Model()
                    {
                        Base = mod.ModelBase,
                        BoxSize = mod.BoxSize,
                        DriveTime = mod.NewDriveTime,
                        AVTime = mod.NewAVTime
                    };

                    try
                    {
                        _serviceProxy.addModel(model);
                        numApproved++;

                        updateModification(mod);
                    }
                    catch (Exception e)
                    {
                        errorText += string.Format("Error adding model {0}\n", model.Base);
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

            foreach (Modification mod in newOptions)
            {
                if (mod.State == 3)
                {
                    Option option = new Option()
                    {
                        OptionCode = mod.OptionCode,
                        BoxSize = mod.BoxSize,
                        Time = mod.NewTime,
                        Name = mod.NewName
                    };
                    try
                    {
                        _serviceProxy.addOption(option);
                        numApproved++;

                        updateModification(mod);
                    }
                    catch (Exception e)
                    {
                        errorText += string.Format("Error adding option {0}-{1}\n", option.OptionCode, option.BoxSize);
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
                    try
                    {
                        _serviceProxy.updateModel(mod.ModelBase, mod.NewDriveTime, mod.NewAVTime);
                        numApproved++;

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
                    Override ov = new Override()
                    {
                        ModelNum = or.ModelNum,
                        OverrideRoute = or.OverrideRoute,
                        OverrideTime = or.OverrideTime
                    };
                    try
                    {
                        _serviceProxy.addOverride(ov, or.ModelBase);
                        numApproved++;

                        updateOverride(or);
                    }
                    catch (Exception e)
                    {
                        errorText += string.Format("Error adding override {0}\n", ov.ModelNum);
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

            refreshTables();
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
        /// Calls updateNewModelTable
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

                updateNewModelTable();
            }
        }

        /// <summary>
        /// Calls updateNewModelTable
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

                updateNewModelTable();
            }
        }

        /// <summary>
        /// Calls updateNewModelTable
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

                updateNewModelTable();
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
        /// Calls updateNewOptionTable
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

                updateNewOptionTable();
            }
        }

        /// <summary>
        /// Calls updateNewOptionTable
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

                updateNewOptionTable();
            }
        }

        /// <summary>
        /// Calls updateNewOptionTable
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

                updateNewOptionTable();
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
        /// Calls updateModelModificationTable
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

                updateModelModificationTable();
            }
        }

        /// <summary>
        /// Calls updateModelModificationTable
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

                updateModelModificationTable();
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
        /// Calls updateOptionModificationTable
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

                updateOptionModificationTable();
            }
        }

        /// <summary>
        /// Calls updateOptionModificationTable
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

                updateOptionModificationTable();
            }
        }

        /// <summary>
        /// Calls updateOptionModificationTable
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

                updateOptionModificationTable();
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
        /// Calls updateOverrideTable
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

                updateOverrideTable();
            }
        }

        /// <summary>
        /// Calls updateOverrideTable
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

                updateOverrideTable();
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
        #endregion

        #region Private Functions
        private void refreshTables()
        {
            updateNewModelTable();
            updateNewOptionTable();
            updateModelModificationTable();
            updateOptionModificationTable();
            updateOverrideTable();
        }

        private void updateNewModelTable()
        {
            newModels = new ObservableCollection<Modification>(_serviceProxy.getFilteredNewModels(NMSenderFilter, NMBaseFilter, NMBoxSizeFilter));
        }

        private void updateNewOptionTable()
        {
            newOptions = new ObservableCollection<Modification>(_serviceProxy.getFilteredNewOptions(NOSenderFilter, NOOptionCodeFilter, NOBoxSizeFilter));
        }

        private void updateModelModificationTable()
        {
            modifiedModels = new ObservableCollection<Modification>(_serviceProxy.getFilteredModifiedModels(MMSenderFilter, MMModelNameFilter));
        }

        private void updateOptionModificationTable()
        {
            modifiedOptions = new ObservableCollection<Modification>(_serviceProxy.getFilteredModifiedOptions(OMSenderFilter, OMOptionCodeFilter, OMBoxSizeFilter));
        }

        private void updateOverrideTable()
        {
            overrides = new ObservableCollection<OverrideRequest>(_serviceProxy.getFilteredOverrideRequests(ORSenderFilter, ORModelNameFilter));
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
