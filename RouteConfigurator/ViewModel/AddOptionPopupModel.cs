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
        private string _description;

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

        public string description 
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                RaisePropertyChanged("description");
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
        /// Calls checkComplete
        /// </summary>
        /// <returns> true if the option is valid and doesn't already exist, false otherwise </returns>
        private bool checkValid()
        {
            bool valid = checkComplete();
            if (valid)
            {
                using(RouteConfiguratorDB context = new RouteConfiguratorDB())
                {
                    List<Option> result = context.Options.Where(option => option.OptionCode.Equals(optionCode) &&
                                                                          option.BoxSize.Equals(boxSize)).ToList();

                    if(result.Count > 0)
                    {
                    informationText = "This option already exists";
                    valid = false;

                    }

                }
                //Check that the option doesn't already exist in the database
                ObservableCollection<Option> options = new ObservableCollection<Option>(_serviceProxy.getFilteredOptions(optionCode, boxSize));
                if (options.Count > 0)
                {
                    informationText = "This option already exists";
                    valid = false;
                }
                else
                {
                    //Check that the option isn't a duplicate in the ready to submit list
                    foreach (Option option in optionsToSubmit)
                    {
                        if (option.OptionCode.Equals(optionCode) && option.BoxSize.Equals(boxSize))
                        {
                            informationText = "This option is already ready to submit";
                            valid = false;
                            break;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(boxSize) &&
                time != null && time > 0)
                {
                }
                else
                {
                    informationText = "Necessary information missing";
                    valid = false;
                }
            }

            return valid;
        }

        private bool checkComplete()
        {
            bool complete = false;

            if (!string.IsNullOrWhiteSpace(optionCode))
            {
                if (optionCode.Length != 2)
                {
                    informationText = "Invalid Option Code Format.  Must be 2 letters";
                    complete = false;
                }
                else
                {
                    if (!optionCode.ElementAt(0).Equals('P') && !optionCode.ElementAt(0).Equals('T'))
                    {
                        informationText = "Option Code must start with a 'P' or 'T'";
                        complete = false;
                    }
                }
            }
            else
            {
                informationText = "Option Code missing";
                complete = false;
            }

            return complete;
        }
        #endregion
    }
}
