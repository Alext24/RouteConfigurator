﻿using GalaSoft.MvvmLight;
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
            await Task.Run(() => getOptionCodes());
            loading = false;
        }

        private void getOptionCodes()
        {
            try
            {
                informationText = "Loading option codes...";
                optionCodes = new ObservableCollection<string>(_serviceProxy.getOptionCodes());
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
                            ProductLine = ""
                        };

                        _serviceProxy.addModificationRequest(modifiedOption);
                    }

                    //Clear input boxes
                    _selectedOptionCode = null;
                    RaisePropertyChanged("selectedOptionCode");
                    _boxSize = "";
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
                _boxSize = value.ToUpper();
                RaisePropertyChanged("boxSize");
                informationText = "";

                updateOptionsTableAsync();
            }
        }

        /// <summary>
        /// Calls updateOptionsTableAsync
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
            if (string.IsNullOrWhiteSpace(selectedOptionCode) && string.IsNullOrWhiteSpace(boxSize))
            {
                optionsFound = new ObservableCollection<Option>();
            }
            else
            {
                try
                {
                    informationText = "Loading options...";
                    optionsFound = new ObservableCollection<Option>(_serviceProxy.getOptionsFound(selectedOptionCode, boxSize, exactBoxSize));
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