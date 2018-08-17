using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Model;
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
    public class HomeViewModel : ViewModelBase
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

        /// <summary>
        /// Full model number.  Ex. A1C1A002PABTXY
        /// User entered
        /// </summary>
        private string _modelNumber;

        /// <summary>
        /// Model number without options.  Ex. A1C1A002
        /// Extracted from modelNumber
        /// </summary>
        private string _modelBase;

        /// <summary>
        /// Options portion of the model number.  Ex. PABTXY
        /// Extracted from modelNumber
        /// </summary>
        private string _options;

        /// <summary>
        /// List of options from model number.  Ex. PA, PB, TX, TY
        /// </summary>
        private List<string> _optionsList = new List<string>();

        private string _routeText;
        private string _prodSupCodeText;
        private decimal? _productionTime;
        private string _timeSearchModelNumber;

        private StandardModel _model;

        private TimeTrial _selectedTimeTrial;
        private ObservableCollection<TimeTrial> _timeTrials = new ObservableCollection<TimeTrial>();

        /// <summary>
        /// Average time for all of the time trials loaded for that model
        /// </summary>
        private decimal? _averageTime;

        private string _informationText;

        private bool _loading = false;
        #endregion

        #region RelayCommands
        public RelayCommand submitToQueueCommand { get; set; }
        public RelayCommand timeSearchCommand { get; set; }
        public RelayCommand supervisorLoginCommand { get; set; }
        public RelayCommand managerLoginCommand { get; set; }
        public RelayCommand routeQueueCommand { get; set; }
        public RelayCommand engineeredOrdersCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public HomeViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            submitToQueueCommand = new RelayCommand(submitToQueueAsync);
            timeSearchCommand = new RelayCommand(timeSearch);
            supervisorLoginCommand = new RelayCommand(supervisorLogin);
            managerLoginCommand = new RelayCommand(managerLogin);
            routeQueueCommand = new RelayCommand(routeQueue);
            engineeredOrdersCommand = new RelayCommand(engineeredOrders);
        }
        #endregion

        #region Commands
        private async void submitToQueueAsync()
        {
            await Task.Run(() => submitToQueue());
        }

        private void submitToQueue()
        {
            if (string.IsNullOrWhiteSpace(modelNumber))
            {
                informationText = "Enter a valid model before submitting route.";
            }
            else if (string.IsNullOrWhiteSpace(prodSupCodeText) || prodSupCodeText.Equals("N/A")) 
            {
                informationText = "Information is not valid. Cannot submit.";
            }
            else
            {
                try
                {
                    informationText = "Adding route to queue...";
                    RouteQueue route = new RouteQueue
                    {
                        Route = int.Parse(routeText),
                        ModelNumber = model.Base,
                        Line = model.Line,
                        TotalTime = (decimal)productionTime,
                        IsApproved = false,
                        AddedDate = DateTime.Now,
                        SubmittedDate = new DateTime(1900, 1, 1)
                    };

                    _serviceProxy.addRouteQueue(route);

                    informationText = "Route added to queue.";
                }
                catch (Exception e)
                {
                    informationText = "There was a problem accessing the database.";
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Calls modelNumberSectionsAsync 
        /// </summary>
        private void timeSearch()
        {
            timeTrials.Clear();
            modelNumberSectionsAsync(timeSearchModelNumber);
        }

        private void supervisorLogin()
        {
            _navigationService.NavigateTo("SupervisorView", true);
        }

        private void managerLogin()
        {
            _navigationService.NavigateTo("ManagerView");
        }

        private void routeQueue()
        {
            _navigationService.NavigateTo("RouteQueueView");
        }

        private void engineeredOrders()
        {
            _navigationService.NavigateTo("EngineeredHomeView");
        }
        #endregion

        #region Public Variables
        /// <summary>
        /// Sets timeSearchModelNumber to the entered model number as well
        /// Calls searchModelAsync
        /// </summary>
        public string modelNumber 
        {
            get
            {
                return _modelNumber;
            }
            set
            {
                _modelNumber = value.ToUpper();
                RaisePropertyChanged("modelNumber");
                informationText = "";

                timeSearchModelNumber = modelNumber;
                searchModelAsync();
            }
        }

        public string routeText
        {
            get
            {
                return _routeText;
            }
            set
            {
                _routeText = value;
                RaisePropertyChanged("routeText");
            }
        }

        public string prodSupCodeText
        {
            get
            {
                return _prodSupCodeText;
            }
            set
            {
                _prodSupCodeText = value;
                RaisePropertyChanged("prodSupCodeText");
            }
        }

        public decimal? productionTime
        {
            get
            {
                return _productionTime;
            }
            set
            {
                _productionTime = value;
                RaisePropertyChanged("productionTime");
            }
        }

        /// <summary>
        /// Calls timeSearch
        /// </summary>
        public string timeSearchModelNumber 
        {
            get
            {
                return _timeSearchModelNumber;
            }
            set
            {
                _timeSearchModelNumber = value.ToUpper();
                RaisePropertyChanged("timeSearchModelNumber");
                informationText = "";

                timeSearch();
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

        public ObservableCollection<TimeTrial> timeTrials
        {
            get
            {
                return _timeTrials;
            }
            set
            {
                _timeTrials = value;
                RaisePropertyChanged("timeTrials");
            }
        }

        public decimal? averageTime
        {
            get
            {
                return _averageTime;
            }
            set
            {
                _averageTime = value;
                RaisePropertyChanged("averageTime");
            }
        }

        public TimeTrial selectedTimeTrial
        {
            get
            {
                return _selectedTimeTrial;
            }
            set
            {
                _selectedTimeTrial = value;
                RaisePropertyChanged("selectedTimeTrial");
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
        private async void modelNumberSectionsAsync(string model)
        {
            await Task.Run(() => modelNumberSections(model));
        }

        /// <summary>
        /// Breaks the model number into its corresponding sections
        /// Base(drive and AV) and options list
        /// Calls searchTimeTrials
        /// </summary>
        /// <param name="model"> full model number entered</param>
        private void modelNumberSections(string model)
        {
            //Assuming first 4 as drive and enclosure.  Ex. A1C1
            //Assuming second 4 as voltage and current.  Ex. D002
            //Assuming anything after are options
            //Power options will be precedded by P
            //Control options will be precedded by T
            //Software options will be precedded by S and ignored
            //The order for options will go Power, then Control, then Software

            _optionsList.Clear();

            if (string.IsNullOrWhiteSpace(model))
            {
                _modelBase = "";
                _options = "";
                averageTime = null;
            }
            else if(model.Length < 8)
            {
                informationText = "Invalid Model Format";
                _modelBase = "";
                _options = "";
                averageTime = null;
            }
            else
            {
                _modelBase = model.Substring(0, 8);
                _options = model.Substring(8);

                bool isPower = false;
                bool isControl = false;
                foreach(char c in _options)
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
                            _optionsList.Add(string.Format("P{0}", c));
                        }
                        else if (isControl)
                        {
                            _optionsList.Add(string.Format("T{0}", c));
                        }
                    }
                }
                searchTimeTrials(_modelBase, _optionsList);
            }
        }

        /// <summary>
        /// Looks for time trials for the given model
        /// Calls calcAverageTime
        /// </summary>
        /// <param name="model"> model number to search for </param>
        /// <param name="options"> list of options wanted for the model </param>
        private void searchTimeTrials(string model, List<string> options)
        {
            if (!string.IsNullOrWhiteSpace(model))
            {
                try
                {
                    //Retrieve timeTrials from the database
                    timeTrials = new ObservableCollection<TimeTrial>(_serviceProxy.getTimeTrials(model, options));

                    if (timeTrials.Count() > 0)
                    {
                        informationText = "Time trials loaded";
                        calcAverageTime();
                    }
                    else
                    {
                        averageTime = null;
                        informationText = "No time trials for this model exist";
                    }
                }catch(Exception e)
                {
                    informationText = "There was a problem accessing the database.";
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Calculates the average time for all of the time trials for the entered model
        /// </summary>
        private void calcAverageTime()
        {
            decimal sum = 0;
            decimal count = 0;

            foreach(TimeTrial timeTrial in timeTrials)
            {
                sum += timeTrial.TotalTime;
                count++;
            }

            averageTime = sum/count;
        }

        /// <summary>
        /// Calls searchModelAsync
        /// </summary>
        private async void searchModelAsync()
        {
            loading = true;
            await Task.Run(() => searchModel());
            loading = false;
        }

        /// <summary>
        /// Looks for and updates the information for the entered model number
        /// Calls updateModelText
        /// </summary>
        private void searchModel()
        {
            routeText = "";
            prodSupCodeText = "";
            productionTime = null;

            if (string.IsNullOrWhiteSpace(_modelBase))
            {
                return;
            }
            try
            {
                informationText = "Searching for model...";

                //Retrieves a model from the database
                model = _serviceProxy.getModel(_modelBase);

                if (model != null)
                {
                    informationText = "Calculating model time...";
                    updateModelText();

                    informationText = "Model loaded";
                }
                else
                {
                    informationText = "Model not found";

                    routeText = string.Format("N/A");
                    prodSupCodeText = string.Format("N/A");
                    productionTime = null;
                }
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database.";
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Updates the route, product supervisor code, and production time for the model
        /// calls getTotalTime, setRoute, and setProdSupCode
        /// </summary>
        private void updateModelText()
        {
            try
            {
                Override modelOverride = _serviceProxy.getModelOverride(modelNumber);

                if (modelOverride != null)
                {
                    //Model's information is currently overriden
                    routeText = modelOverride.OverrideRoute.ToString();

                    //TimeSpan time = TimeSpan.FromHours((double)modelOverride.OverrideTime);
                    //productionTimeText = string.Format("{0}:{1:00}", ((time.Days * 24) + time.Hours), time.Minutes);
                    productionTime = modelOverride.OverrideTime;

                    setProdSupCode(modelOverride.OverrideTime);
                }
                else
                {
                    //Model's information is not currently overriden
                    decimal totalTime = getTotalTime();

                    TimeSpan time = TimeSpan.FromHours((double)totalTime);
                    //productionTimeText = string.Format("{0}:{1:00}", ((time.Days * 24) + time.Hours), time.Minutes);
                    productionTime = totalTime;

                    setRoute(time);
                    setProdSupCode(totalTime);
                }
            }catch (Exception e)
            {
                informationText = "There was a problem accessing the database.";
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Sums the base time, AV time, and all option times for the model
        /// </summary>
        /// <returns> total production time for the model</returns>
        private decimal getTotalTime()
        {
            decimal totalTime = 0;
            totalTime += model.DriveTime;
            totalTime += model.AVTime;

            if (_optionsList.Count > 0)
            {
                string errorText = "";
                bool missedOption = false;
                bool foundOption;

                try
                {
                    foreach (string option in _optionsList)
                    {
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

                    totalTime += _serviceProxy.getTotalOptionsTime(model.BoxSize, _optionsList);

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

            return totalTime;
        }

        /// <summary>
        /// Determines the product supervisor code based off the production time
        /// </summary>
        /// <param name="time"> the production time for the model</param>
        private void setProdSupCode(decimal time)
        {
            if(time <= 0)
            {
                prodSupCodeText = "";
            }
            else if(time > 0 && time < 1.50M)
            {
                prodSupCodeText = "3";
            }
            else if(time >= 1.50M && time < 2.50M)
            {
                prodSupCodeText = "4";
            }
            else if(time >= 2.50M && time < 4.00M)
            {
                prodSupCodeText = "5";
            }
            else if(time >= 4.00M && time < 5.00M)
            {
                prodSupCodeText = "6";
            }
            else if(time >= 5.00M && time < 9.00M)
            {
                prodSupCodeText = "7";
            }
            else if(time >= 9.00M && time < 15.00M)
            {
                prodSupCodeText = "8";
            }
            else if(time >= 15.00M && time < 20.00M)
            {
                prodSupCodeText = "9";
            }
            else if(time >= 20.00M && time < 30.00M)
            {
                prodSupCodeText = "10";
            }
            else if(time >= 30.00M && time < 40.00M)
            {
                prodSupCodeText = "11";
            }
            else if(time >= 40.00M)
            {
                prodSupCodeText = "12";
            }
            else
            {
                prodSupCodeText = "error";
            }
        }

        /// <summary>
        /// Determines the route number based off the production time
        /// </summary>
        /// <param name="time"> the production time for the model</param>
        private void setRoute(TimeSpan time)
        {
            if (time.TotalMinutes <= 0)
            {
                routeText = "0";
            }
            else
            {
                //Format for route is "501",
                //      2 digit hour,
                //      0 if minutes < 30; 1 if >= 30,
                //      extra 2 digits for unique route if necessary

                routeText = "501";

                decimal hours = (time.Days * 24 + time.Hours);
                string hoursText = "";
                if (hours >= 100)
                {
                    hoursText = "999";
                    routeText = string.Concat(routeText, hoursText);
                }
                else
                {
                    hoursText = string.Format("{0:00}", (time.Days * 24 + time.Hours));
                    routeText = string.Concat(routeText, hoursText);

                    string minutesText = "0";
                    if (time.Minutes >= 30)
                    {
                        minutesText = "1";
                    }
                    routeText = string.Concat(routeText, minutesText);
                }

                routeText = string.Concat(routeText, "00");
            }
        }
        #endregion
    }
}
