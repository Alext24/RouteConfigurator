﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Model.EF_StandardModels;
using RouteConfigurator.Services;
using RouteConfigurator.Services.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RouteConfigurator.ViewModel.StandardModelViewModel
{
    public class OverrideModelPopupModel : ViewModelBase
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

        //Inputs
        private string _modelText = "";
        private StandardModel _model;

        private decimal? _overrideTime;
        private int? _overrideRoute;
        private string _description;

        //Model information for user reference
        private decimal? _modelTime;
        private string _modelRoute;
        private int _modelRouteInt;

        //Current list of overrides to submit
        private ObservableCollection<OverrideRequest> _overridesToSubmit = new ObservableCollection<OverrideRequest>();

        private string _informationText;

        private bool _loading = false;
        #endregion

        #region RelayCommands
        public RelayCommand addOverrideCommand { get; set; }
        public RelayCommand submitCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public OverrideModelPopupModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            addOverrideCommand = new RelayCommand(addOverrideAsync);
            submitCommand = new RelayCommand(submitAsync);
        }
        #endregion

        #region Commands
        private async void addOverrideAsync()
        {
            loading = true;
            await Task.Run(() => addOverride());
            loading = false;
        }

        /// <summary>
        /// Adds the information as an override to the ready to submit list
        /// </summary>
        private void addOverride()
        {
            if (checkValid())
            {
                OverrideRequest ov = new OverrideRequest()
                {
                    RequestDate = DateTime.Now,
                    ModelNum = modelText,
                    Description = string.IsNullOrWhiteSpace(description) ? "no description entered" : description,
                    State = 0,
                    Sender = string.Format("{0} {1}", _navigationService.user.FirstName, _navigationService.user.LastName),
                    OverrideTime = (decimal)overrideTime,
                    OverrideRoute = (int)overrideRoute,
                    ModelTime = (decimal)modelTime,
                    ModelRoute = _modelRouteInt,
                    ModelBase = model.Base,

                    ReviewDate = new DateTime(1900, 1, 1),
                    Reviewer = ""
                };

                // Since the observable collection was created on the UI thread 
                // we have to add the override to the list using a delegate function.
                App.Current.Dispatcher.Invoke(delegate
                {
                    overridesToSubmit.Add(ov);
                });
            }
        }

        private async void submitAsync()
        {
            loading = true;
            await Task.Run(() => submit());
            loading = false;
        }

        /// <summary>
        /// Submits each of the override requests to the database
        /// </summary>
        private void submit()
        {
            informationText = "";

            if (overridesToSubmit.Count > 0)
            {
                try
                {
                    informationText = "Submitting overrides...";
                    foreach (OverrideRequest ov in overridesToSubmit)
                    {
                        _serviceProxy.addOverrideRequest(ov);
                    }
                }
                catch (Exception e)
                {
                    informationText = "There was a problem accessing the database";
                    Console.WriteLine(e);
                    return;
                }

                //Clear input boxes
                _modelText = "";
                RaisePropertyChanged("modelText");

                overrideTime = null; 
                overrideRoute = null;
                modelTime = null;
                modelRoute = null;
                description = "";
                overridesToSubmit = new ObservableCollection<OverrideRequest>();

                informationText = "Overrides have been submitted.  Waiting for managerapproval.";
            }
            else
            {
                informationText = "No overrides to submit.";
            }
        }
        #endregion

        #region Public Variables
        /// <summary>
        /// Updates the model if the text has 8 or more characters
        /// Calls findModelAsync
        /// </summary>
        public string modelText
        {
            get
            {
                return _modelText;
            }
            set
            {
                _modelText = value.ToUpper();
                RaisePropertyChanged("modelText");

                if(modelText.Length >= 8)
                {
                    findModelAsync();
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(modelText))
                    {
                        informationText = "Invalid model format";
                    }
                    model = null;
                    modelTime = null;
                    modelRoute = "";
                }
            }
        }

        public StandardModel model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
                RaisePropertyChanged("model");
            }
        }

        public decimal? overrideTime
        {
            get
            {
                return _overrideTime;
            }
            set
            {
                _overrideTime = value;
                RaisePropertyChanged("overrideTime");
                informationText = "";
            }
        }

        public int? overrideRoute 
        {
            get
            {
                return _overrideRoute;
            }
            set
            {
                _overrideRoute = value;
                RaisePropertyChanged("overrideRoute");
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

        public decimal? modelTime
        {
            get
            {
                return _modelTime;
            }
            set
            {
                _modelTime = value;
                RaisePropertyChanged("modelTime");
            }
        }

        /// <summary>
        /// Updates _modelRouteInt
        /// </summary>
        public string modelRoute 
        {
            get
            {
                return _modelRoute;
            }
            set
            {
                _modelRoute = value;
                RaisePropertyChanged("modelRoute");
                if (string.IsNullOrWhiteSpace(value))
                {
                    _modelRouteInt = 0;
                }
                else
                {
                    try
                    {
                        _modelRouteInt = int.Parse(modelRoute);
                    }
                    catch (Exception)
                    {
                        modelRoute = null;
                        informationText = "Problem with model route";
                    }
                }
            }
        }

        public ObservableCollection<OverrideRequest> overridesToSubmit 
        {
            get
            {
                return _overridesToSubmit;
            }
            set
            {
                _overridesToSubmit = value;
                RaisePropertyChanged("overridesToSubmit");
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
        private async void findModelAsync()
        {
            loading = true;
            await Task.Run(() => findModel());
            loading = false;
        }

        /// <summary>
        /// Finds the model in the database associated with the modelText entered
        /// and then updates the information
        /// Calls updateModelTime and updateModelRoute
        /// </summary>
        private void findModel()
        {
            try
            {
                informationText = "Searching for model...";
                model = _serviceProxy.getModel(modelText.Substring(0, 8));
                if (model != null)
                {
                    updateModelTime();
                    updateModelRoute();
                    informationText = "";
                }
                else
                {
                    informationText = "Model does not exist.";
                }
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Updates the model time with the calculated time for the model
        /// Calls parseOptions
        /// </summary>
        private void updateModelTime()
        {
            decimal totalTime = 0;
            totalTime += model.DriveTime;
            totalTime += model.AVTime;

            List<string> options = parseOptions();
            if(options.Count > 0)
            {
                string errorText = "";
                bool missedOption = false;
                bool foundOption;

                try
                {
                    foreach (string option in options)
                    {
                        //Check to see if the option is in the database
                        foundOption = false;
                        if (_serviceProxy.getFilteredOptions(option, model.BoxSize, true).ToList().Count == 1)
                        {
                            foundOption = true;
                        }

                        if (!foundOption)
                        {
                            missedOption = true;
                            errorText += string.Format("Option {0} not found\n", option);
                        }
                    }

                    totalTime += _serviceProxy.getTotalOptionsTime(model.BoxSize, options);

                    if (missedOption)
                    {
                        errorText += "Information may be inaccurate.";
                        MessageBox.Show(errorText);
                    }
                }
                catch (Exception e)
                {
                    totalTime = 0;
                    informationText = "There was a problem accessing the database.";
                    Console.WriteLine(e.Message);
                }
            }

            modelTime = totalTime;
        }

        /// <summary>
        /// Parses the model and creates a list of the options
        /// </summary>
        /// <returns> returns a list of the options </returns>
        private List<string> parseOptions()
        {
            List<string> optionsList = new List<string>();

            if (modelText.Length > 8)
            {
                string options = modelText.Substring(8);

                bool isPower = false;
                bool isControl = false;
                foreach (char c in options)
                {
                    if (c.Equals('P'))
                    {
                        isPower = true;
                        isControl = false;
                    }
                    else if (c.Equals('T'))
                    {
                        isControl = true;
                        isPower = false;
                    }
                    else if (c.Equals('S'))
                    {
                        //Ignoring software options
                        break;
                    }
                    else
                    {
                        if (isPower)
                        {
                            optionsList.Add(string.Format("P{0}", c));
                        }
                        else if (isControl)
                        {
                            optionsList.Add(string.Format("T{0}", c));
                        }
                    }
                }
            }
            return optionsList;
        }

        /// <summary>
        /// Updates the model route based on the model time
        /// </summary>
        private void updateModelRoute()
        {
            TimeSpan time = TimeSpan.FromHours((double)modelTime);
            string route = "";

            if (time.TotalMinutes <= 0)
            {
                route = "0";
            }
            else
            {
                //Format for route is "501",
                //      2 digit hour,
                //      0 if minutes < 30; 1 if > 30,
                //      extra 2 digits for unique route if necessary

                route = "501";

                decimal hours = (time.Days * 24 + time.Hours);
                string hoursText = "";
                if (hours >= 100)
                {
                    hoursText = "999";
                    route = string.Concat(route, hoursText);
                }
                else
                {
                    hoursText = string.Format("{0:00}", (time.Days * 24 + time.Hours));
                    route = string.Concat(route, hoursText);

                    string minutesText = "0";
                    if (time.Minutes >= 30)
                    {
                        minutesText = "1";
                    }
                    route = string.Concat(route, minutesText);
                }

                route = string.Concat(route, "00");
            }
            modelRoute = route;
        }

        /// <summary>
        /// Ensures the model override is not a duplicate in the ready to submit list
        /// Calls checkComplete
        /// </summary>
        /// <remarks> if an override request gets accepted by the manager and the model is
        /// already overriden, the old override will be deleted and the new information
        /// will be associated with the model </remarks>
        /// <returns> true if the override is valid and doesn't already exist, false otherwise </returns>
        private bool checkValid()
        {
            bool valid = checkComplete();

            if (valid)
            {
                //Check if the option is a duplicate in the ready to submit list
                foreach (OverrideRequest ov in overridesToSubmit)
                {
                    if (modelText.Equals(ov.ModelNum))
                    {
                        informationText = string.Format("Override for model {0} is already ready to submit", modelText);
                        valid = false;
                    }
                }
            }
            return valid;
        }

        /// <summary>
        /// Checks to see if all necessary fields are filled out before the override
        /// can be added.  
        /// </summary>
        /// <returns> true if the form is complete, otherwise false </returns>
        private bool checkComplete()
        {
            bool complete = true;

            // Model needs 4 characters for the drive and 4 characters for voltage and amperage
            if (!string.IsNullOrWhiteSpace(modelText) && modelText.Length >= 8)
            {
                if(model == null)
                {
                    complete = false;
                    informationText = "Model does not exist, enter a different model";
                }
                else if(complete && overrideTime == null || overrideTime <= 0)
                {
                    complete = false;
                    informationText = "Invalid override time";
                }
                else if (complete && overrideRoute == null || overrideRoute <= 0)
                {
                    complete = false;
                    informationText = "Invalid override route";
                }
            }
            else
            {
                //If user has entered less than 8 characters for the model
                if (!string.IsNullOrWhiteSpace(modelText))
                {
                    informationText = "Invalid model format";
                }
                complete = false;
            }

            return complete;
        }
        #endregion
    }
}