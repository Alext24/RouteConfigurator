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
    public class ModifyModelPopupModel : ViewModelBase
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

        private ObservableCollection<string> _driveTypes = new ObservableCollection<string>();

        private string _selectedDrive;

        private bool _driveNotSelected = true;

        private string _AVText = "";

        private string _boxSize = "";

        private ObservableCollection<Model.Model> _modelsFound;

        private string _informationText;
        #endregion

        #region RelayCommands
        public RelayCommand loadedCommand { get; set; }
        public RelayCommand submitCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public ModifyModelPopupModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            loadedCommand = new RelayCommand(loaded);
            submitCommand = new RelayCommand(submit);
        }
        #endregion

        #region Commands
        private void loaded()
        {
            driveTypes = _serviceProxy.getDriveTypes();
        }

        private void submit()
        {
            MessageBox.Show("Placeholder for sending updates to director");
        }
        #endregion

        #region Public Variables
        public ObservableCollection<string> driveTypes
        {
            get
            {
                return _driveTypes;
            }
            set
            {
                _driveTypes = value;
                RaisePropertyChanged("driveTypes");
            }
        }

        public string selectedDrive
        {
            get
            {
                return _selectedDrive;
            }
            set
            {
                _selectedDrive = value;
                RaisePropertyChanged("selectedDrive");

                modelsFound = _serviceProxy.getNumModelsFound(selectedDrive, AVText, boxSize); 

                driveNotSelected = value != null ? false : true;

                AVText = "";
                boxSize = "";
            }
        }

        public bool driveNotSelected
        {
            get
            {
                return _driveNotSelected;
            }
            set
            {
                _driveNotSelected = value;
                RaisePropertyChanged("driveNotSelected");
            }
        }

        public string AVText
        {
            get
            {
                return _AVText;
            }
            set
            {
                _AVText = value.ToUpper();
                RaisePropertyChanged("AVText");

                modelsFound = _serviceProxy.getNumModelsFound(selectedDrive, AVText, boxSize); 
            }
        }

        public string boxSize
        {
            get
            {
                return _boxSize;
            }
            set
            {
                _boxSize = value.ToUpper();
                RaisePropertyChanged("boxSize");

                modelsFound = _serviceProxy.getNumModelsFound(selectedDrive, AVText, boxSize); 
            }
        }

        public ObservableCollection<Model.Model> modelsFound
        {
            get
            {
                return _modelsFound;
            }
            set
            {
                _modelsFound = value;
                RaisePropertyChanged("modelsFound");
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
        #endregion
    }
}
