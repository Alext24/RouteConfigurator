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
using System.Windows;

namespace RouteConfigurator.ViewModel
{
    public class AddModelPopupModel : ViewModelBase
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

        private string _modelNum;
        private string _boxSize;
        private decimal _driveTime;
        private decimal _AVTime;

        private bool _modelEntered = false;
        private bool _boxSizeEntered = false;
        private bool _driveTimeEntered = false;
        private bool _AVTimeEntered = false;

        private string _modelTimeText;
        private string _totalTimeText;
        private string _routeText;
        private string _prodSupCodeText;

        private ObservableCollection<Option> _options = new ObservableCollection<Option>();

        private string _informationText;
        #endregion

        #region RelayCommands
        public RelayCommand submitCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public AddModelPopupModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            submitCommand = new RelayCommand(submit);
        }
        #endregion

        #region Commands
        private void submit()
        {
            MessageBox.Show("Hi\nPlaceholder for sending model to director");
        }
        #endregion

        #region Public Variables
        /// <summary>
        /// Updates modelEntered boolean if not null
        /// Calls updateOptions if model and box size has been entered
        /// Calls checkForInfo
        /// </summary>
        public string modelNum
        {
            get
            {
                return _modelNum;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _modelEntered = true;
                }
                else
                {
                    _modelEntered = false;
                }

                _modelNum = value.ToUpper();
                RaisePropertyChanged("modelNum");

                if (_modelEntered && _boxSizeEntered)
                    updateOptions();

                checkForInfo();
            }
        }

        /// <summary>
        /// Updates boxSizeEntered boolean if not null
        /// Calls updateOptions if model and box size has been entered
        /// Calls checkForInfo
        /// </summary>
        public string boxSize
        {
            get
            {
                return _boxSize;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _boxSizeEntered = true;
                }
                else
                {
                    _boxSizeEntered = false;
                }

                _boxSize = value.ToUpper();
                RaisePropertyChanged("boxSize");

                if (_modelEntered && _boxSizeEntered)
                    updateOptions();

                checkForInfo();
            }
        }

        /// <summary>
        /// Updates driveTimeEntered boolean if not null
        /// Calls checkForInfo
        /// </summary>
        public decimal driveTime
        {
            get
            {
                return _driveTime;
            }
            set
            {
                if (value > 0)
                {
                    _driveTimeEntered = true;
                }
                else
                {
                    _driveTimeEntered = false;
                }

                _driveTime = value;
                RaisePropertyChanged("driveTime");
                checkForInfo();
            }
        }

        /// <summary>
        /// Updates AVTimeEntered boolean if not null
        /// Calls checkForInfo
        /// </summary>
        public decimal AVTime
        {
            get
            {
                return _AVTime;
            }
            set
            {
                if (value > 0)
                {
                    _AVTimeEntered = true;
                }
                else
                {
                    _AVTimeEntered = false;
                }

                _AVTime = value;
                RaisePropertyChanged("AVTime");
                checkForInfo();
            }
        }

        public ObservableCollection<Option> options
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value;
                RaisePropertyChanged("options");
            }

        }

        public string modelTimeText
        {
            get
            {
                return _modelTimeText;
            }
            set
            {
                _modelTimeText = value;
                RaisePropertyChanged("modelTimeText");
            }
        }

        public string totalTimeText
        {
            get
            {
                return _totalTimeText;
            }
            set
            {
                _totalTimeText = value;
                RaisePropertyChanged("totalTimeText");
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
        /// Breaks up the model number to isolate the options
        /// Searches and returns the options that are part of the entered model
        /// Calls parseOptions
        /// </summary>
        private void updateOptions()
        {
            List<String> optionsList = parseOptions(modelNum);

            options = _serviceProxy.getModelOptions(optionsList, boxSize);
        }

        /// <summary>
        /// Takes the combined options from the model number and splits
        /// them into seperate options.
        /// </summary>
        /// <param name="model"> full model number entered</param>
        /// <returns> list of options that are a part of the model</returns>
        private List<string> parseOptions(string model)
        {
            List<String> optionsList = new List<string>();
            string options = model.Substring(8);

            bool isPower = false;
            bool isControl = false;
            foreach(char c in options)
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

            return optionsList;
        }

        /// <summary>
        /// Checks if all fields have been entered
        /// Calls updateInformation if all fields are entered
        /// </summary>
        private void checkForInfo()
        {
            if(_modelEntered && _boxSizeEntered && _driveTimeEntered && _AVTimeEntered)
            {
                updateInformation();
            }
        }

        /// <summary>
        /// Updates the modle time, total time, route, and product supervisor code for the model
        /// Calls setRoute and setProdSupCode
        /// </summary>
        private void updateInformation()
        {
            decimal modelTime = driveTime + AVTime;

            TimeSpan time = TimeSpan.FromHours((double)modelTime);
            modelTimeText = string.Format("{0}:{1:00}", ((time.Days*24) + time.Hours), time.Minutes);

            decimal totalTime = modelTime;
            foreach(Option option in options)
            {
                totalTime += option.Time;
            }

            time = TimeSpan.FromHours((double)totalTime);
            totalTimeText = string.Format("{0}:{1:00}", ((time.Days*24) + time.Hours), time.Minutes);

            setRoute(time);

            setProdSupCode(totalTime);
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
                //      0 if minutes < 30; 1 if > 30,
                //      extra 2 digits for unique route if necessary

                routeText = "501";

                string hoursText = string.Format("{0:00}", time.Hours);
                routeText = string.Concat(routeText, hoursText);

                string minutesText = "0";
                if(time.Minutes >= 30)
                {
                    minutesText = "1";
                }
                routeText = string.Concat(routeText, minutesText);

                routeText = string.Concat(routeText, "00");
            }
        }

        #endregion
    }
}
