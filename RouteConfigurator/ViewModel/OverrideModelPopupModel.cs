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
    public class OverrideModelPopupModel : ViewModelBase
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

        private string _modelText = "";

        private Model.Model _model;

        private decimal? _overrideTime;

        private int? _overrideRoute;

        private decimal _modelTime;

        private string _modelRoute;

        private ObservableCollection<Override> _overridesToSubmit = new ObservableCollection<Override>();

        private string _informationText;
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

            addOverrideCommand = new RelayCommand(addOverride);
            submitCommand = new RelayCommand(submit);
        }
        #endregion

        #region Commands
        /// <summary>
        /// Adds the information as an override to the ready to submit list
        /// </summary>
        private void addOverride()
        {
            if (checkValid())
            {
                Override ov = new Override
                {
                    Model = model,
                    ModelNum = modelText,
                    OverrideTime = (decimal)overrideTime,
                    OverrideRoute = (int)overrideRoute,
                };
                
                _overridesToSubmit.Add(ov);
            }
        }

        private void submit()
        {
            MessageBox.Show("Placeholder");
        }
        #endregion

        #region Public Variables
        /// <summary>
        /// Updates the model if the text has 8 or more characters
        /// Calls updateModelTime and updateModelRoute
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

                informationText = "";

                if(modelText.Length >= 8)
                {
                    model = _serviceProxy.getModel(modelText.Substring(0, 8));
                    updateModelTime();
                    updateModelRoute();
                }
                else
                {
                    model = null;
                    modelTime = 0;
                    modelRoute = "";
                }
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

        public decimal modelTime
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
            }
        }

        public ObservableCollection<Override> overridesToSubmit 
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
        #endregion

        #region Private Functions
        /// <summary>
        /// Parses the model and creates a list of the options
        /// </summary>
        /// <returns> list of the options</returns>
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
        /// Updates the model time with the calculated time for the model
        /// </summary>
        private void updateModelTime()
        {
            decimal totalTime = 0;
            totalTime += model.DriveTime;
            totalTime += model.AVTime;

            totalTime += _serviceProxy.getTotalOptionsTime(model.BoxSize, parseOptions());

            modelTime = totalTime;
        }

        /// <summary>
        /// Updates the model route based on the model time
        /// </summary>
        private void updateModelRoute()
        {
            TimeSpan time = TimeSpan.FromHours((double)modelTime);

            if (time.TotalMinutes <= 0)
            {
                modelRoute = "0";
            }
            else
            {
                //Format for route is "501",
                //      2 digit hour,
                //      0 if minutes < 30; 1 if > 30,
                //      extra 2 digits for unique route if necessary

                modelRoute = "501";

                decimal hours = (time.Days * 24 + time.Hours);
                string hoursText = "";
                if (hours >= 100)
                {
                    hoursText = "999";
                    modelRoute = string.Concat(modelRoute, hoursText);
                }
                else
                {
                    hoursText = string.Format("{0:00}", (time.Days * 24 + time.Hours));
                    modelRoute = string.Concat(modelRoute, hoursText);

                    string minutesText = "0";
                    if (time.Minutes >= 30)
                    {
                        minutesText = "1";
                    }
                    modelRoute = string.Concat(modelRoute, minutesText);
                }

                modelRoute = string.Concat(modelRoute, "00");
            }
        }

        /// <summary>
        /// Checks if the information is valid
        /// </summary>
        /// <returns> true if valid, false otherwise</returns>
        private bool checkValid()
        {
            bool valid = true;

            // Model needs 4 characters for the drive and 4 characters for voltage and amperage
            if (!string.IsNullOrWhiteSpace(modelText) && modelText.Length >= 8)
            {
                if(model == null)
                {
                    valid = false;
                    informationText = "Model does not exist, enter a different model";
                }

                if(valid && overrideTime == null || overrideTime <= 0)
                {
                    valid = false;
                    informationText = "Invalid override time";
                }

                if (valid && overrideRoute == null || overrideRoute <= 0)
                {
                    valid = false;
                    informationText = "Invalid override route";
                }
            }
            else
            {
                valid = false;
                informationText = "Invalid model";
            }
            if (valid)
            {
                foreach (Override ov in overridesToSubmit)
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

        #endregion
    }
}