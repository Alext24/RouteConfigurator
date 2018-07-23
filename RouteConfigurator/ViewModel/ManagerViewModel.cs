using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Design;
using RouteConfigurator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        private Modification _selectedModifiedOption;

        private string _ORSenderFilter = "";
        private string _ORModelNameFilter = "";
        private OverrideRequest _selectedOverride;

        private string _informationText;
        #endregion

        #region RelayCommands
        public RelayCommand loadedCommand { get; set; }
        public RelayCommand approveNewModelCommand { get; set; }
        public RelayCommand declineNewModelCommand { get; set; }
        public RelayCommand approveNewOptionCommand { get; set; }
        public RelayCommand declineNewOptionCommand { get; set; }
        public RelayCommand approveModifiedModelCommand { get; set; }
        public RelayCommand declineModifiedModelCommand { get; set; }
        public RelayCommand approveModifiedOptionCommand { get; set; }
        public RelayCommand declineModifiedOptionCommand { get; set; }
        public RelayCommand approveOverrideCommand { get; set; }
        public RelayCommand declineOverrideCommand { get; set; }
        public RelayCommand approveCheckedCommand { get; set; }
        public RelayCommand declineCheckedCommand { get; set; }
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
            approveNewModelCommand = new RelayCommand(approveNM);
            declineNewModelCommand = new RelayCommand(declineNM);
            approveNewOptionCommand = new RelayCommand(approveNO);
            declineNewOptionCommand = new RelayCommand(declineNO);
            approveModifiedModelCommand = new RelayCommand(approveMM);
            declineModifiedModelCommand = new RelayCommand(declineMM);
            approveModifiedOptionCommand = new RelayCommand(approveMO);
            declineModifiedOptionCommand = new RelayCommand(declineMO);
            approveOverrideCommand = new RelayCommand(approveOR);
            declineOverrideCommand = new RelayCommand(declineOR);
            approveCheckedCommand = new RelayCommand(approveChecked);
            declineCheckedCommand = new RelayCommand(declineChecked);
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

            /*
            newModels = _serviceProxy.getFilteredNewModels();
            newOptions = _serviceProxy.getFilteredNewOptions();
            modifiedModels = _serviceProxy.getFilteredModifiedModels();
            modifiedOptions = _serviceProxy.getFilteredModifiedOptions();
            */
        }

        private void approveNM()
        {
            MessageBox.Show("Placeholder for approving");
        }

        private void declineNM()
        {
            MessageBox.Show("Placeholder for declining");
        }

        private void approveNO()
        {
            MessageBox.Show("Placeholder for approving");
        }

        private void declineNO()
        {
            MessageBox.Show("Placeholder for declining");
        }

        private void approveMM()
        {
            MessageBox.Show("Placeholder for approving");
        }

        private void declineMM()
        {
            MessageBox.Show("Placeholder for declining");
        }

        private void approveMO()
        {
            MessageBox.Show("Placeholder for approving");
        }

        private void declineMO()
        {
            MessageBox.Show("Placeholder for declining");
        }

        private void approveOR()
        {
            MessageBox.Show("Placeholder for approving");
        }

        private void declineOR()
        {
            MessageBox.Show("Placeholder for declining");
        }

        private void approveChecked()
        {
            MessageBox.Show("Placeholder for approving all selected");
        }

        private void declineChecked()
        {
            MessageBox.Show("Placeholder for delcining all selected");
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
        private void updateNewModelTable()
        {
            newModels = _serviceProxy.getFilteredNewModels(NMSenderFilter, NMBaseFilter, NMBoxSizeFilter);
        }

        private void updateNewOptionTable()
        {
            newOptions = _serviceProxy.getFilteredNewOptions(NOSenderFilter, NOOptionCodeFilter, NOBoxSizeFilter);
        }

        private void updateModelModificationTable()
        {
            modifiedModels = _serviceProxy.getFilteredModifiedModels(MMSenderFilter, MMModelNameFilter);
        }

        private void updateOptionModificationTable()
        {
            modifiedOptions = _serviceProxy.getFilteredModifiedOptions(OMSenderFilter, OMOptionCodeFilter);
        }

        private void updateOverrideTable()
        {
            overrides = new ObservableCollection<OverrideRequest>(_serviceProxy.getFilteredOverrideRequests(ORSenderFilter, ORModelNameFilter));
        }
        #endregion
    }
}
