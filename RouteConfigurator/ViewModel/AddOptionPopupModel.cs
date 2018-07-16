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
    public class AddOptionPopupModel : ViewModelBase
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

        private string _optionCode;
        private string _boxSize;
        private decimal? _time;
        private string _name;

        private ObservableCollection<Option> _optionsToSubmit = new ObservableCollection<Option>();

        private string _informationText;
        #endregion

        #region RelayCommands
        public RelayCommand addOptionCommand { get; set; }
        public RelayCommand submitCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public AddOptionPopupModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            addOptionCommand = new RelayCommand(addOption);
            submitCommand = new RelayCommand(submit);
        }
        #endregion

        #region Commands
        /// <summary>
        /// Creates the new option and adds it to the options to submit list
        /// Calls checkValid
        /// </summary>
        private void addOption()
        {
            if (checkValid())
            {
                Option option = new Option
                {
                    OptionCode = optionCode,
                    BoxSize = boxSize,
                    Time = (decimal)time,
                    Name = name
                };

                optionsToSubmit.Add(option);

                //Clear input boxes
                boxSize = "";
                time = null;
            }
        }

        private void submit()
        {
            if (optionsToSubmit.Count > 0)
            {
                MessageBox.Show("Hi\nPlaceholder for sending options to director");
                //optionsToSubmit.Clear();
            }
            else
            {
                informationText = "No options to submit.";
            }
        }
        #endregion

        #region Public Variables
        public string optionCode 
        {
            get
            {
                return _optionCode;
            }
            set
            {
                _optionCode = value.ToUpper();
                RaisePropertyChanged("optionCode");
                informationText = "";
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
            }
        }

        public decimal? time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                RaisePropertyChanged("time");
                informationText = "";
            }
        }

        public string name 
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                RaisePropertyChanged("name");
                informationText = "";
            }
        }

        public ObservableCollection<Option> optionsToSubmit
        {
            get
            {
                return _optionsToSubmit;
            }
            set
            {
                _optionsToSubmit = value;
                RaisePropertyChanged("optionsToSubmit");
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
        /// <summary>
        /// Checks that all necessary fields for the option are entered
        /// Also checks that the option does not already exist
        /// </summary>
        /// <returns> true if the option is valid and doesn't already exist </returns>
        private bool checkValid()
        {
            bool isValid = false;

            if (!string.IsNullOrWhiteSpace(optionCode) && 
                !string.IsNullOrWhiteSpace(boxSize) && 
                time != null && time > 0)
            {
                //Check that the option doesn't already exist in the database
                ObservableCollection<Option> options = _serviceProxy.getFilteredOptions(optionCode, boxSize);
                if(options.Count < 1)
                {
                    isValid = true;
                }
                else
                {
                    informationText = "This option already exists";
                }

                //Check that the option isn't a duplicate in the ready to submit list
                foreach(Option option in optionsToSubmit)
                {
                    if (option.OptionCode.Equals(optionCode) && option.BoxSize.Equals(boxSize))
                    {
                        isValid = false;
                        informationText = "This option is already ready to submit";
                        break;
                    }
                }
            }
            else
            {
                informationText = "Necessary information missing";
            }

            return isValid;
        }
        #endregion
    }
}
