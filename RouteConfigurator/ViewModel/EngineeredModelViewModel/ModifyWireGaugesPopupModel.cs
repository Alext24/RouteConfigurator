﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Model.EF_EngineeredModels;
using RouteConfigurator.Services;
using RouteConfigurator.Services.Interface;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RouteConfigurator.ViewModel.EngineeredModelViewModel
{
    public class ModifyWireGaugesPopupModel : ViewModelBase
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

        public ObservableCollection<WireGauge> _wireGauges = new ObservableCollection<WireGauge>();
        private WireGauge _wireGauge;

        private decimal? _newTimePercentage;
        private string _description;

        /// <summary>
        /// All wire gauges found that meet the filters
        /// These are the wire gauges that will be modified when submitted
        /// </summary>
        /// <remarks> Since the filter is a combo box for the wire gauge, only
        ///           one wire gauge can be modified at a time.  This list is
        ///           used to display that wire gauge in a datagrid. </remarks>
        private ObservableCollection<WireGauge> _wireGaugesFound = new ObservableCollection<WireGauge>();

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
        public ModifyWireGaugesPopupModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            loadedCommand = new RelayCommand(loaded);
            submitCommand = new RelayCommand(submitAsync);
        }
        #endregion

        #region Commands
        private async void loadedAsync()
        {
            loading = true;
            await Task.Run(() => loaded());
            loading = false;
        }

        /// <summary>
        /// Loads the information for the page
        /// </summary>
        private void loaded()
        {
            informationText = "Loading information...";
            try
            {
                wireGauges = new ObservableCollection<WireGauge>(_serviceProxy.getWireGauges());
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
            informationText = "";
        }

        private async void submitAsync()
        {
            loading = true;
            await Task.Run(() => submit());
            loading = false;
        }

        /// <summary>
        /// Submits the wire guage modification to the database
        /// </summary>
        private void submit()
        {
            informationText = "";

            if (wireGaugesFound.Count <= 0)
            {
                informationText = "No wire guages selected to update.";
            }
            else if (checkComplete())
            {
                try
                {
                    informationText = "Submitting wire gauge modifications...";

                    //Create a new modification for each wire gauge in the list
                    //Should only be one
                    foreach (WireGauge gauge in wireGaugesFound)
                    {
                        EngineeredModification modifiedGauge = new EngineeredModification()
                        {
                            RequestDate = DateTime.Now,
                            ReviewedDate = new DateTime(1900, 1, 1),
                            Description = string.IsNullOrWhiteSpace(description) ? "no description entered" : description,
                            State = 0,
                            Sender = string.Format("{0} {1}", _navigationService.user.FirstName, _navigationService.user.LastName),
                            Reviewer = "",
                            IsNew = false,
                            ComponentName = "",
                            EnclosureSize = "",
                            EnclosureType = "",
                            NewTime = 0,
                            OldTime = 0,
                            Gauge = gauge.Gauge,
                            NewTimePercentage = (decimal)newTimePercentage,
                            OldTimePercentage = gauge.TimePercentage
                        };
                        _serviceProxy.addEngineeredModificationRequest(modifiedGauge);
                    }

                    //Clear input boxes
                    _wireGauge = null;
                    RaisePropertyChanged("wireGauge");

                    wireGaugesFound = new ObservableCollection<WireGauge>();
                    newTimePercentage = null;
                    description = "";

                    informationText = "Wire gauge modification has been submitted.  Waiting for manager approval.";
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
        public ObservableCollection<WireGauge> wireGauges 
        {
            get
            {
                return _wireGauges;
            }
            set
            {
                _wireGauges = value;
                RaisePropertyChanged("wireGauges");
            }
        }

        /// <summary>
        /// Calls updateWireGaugeTableAsync
        /// </summary>
        public WireGauge wireGauge
        {
            get
            {
                return _wireGauge;
            }
            set
            {
                _wireGauge = value;
                RaisePropertyChanged("wireGauge");
                informationText = "";

                updateWireGaugeTableAsync();
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

        public ObservableCollection<WireGauge> wireGaugesFound 
        {
            get
            {
                return _wireGaugesFound;
            }
            set
            {
                _wireGaugesFound = value;
                RaisePropertyChanged("wireGaugesFound");
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
        private async void updateWireGaugeTableAsync()
        {
            loading = true;
            await Task.Run(() => updateWireGaugeTable());
            loading = false;
        }

        /// <summary>
        /// Updates the wire gauge table with the selected wire gauge
        /// </summary>
        private void updateWireGaugeTable()
        {
            //Reset the list
            wireGaugesFound = new ObservableCollection<WireGauge>();

            if(wireGauge != null)
            {
                wireGaugesFound.Add(wireGauge);
            }
        }

        /// <summary>
        /// Checks to see if all necessary fields are filled out with correct formatting
        /// before the wire gauge modification can be added.
        /// </summary>
        /// <returns> true if the form is complete, otherwise false</returns>
        private bool checkComplete()
        {
            bool complete = true;

            if(newTimePercentage == null || newTimePercentage <= 0)
            {
                informationText = "No new information associated with modification.";
                complete = false;
            }
            return complete;
        }
        #endregion
    }
}
