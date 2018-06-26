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

namespace RouteConfigurator.ViewModel
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

        private string _modelNumber;
        private string _routeText;
        private string _prodSupCodeText;
        private string _productionTimeText;
        private string _timeSearchModelNumber;

        private ObservableCollection<TimeTrial> _timeTrials = new ObservableCollection<TimeTrial>();
        private decimal _averageTime;

        private TimeTrial _selectedTimeTrial;

        private string _informationText;

        private Model.Model _model;
        #endregion

        #region RelayCommands
        public RelayCommand timeSearchCommand { get; set; }
        public RelayCommand supervisorLoginCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public HomeViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            timeSearchCommand = new RelayCommand(timeSearch);
            supervisorLoginCommand = new RelayCommand(supervisorLogin);
        }
        #endregion

        #region Commands
        /// <summary>
        /// see searchTimeTrials
        /// </summary>
        private void timeSearch()
        {
            searchTimeTrials(timeSearchModelNumber);
        }

        private void supervisorLogin()
        {
            informationText = "Hello Mr. Supervisor";
//            _navigationService.NavigateTo("DaysView", _day);
        }
        #endregion

        #region Public Variables
        public string modelNumber 
        {
            get
            {
                return _modelNumber;
            }
            set
            {
                _modelNumber = value.ToUpper();
                timeSearchModelNumber = modelNumber;
                RaisePropertyChanged("modelNumber");
                searchModel();
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

        public string productionTimeText
        {
            get
            {
                return _productionTimeText;
            }
            set
            {
                _productionTimeText = value;
                RaisePropertyChanged("productionTimeText");
            }
        }

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
                searchTimeTrials(timeSearchModelNumber);
            }
        }

        public Model.Model model
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

        public decimal averageTime
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
        #endregion

        #region Private Functions

        /// <summary>
        /// Looks for and updates the information for the entered model number
        /// </summary>
        private void searchModel()
        {
            try
            {
                //Retrieves a model from the database
                model = _serviceProxy.getModel(modelNumber);
                routeText = string.Format("{0}", model.RouteNum);
                productionTimeText =  string.Format("{0}", model.TotalTime);
                setProdSupCode();

                informationText = "Model loaded";
            }catch(Exception e)
            {
                informationText = e.Message;

                routeText = string.Format("N/A");
                prodSupCodeText = string.Format("N/A");
                productionTimeText =  string.Format("N/A");
            }
        }

        /// <summary>
        /// Looks for time trials for the given model
        /// </summary>
        /// <param name="model"> model number to search for </param>
        private void searchTimeTrials(string model)
        {
            if (!string.IsNullOrWhiteSpace(model))
            {
                //Retrieve timeTrials from the database
                timeTrials = _serviceProxy.getTimeTrials(model);

                if (timeTrials.Count() > 0)
                {
                    informationText = "Time trials loaded";
                    calcAverageTime();
                }
                else
                {
                    informationText = "No time trials for this model exist";
                }
            }
            else
            {
                //Reset the information since there is not a model name
                timeTrials.Clear();
                averageTime = 0;
                informationText = "Please enter model number";
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
        /// Determines the product supervisor code based off the production time
        /// </summary>
        private void setProdSupCode()
        {
            if(model.TotalTime <= 0)
            {
                prodSupCodeText = "";
            }
            else if(model.TotalTime > 0 && model.TotalTime < 1.50M)
            {
                prodSupCodeText = "3";
            }
            else if(model.TotalTime >= 1.50M && model.TotalTime < 2.50M)
            {
                prodSupCodeText = "4";
            }
            else if(model.TotalTime >= 2.50M && model.TotalTime < 4.00M)
            {
                prodSupCodeText = "5";
            }
            else if(model.TotalTime >= 4.00M && model.TotalTime < 5.00M)
            {
                prodSupCodeText = "6";
            }
            else if(model.TotalTime >= 5.00M && model.TotalTime < 9.00M)
            {
                prodSupCodeText = "7";
            }
            else if(model.TotalTime >= 9.00M && model.TotalTime < 15.00M)
            {
                prodSupCodeText = "8";
            }
            else if(model.TotalTime >= 15.00M && model.TotalTime < 20.00M)
            {
                prodSupCodeText = "9";
            }
            else if(model.TotalTime >= 20.00M && model.TotalTime < 30.00M)
            {
                prodSupCodeText = "10";
            }
            else if(model.TotalTime >= 30.00M && model.TotalTime < 40.00M)
            {
                prodSupCodeText = "11";
            }
            else if(model.TotalTime >= 40.00M)
            {
                prodSupCodeText = "12";
            }
            else
            {
                prodSupCodeText = "error";
            }
        }

        #endregion
    }
}
