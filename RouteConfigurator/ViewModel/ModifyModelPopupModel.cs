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

        private Model.Model _selectedModel;

        private string _renameDrive;

        private bool _canRenameDrive = false;

        private bool _renameActive = false;

        private string _renameModel;

        private string _informationText;
        #endregion

        #region RelayCommands
        public RelayCommand loadedCommand { get; set; }
        public RelayCommand renameCommand { get; set; }
        public RelayCommand acceptCommand { get; set; }
        public RelayCommand cancelCommand { get; set; }
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
            renameCommand = new RelayCommand(rename);
            acceptCommand = new RelayCommand(accept);
            cancelCommand = new RelayCommand(cancel);
            submitCommand = new RelayCommand(submit);
        }
        #endregion

        #region Commands
        private void loaded()
        {
            driveTypes = _serviceProxy.getDriveTypes();
        }

        private void rename()
        {
            renameActive = true;
        }

        private void accept()
        {
            //Check if name is valid
            //Send to director

        }

        private void cancel()
        {
            renameModel = "";
            renameActive = false;
            selectedModel = null;
            informationText = "Rename canceled";
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
                informationText = "";

                updateModelsTable();

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
                informationText = "";

                updateModelsTable();
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
                informationText = "";

                updateModelsTable();
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

        /// <summary>
        /// Turns off rename
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
                informationText = "";

                renameActive = false;
            }
        }

        public string renameDrive
        {
            get
            {
                return _renameDrive;
            }
            set
            {
                _renameDrive = value.ToUpper();
                RaisePropertyChanged("renameDrive");
                informationText = "";
            }
        }

        public bool canRenameDrive
        {
            get
            {
                return _canRenameDrive;
            }
            set
            {
                _canRenameDrive = value;
                RaisePropertyChanged("canRenameDrive");
                renameDrive = "";
            }
        }

        public bool renameActive
        {
            get
            {
                return _renameActive;
            }
            set
            {
                _renameActive = value;
                RaisePropertyChanged("renameActive");
            }
        }

        public string renameModel
        {
            get
            {
                return _renameModel;
            }
            set
            {
                _renameModel = value.ToUpper();
                RaisePropertyChanged("renameModel");
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
        #endregion

        #region Private Functions 
        private void updateModelsTable()
        {
            modelsFound = _serviceProxy.getNumModelsFound(selectedDrive, AVText, boxSize);

            if(string.IsNullOrWhiteSpace(AVText) && string.IsNullOrWhiteSpace(boxSize))
            {
                canRenameDrive = true;
            }
            else
            {
                canRenameDrive = false;
            }
        }
        #endregion
    }
}
