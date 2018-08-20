using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Model.EF_StandardModels;
using RouteConfigurator.Services;
using RouteConfigurator.Services.Interface;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RouteConfigurator.ViewModel.StandardModelViewModel
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

        //Inputs
        private string _optionCode;
        private string _boxSize;
        private decimal? _time;
        private string _name;
        private string _description;

        private ObservableCollection<Modification> _modificationsToSubmit = new ObservableCollection<Modification>();

        private string _informationText;

        private bool _loading = false;
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

            addOptionCommand = new RelayCommand(addOptionAsync);
            submitCommand = new RelayCommand(submitAsync);
        }
        #endregion

        #region Commands
        private async void addOptionAsync()
        {
            loading = true;
            await Task.Run(() => addOption());
            loading = false;
        }

        /// <summary>
        /// Creates the new option modification and adds it to the modifications to submit list
        /// Calls checkValid
        /// </summary>
        private void addOption()
        {
            informationText = "";

            if (checkValid())
            {
                informationText = "Adding option...";

                Modification mod = new Modification()
                {
                    RequestDate = DateTime.Now,
                    OptionCode = optionCode,
                    BoxSize = boxSize,
                    Description = string.IsNullOrWhiteSpace(description) ? "no description entered" : description,
                    State = 0,
                    Sender = "TEMPORARY PLACEHOLDER",
                    IsOption = true,
                    IsNew = true,
                    NewTime = (decimal)time,
                    NewName = name == null ? "" : name,

                    Reviewer = "",
                    ReviewDate = new DateTime(1900, 1, 1),
                    ModelBase = "",
                    OldOptionName = "",
                    ProductLine = ""
                };

                // Since the observable collection was created on the UI thread 
                // we have to add the option to the list using a delegate function.
                App.Current.Dispatcher.Invoke(delegate
                {
                    modificationsToSubmit.Add(mod);
                });

                //Clear input boxes
                boxSize = "";
                time = null;
                informationText = "Option added.";
            }
        }

        private async void submitAsync()
        {
            loading = true;
            await Task.Run(() => submit());
            loading = false;
        }

        /// <summary>
        /// Submits each of the new option modifications to the database
        /// </summary>
        private void submit()
        {
            informationText = "";

            if (modificationsToSubmit.Count > 0)
            {
                try
                {
                    informationText = "Submitting option modifications...";

                    foreach (Modification mod in modificationsToSubmit)
                    {
                        _serviceProxy.addModificationRequest(mod);
                    }
                }
                catch (Exception e)
                {
                    informationText = "There was a problem accessing the database";
                    Console.WriteLine(e);
                    return;
                }

                //Clear input boxes
                optionCode = "";
                boxSize = "";
                time = null;
                name = "";
                description = "";

                modificationsToSubmit = new ObservableCollection<Modification>();

                informationText = "Options have been submitted.  Waiting for manager approval.";
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

        public ObservableCollection<Modification> modificationsToSubmit
        {
            get
            {
                return _modificationsToSubmit;
            }
            set
            {
                _modificationsToSubmit = value;
                RaisePropertyChanged("modificationsToSubmit");
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
        /// <summary>
        /// Checks that the option does not already exist
        /// Calls checkComplete
        /// </summary>
        /// <returns> true if the option is valid and doesn't already exist, false otherwise </returns>
        private bool checkValid()
        {
            bool valid = checkComplete();
            if (valid)
            {
                try
                {
                    //Check if the option already exists in the database as an option
                    if (_serviceProxy.getFilteredOptions(optionCode, boxSize, true).ToList().Count > 0)
                    {
                        informationText = string.Format("Option {0}-{1} already exists.", optionCode, boxSize);
                        valid = false;
                    }
                    //Check if the option already exists in the database as a new option request
                    else if (_serviceProxy.getFilteredNewOptions("", optionCode, boxSize).ToList().Count > 0)
                    {
                        informationText = string.Format("Option {0}-{1} is already waiting for approval.", optionCode, boxSize);
                        valid = false;
                    }
                    else
                    {
                        //Check if the option is a duplicate in the ready to submit list
                        foreach (Modification newOption in modificationsToSubmit)
                        {
                            if (newOption.OptionCode.Equals(optionCode) && newOption.BoxSize.Equals(boxSize))
                            {
                                informationText = "This option is already ready to submit";
                                valid = false;
                                break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    informationText = "There was a problem accessing the database";
                    Console.WriteLine(e);
                }
            }
            return valid;
        }

        /// <summary>
        /// Checks to see if all necessary fields are filled out with correct formatting
        /// before the option can be added.
        /// </summary>
        /// <returns> true if the form is complete, otherwise false</returns>
        private bool checkComplete()
        {
            bool complete = true;

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
                    else if(optionCode.ElementAt(1).Equals('P') || optionCode.ElementAt(1).Equals('T'))
                    {
                        informationText = "Option Code cannot end with a 'P' or 'T'";
                        complete = false;
                    }
                }

                if (string.IsNullOrWhiteSpace(boxSize) || time == null || time <= 0)
                {
                    informationText = "Necessary information missing";
                    complete = false;
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
