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
        private bool _exactBoxSize = false;

        private ObservableCollection<Option> _optionsFound = new ObservableCollection<Option>();
        private Option _selectedOption;

        private decimal? _newTime;
        private string _newName;
        private string _description;

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
            try
            {
                optionCodes = new ObservableCollection<string>(_serviceProxy.getOptionCodes());
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Submits each option modification to the database
        /// </summary>
        private void submit()
        {
            if (optionsFound.Count <= 0)
            {
                informationText = "No options selected to update.";
            }
            else if (checkValid())
            {
                try
                {
                    foreach (Option option in optionsFound)
                    {
                        Modification modifiedOption = new Modification()
                        {
                            RequestDate = DateTime.Now,
                            OptionCode = option.OptionCode,
                            BoxSize = option.BoxSize,
                            Description = string.IsNullOrWhiteSpace(description) ? "no description entered" : description,
                            State = 0,
                            Sender = "TEMPORARY PLACEHOLDER",
                            IsOption = true,
                            IsNew = false,
                            NewTime = newTime == null || newTime <= 0 ? option.Time : (decimal)newTime,
                            NewName = string.IsNullOrWhiteSpace(newName) ? option.Name : newName,
                            OldOptionTime = option.Time,
                            OldOptionName = option.Name,

                            Reviewer = "",
                            ReviewDate = new DateTime(1900, 1, 1),
                            ModelBase = "",
                        };

                        _serviceProxy.addModificationRequest(modifiedOption);
                    }

                    //Clear input boxes
                    selectedOptionCode = null;
                    boxSize = "";
                    optionsFound.Clear();
                    newTime = null;
                    newName = null;
                    description = "";

                    informationText = "Option modifications have been submitted.  Waiting for director approval.";
                }
                catch (Exception e)
                {
                    informationText = "There was a problem accessing the database";
                    Console.WriteLine(e);
                    return;
                }
            }
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

        /// <summary>
        /// Calls updateOptionsTable
        /// </summary>
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
            }
        }

        /// <summary>
        /// Calls updateOptionsTable
        /// </summary>
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

        /// <summary>
        /// Calls updateOptionsTable
        /// </summary>
        public bool exactBoxSize
        {
            get
            {
                return _exactBoxSize;
            }
            set
            {
                _exactBoxSize = value;
                RaisePropertyChanged("exactBoxSize");
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

        public decimal? newTime
        {
            get
            {
                return _newTime;
            }
            set
            {
                _newTime = value;
                RaisePropertyChanged("newTime");
                informationText = "";
            }
        }

        public string newName
        {
            get
            {
                return _newName;
            }
            set
            {
                _newName = value;
                RaisePropertyChanged("newName");
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
        /// Updates the options table with the filtered information
        /// </summary>
        private void updateOptionsTable()
        {
            try
            {
                optionsFound = new ObservableCollection<Option>(_serviceProxy.getNumOptionsFound(selectedOptionCode, boxSize, exactBoxSize));
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Calls checkComplete
        /// </summary>
        /// <returns> checkComplete value </returns>
        private bool checkValid()
        {
            bool valid = checkComplete();

            /*  I'm not sure how to handle an already existing modification request for an option
            *  so I will allow multiple of the same option requests.
            if (valid)
            {
                //Check if the option already exists in the database as an option modification request
                List<Modification> mods = _serviceProxy.getFilteredModifiedOptions("", selectedOptionCode, boxSize).ToList();
                if (mods.Count > 0)
                {
                    informationText = string.Format("Option {0} is already waiting for approval.", selectedOptionCode);
                    valid = false;
                }
            }
            */

            return valid;
        }

        /// <summary>
        /// Checks to see if new time or new name is filled out correctly
        /// before the modification can be added.  
        /// </summary>
        /// <returns> true if the form is complete, otherwise false </returns>
        private bool checkComplete()
        {
            bool complete = true;

            if ((newTime == null || newTime <= 0) && (string.IsNullOrWhiteSpace(newName)))
            {
                informationText = "No new information associated with modification.";
                complete = false;
            }

            return complete;
        }
        #endregion
    }
}
