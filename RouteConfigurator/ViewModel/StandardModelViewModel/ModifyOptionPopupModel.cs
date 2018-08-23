using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Model.EF_StandardModels;
using RouteConfigurator.Services;
using RouteConfigurator.Services.Interface;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RouteConfigurator.ViewModel.StandardModelViewModel
{
    public class ModifyOptionPopupModel : ViewModelBase
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

        /// <summary>
        /// List of unique option codes to populate a drop down box
        /// </summary>
        private ObservableCollection<string> _optionCodes = new ObservableCollection<string>();
        private string _selectedOptionCode;

        /// <summary>
        /// List of unique box sizes for the options to populate a drop down box
        /// </summary>
        private ObservableCollection<string> _boxSizes = new ObservableCollection<string>();
        private string _boxSize = "";

        /// <summary>
        /// All options found that meet the filters
        /// These are the options that will be modified when submitted
        /// </summary>
        private ObservableCollection<Option> _optionsFound = new ObservableCollection<Option>();
        private Option _selectedOption;

        //New information
        private decimal? _newTime;
        private string _newName;

        private string _description;

        private string _informationText;

        private bool _loading = false;
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
            submitCommand = new RelayCommand(submitAsync);
        }
        #endregion

        #region Commands
        private async void loaded()
        {
            loading = true;
            await Task.Run(() => getInformation());
            loading = false;
        }

        /// <summary>
        /// Loads the information, option codes and box sizes, needed for the page 
        /// </summary>
        private void getInformation()
        {
            try
            {
                informationText = "Loading Information...";
                optionCodes = new ObservableCollection<string>(_serviceProxy.getOptionCodes());
                boxSizes = new ObservableCollection<string>(_serviceProxy.getOptionBoxSizes());
                informationText = "";
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }
        
        private async void submitAsync()
        {
            loading = true;
            await Task.Run(() => submit());
            loading = false;
        }

        /// <summary>
        /// Submits each option modification to the database
        /// Calls checkComplete
        /// </summary>
        private void submit()
        {
            if (optionsFound.Count <= 0)
            {
                informationText = "No options selected to update.";
            }
            else if (checkComplete())
            {
                try
                {
                    informationText = "Submitting option modifications...";
                    foreach (Option option in optionsFound)
                    {
                        Modification modifiedOption = new Modification()
                        {
                            RequestDate = DateTime.Now,
                            OptionCode = option.OptionCode,
                            BoxSize = option.BoxSize,
                            Description = string.IsNullOrWhiteSpace(description) ? "no description entered" : description,
                            State = 0,
                            Sender = string.Format("{0} {1}", _navigationService.user.FirstName, _navigationService.user.LastName),
                            IsOption = true,
                            IsNew = false,
                            NewTime = newTime == null || newTime <= 0 ? option.Time : (decimal)newTime,
                            NewName = string.IsNullOrWhiteSpace(newName) ? option.Name : newName,
                            OldOptionTime = option.Time,
                            OldOptionName = option.Name,

                            Reviewer = "",
                            ReviewDate = new DateTime(1900, 1, 1),
                            ModelBase = "",
                            ProductLine = ""
                        };

                        _serviceProxy.addModificationRequest(modifiedOption);
                    }

                    //Clear input boxes
                    _selectedOptionCode = null;
                    RaisePropertyChanged("selectedOptionCode");
                    _boxSize = null; 
                    RaisePropertyChanged("boxSize");
                    optionsFound = new ObservableCollection<Option>();

                    newTime = null;
                    newName = null;
                    description = "";

                    informationText = "Option modifications have been submitted.  Waiting for manager approval.";
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
        /// Calls updateOptionsTableAsync
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

                updateOptionsTableAsync();
            }
        }

        public ObservableCollection<string> boxSizes
        {
            get
            {
                return _boxSizes;
            }
            set
            {
                _boxSizes = value;
                RaisePropertyChanged("boxSizes");
            }
        }

        /// <summary>
        /// Calls updateOptionsTableAsync
        /// </summary>
        public string boxSize
        {
            get
            {
                return _boxSize;
            }
            set
            {
                _boxSize = value == null ? value : value.ToUpper();
                RaisePropertyChanged("boxSize");
                informationText = "";

                updateOptionsTableAsync();
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
        private async void updateOptionsTableAsync()
        {
            loading = true;
            await Task.Run(() => updateOptionsTable());
            loading = false;
        }

        /// <summary>
        /// Updates the options table with the filtered information
        /// </summary>
        private void updateOptionsTable()
        {
            //If no filters are entered clear the list so no options will be updated
            if (string.IsNullOrWhiteSpace(selectedOptionCode) && string.IsNullOrWhiteSpace(boxSize))
            {
                optionsFound = new ObservableCollection<Option>();
            }
            else
            {
                try
                {
                    informationText = "Loading options...";
                    //If a box size is entered get the options that are that box size and meet the other filter
                    if (!string.IsNullOrWhiteSpace(boxSize))
                    {
                        optionsFound = new ObservableCollection<Option>(_serviceProxy.getOptionsFound(selectedOptionCode, boxSize));
                    }
                    //If a box size is not entered get the options that meet the other filter
                    else
                    {
                        optionsFound = new ObservableCollection<Option>(_serviceProxy.getOptionsFound(selectedOptionCode));
                    }
                    informationText = "";
                }
                catch (Exception e)
                {
                    informationText = "There was a problem accessing the database";
                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Checks to see if new time or new name is filled out correctly
        /// before the modification can be added.  
        /// </summary>
        /// <remarks> Either newDriveTime or newAVTime need to be filled out </remarks>
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
