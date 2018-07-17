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
    public class ModifyOptionPopupModel : ViewModelBase
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

        private ObservableCollection<string> _optionCodes = new ObservableCollection<string>();

        private string _selectedOptionCode;

        private string _boxSize = "";

        private ObservableCollection<Option> _optionsFound;

        private Option _selectedOption;

        private bool _optionCodeNotSelected = true;

        private string _renameOptionCodes;

        private bool _canRenameOptionCodes = false;

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
        public ModifyOptionPopupModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            loadedCommand = new RelayCommand(loaded);
            submitCommand = new RelayCommand(submit);
        }
        #endregion

        #region Commands
        private void loaded()
        {
            optionCodes = _serviceProxy.getOptionCodes();
        }

        private void submit()
        {
            MessageBox.Show("Placeholder for sending updates to director");
        }
        #endregion

        #region Public Variables
        public ObservableCollection<string> optionCodes
        {
            get
            {
                return _optionCodes;
            }
            set
            {
                _optionCodes = value;
                RaisePropertyChanged("optionCodes");
            }
        }

        public string selectedOptionCode
        {
            get
            {
                return _selectedOptionCode;
            }
            set
            {
                _selectedOptionCode = value;
                RaisePropertyChanged("selectedOptionCode");
                informationText = "";

                updateOptionsTable();

                optionCodeNotSelected = value == null ? true : false;

                boxSize = "";
            }
        }

        public bool optionCodeNotSelected
        {
            get
            {
                return _optionCodeNotSelected;
            }
            set
            {
                _optionCodeNotSelected = value;
                RaisePropertyChanged("optionCodeNotSelected");
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

                updateOptionsTable();
            }
        }

        public ObservableCollection<Option> optionsFound
        {
            get
            {
                return _optionsFound;
            }
            set
            {
                _optionsFound = value;
                RaisePropertyChanged("optionsFound");
            }
        }

        public Option selectedOption
        {
            get
            {
                return _selectedOption;
            }
            set
            {
                _selectedOption = value;
                RaisePropertyChanged("selectedOption");
                informationText = "";
            }
        }

        public string renameOptionCodes
        {
            get
            {
                return _renameOptionCodes;
            }
            set
            {
                _renameOptionCodes = value.ToUpper();
                RaisePropertyChanged("renameOptionCodes");
                informationText = "";
            }
        }

        public bool canRenameOptionCodes
        {
            get
            {
                return _canRenameOptionCodes;
            }
            set
            {
                _canRenameOptionCodes = value;
                RaisePropertyChanged("canRenameOptionCodes");
                renameOptionCodes = "";
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
        private void updateOptionsTable()
        {
            optionsFound = _serviceProxy.getNumOptionsFound(selectedOptionCode, boxSize);

            if(string.IsNullOrWhiteSpace(boxSize))
            {
                canRenameOptionCodes = true;
            }
            else
            {
                canRenameOptionCodes = false;
            }
        }
        #endregion
    }
}
