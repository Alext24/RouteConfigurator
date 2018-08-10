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

namespace RouteConfigurator.ViewModelEngineered
{
    public class ModifyComponentsPopupModel : ViewModelBase
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

        private string _componentName;
        public ObservableCollection<string> _enclosureSizes = new ObservableCollection<string>();
        private string _enclosureSize;
        private decimal? _time;
        private string _description;

        private ObservableCollection<EngineeredModification> _modificationsToSubmit = new ObservableCollection<EngineeredModification>();

        private string _informationText;

        private bool _loading = false;
        #endregion

        #region RelayCommands
        public RelayCommand loadedCommand { get; set; }
        public RelayCommand addComponentCommand { get; set; }
        public RelayCommand submitCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public ModifyComponentsPopupModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            loadedCommand = new RelayCommand(loaded);
            addComponentCommand = new RelayCommand(addComponentAsync);
            submitCommand = new RelayCommand(submitAsync);
        }
        #endregion

        #region Commands
        private void loaded()
        {
            enclosureSizes = new ObservableCollection<string>(_serviceProxy.getEnclosureSizes());
        }

        private async void addComponentAsync()
        {
            loading = true;
            await Task.Run(() => addComponent());
            loading = false;
        }

        /// <summary>
        /// Creates the new option modification and adds it to the options to submit list
        /// Calls checkValid
        /// </summary>
        private void addComponent()
        {
            informationText = "";

            if (checkValid())
            {
                /*
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
                    OldOptionName = ""
                };

                // Since the observable collection was created on the UI thread 
                // we have to add the override to the list using a delegate function.
                App.Current.Dispatcher.Invoke(delegate
                {
                    modificationsToSubmit.Add(mod);
                });

                //Clear input boxes
                boxSize = "";
                time = null;
                informationText = "Option added.";
                */
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
                /*
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
                */
            }
            else
            {
                informationText = "No components to submit.";
            }
        }
        #endregion

        #region Public Variables
        public string componentName 
        {
            get
            {
                return _componentName;
            }
            set
            {
                _componentName = value.ToUpper();
                RaisePropertyChanged("componentName");
                informationText = "";
            }
        }

        public ObservableCollection<string> enclosureSizes
        {
            get
            {
                return _enclosureSizes;
            }
            set
            {
                _enclosureSizes = value;
                RaisePropertyChanged("enclosureSizes");
            }
        }

        public string enclosureSize
        {
            get
            {
                return _enclosureSize;
            }
            set
            {
                _enclosureSize = value;
                RaisePropertyChanged("enclosureSize");
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

        public ObservableCollection<EngineeredModification> modificationsToSubmit
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
/*
            if (valid)
            {
                try
                {
                    //Check if the option already exists in the database as an option
                    if (_serviceProxy.getFilteredOptions(optionCode, boxSize, true).ToList().Count > 0)
                    {
                        informationText = "This option already exists";
                        valid = false;
                    }
                    else
                    {
                        //Check if the option already exists in the database as a new option request
                        if (_serviceProxy.getFilteredNewOptions("", optionCode, boxSize).ToList().Count > 0)
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
                }
                catch (Exception e)
                {
                    informationText = "There was a problem accessing the database";
                    Console.WriteLine(e);
                }
            }
            */
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

            /*
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
    */
            return complete;
        }
        #endregion
    }
}
