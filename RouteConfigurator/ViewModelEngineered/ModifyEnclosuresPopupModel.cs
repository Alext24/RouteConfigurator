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
    public class ModifyEnclosuresPopupModel : ViewModelBase
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

        public ObservableCollection<string> _enclosureTypes = new ObservableCollection<string>();
        private string _enclosureType = "";
        public ObservableCollection<string> _enclosureSizes = new ObservableCollection<string>();
        private string _enclosureSize = "";
        private decimal? _newTime;
        private string _description;

        private ObservableCollection<Enclosure> _enclosuresFound = new ObservableCollection<Enclosure>();

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
        public ModifyEnclosuresPopupModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            loadedCommand = new RelayCommand(loaded);
            submitCommand = new RelayCommand(submitAsync);
        }
        #endregion

        #region Commands
        private void loaded()
        {
            enclosureTypes = new ObservableCollection<string>(_serviceProxy.getEnclosureTypes());
            enclosureSizes = new ObservableCollection<string>(_serviceProxy.getEnclosureSizes());
        }

        private async void submitAsync()
        {
            loading = true;
            await Task.Run(() => submit());
            loading = false;
        }

        /// <summary>
        /// Submits each of the enclosure modifications to the database
        /// </summary>
        private void submit()
        {
            informationText = "";

            if (enclosuresFound.Count <= 0)
            {
                informationText = "No enclosures selected to update.";
            }
            else if (checkComplete())
            {
                try
                {
                    foreach (Enclosure enclosure in enclosuresFound)
                    {
                        EngineeredModification modifiedEnclosure = new EngineeredModification()
                        {
                            RequestDate = DateTime.Now,
                            ReviewedDate = new DateTime(1900, 1, 1),
                            Description = string.IsNullOrWhiteSpace(description) ? "no description entered" : description,
                            State = 0,
                            Sender = "TEMPORARY SENDER",
                            Reviewer = "",
                            IsNew = false,
                            ComponentName = "",
                            EnclosureSize = enclosure.EnclosureSize,
                            EnclosureType = enclosure.EnclosureType,
                            NewTime = (decimal)newTime,
                            OldTime = enclosure.Time,
                            Gauge = "",
                            NewTimePercentage = 0,
                            OldTimePercentage = 0
                        };
                        _serviceProxy.addEngineeredModificationRequest(modifiedEnclosure);
                    }

                    //Clear input boxes
                    _enclosureType = "";
                    RaisePropertyChanged("enclosureType");
                    _enclosureSize = ""; 
                    RaisePropertyChanged("enclosureSize");

                    enclosuresFound = new ObservableCollection<Enclosure>();
                    newTime = null;
                    description = "";

                    informationText = "Enclosure modifications have been submitted.  Waiting for manager approval.";
                }
                catch (Exception e)
                {
                    informationText = "There was a problem accessing the database";
                    Console.WriteLine(e);
                }
            }
        }
        #endregion

        #region Public Variables
        public ObservableCollection<string> enclosureTypes 
        {
            get
            {
                return _enclosureTypes;
            }
            set
            {
                _enclosureTypes = value;
                RaisePropertyChanged("enclosureTypes");
            }
        }

        /// <summary>
        /// Calls updateEnclosureTableAsync
        /// </summary>
        public string enclosureType 
        {
            get
            {
                return _enclosureType;
            }
            set
            {
                _enclosureType = value.ToUpper();
                RaisePropertyChanged("enclosureType");
                informationText = "";

                updateEnclosureTableAsync();
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

        /// <summary>
        /// Calls updateEnclosureTableAsync
        /// </summary>
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

                updateEnclosureTableAsync();
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

        public ObservableCollection<Enclosure> enclosuresFound 
        {
            get
            {
                return _enclosuresFound;
            }
            set
            {
                _enclosuresFound = value;
                RaisePropertyChanged("enclosuresFound");
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
        private async void updateEnclosureTableAsync()
        {
            loading = true;
            await Task.Run(() => updateEnclosureTable());
            loading = false;
        }

        /// <summary>
        /// Updates the enclosure table with the filtered information
        /// </summary>
        private void updateEnclosureTable()
        {
            if(string.IsNullOrWhiteSpace(enclosureType) && string.IsNullOrWhiteSpace(enclosureSize))
            {
                enclosuresFound = new ObservableCollection<Enclosure>();
            }
            else
            {
                try
                {
                    informationText = "Loading components...";
                    enclosuresFound = new ObservableCollection<Enclosure>(_serviceProxy.getFilteredEnclosures(enclosureType, enclosureSize));
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
        /// Checks to see if all necessary fields are filled out with correct formatting
        /// before the option can be added.
        /// </summary>
        /// <returns> true if the form is complete, otherwise false</returns>
        private bool checkComplete()
        {
            bool complete = true;

            if(newTime == null || newTime <= 0)
            {
                informationText = "No new information associated with modification.";
                complete = false;
            }
            return complete;
        }
        #endregion
    }
}
