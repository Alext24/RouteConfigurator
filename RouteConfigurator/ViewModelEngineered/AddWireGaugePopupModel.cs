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
    public class AddWireGaugePopupModel : ViewModelBase
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

        private string _wireGauge;
        private decimal? _newTimePercentage;
        private string _description;

        private ObservableCollection<EngineeredModification> _modificationsToSubmit = new ObservableCollection<EngineeredModification>();

        private string _informationText;

        private bool _loading = false;
        #endregion

        #region RelayCommands
        public RelayCommand loadedCommand { get; set; }
        public RelayCommand addWireGaugeCommand { get; set; }
        public RelayCommand submitCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public AddWireGaugePopupModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            loadedCommand = new RelayCommand(loaded);
            addWireGaugeCommand = new RelayCommand(addWireGaugeAsync);
            submitCommand = new RelayCommand(submitAsync);
        }
        #endregion

        #region Commands
        private void loaded()
        {
        }

        private async void addWireGaugeAsync()
        {
            loading = true;
            await Task.Run(() => addWireGauge());
            loading = false;
        }

        /// <summary>
        /// Creates the new component modification and adds it to the modifications to submit list
        /// Calls checkValid
        /// </summary>
        private void addWireGauge()
        {
            informationText = "";

            if (checkValid())
            {
                informationText = "Adding wire gauge...";
                EngineeredModification newWireGauge = new EngineeredModification()
                {
                    RequestDate = DateTime.Now,
                    ReviewedDate = new DateTime(1900, 1, 1),
                    Description = string.IsNullOrWhiteSpace(description) ? "no description entered" : description,
                    State = 0,
                    Sender = "TEMPORARY SENDER",
                    Reviewer = "",
                    IsNew = true,
                    ComponentName = "",
                    EnclosureSize = "",
                    EnclosureType = "",
                    NewTime = 0,
                    OldTime = 0,
                    Gauge = wireGauge,
                    NewTimePercentage = (decimal)newTimePercentage,
                    OldTimePercentage = 0
                };

                // Since the observable collection was created on the UI thread 
                // we have to add the override to the list using a delegate function.
                App.Current.Dispatcher.Invoke(delegate
                {
                    modificationsToSubmit.Add(newWireGauge);
                });

                //Clear input boxes
                wireGauge = "";
                newTimePercentage = null;
                informationText = "Wire Gauge has been submitted.  Waiting for manager approval.";
            }
        }

        private async void submitAsync()
        {
            loading = true;
            await Task.Run(() => submit());
            loading = false;
        }

        /// <summary>
        /// Submits each of the new component modifications to the database
        /// </summary>
        private void submit()
        {
            informationText = "";

            if (modificationsToSubmit.Count > 0)
            {
                try
                {
                    informationText = "Submitting wire gauge modifications...";
                    foreach (EngineeredModification mod in modificationsToSubmit)
                    {
                        _serviceProxy.addEngineeredModificationRequest(mod);
                    }
                }
                catch (Exception e)
                {
                    informationText = "There was a problem accessing the database";
                    Console.WriteLine(e);
                    return;
                }
                //Clear input boxes
                wireGauge = "";
                newTimePercentage = null;
                description = "";

                modificationsToSubmit = new ObservableCollection<EngineeredModification>();

                informationText = "Wire Gauges have been submitted.  Waiting for manager approval.";
            }
            else
            {
                informationText = "No wire gauges to submit.";
            }
        }
        #endregion

        #region Public Variables
        public string wireGauge 
        {
            get
            {
                return _wireGauge;
            }
            set
            {
                _wireGauge = value.ToUpper();
                RaisePropertyChanged("wireGauge");
                informationText = "";
            }
        }

        public decimal? newTimePercentage
        {
            get
            {
                return _newTimePercentage;
            }
            set
            {
                _newTimePercentage = value;
                RaisePropertyChanged("newTimePercentage");
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
        /// Checks that the wire gauge does not already exist
        /// Calls checkComplete
        /// </summary>
        /// <returns> true if the wire gauge is valid and doesn't already exist, false otherwise </returns>
        private bool checkValid()
        {
            bool valid = checkComplete();
            if (valid)
            {
                try
                {
                    //Check if the wire gauge already exists in the database as a wire gauge
                    if (_serviceProxy.getFilteredWireGauges(wireGauge).ToList().Count > 0)
                    {
                        informationText = "This wire gauge already exists";
                        valid = false;
                    }
                    //Check if the wire gauge already exists in the database as a new wire gauge request
                    else if (_serviceProxy.getNewWireGaugeMods(wireGauge).ToList().Count > 0)
                    {
                        informationText = "wire gauge is already waiting for approval.";
                        valid = false;
                    }
                    else
                    {
                        //Check if the wire gauge is a duplicate in the ready to submit list
                        foreach (EngineeredModification newWireGauge in modificationsToSubmit)
                        {
                            if (newWireGauge.Gauge.Equals(wireGauge))
                            {
                                informationText = "This wire gauge is already ready to submit";
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
        /// before the component can be added.
        /// </summary>
        /// <returns> true if the form is complete, otherwise false</returns>
        private bool checkComplete()
        {
            bool complete = true;

            if (string.IsNullOrWhiteSpace(wireGauge))
            {
                complete = false;
                informationText = "Enter a wire gauge.";
            }
            else if (newTimePercentage == null || newTimePercentage <= 0)
            {
                complete = false;
                informationText = "Enter a valid time percentage.";
            }

            return complete;
        }
        #endregion
    }
}
