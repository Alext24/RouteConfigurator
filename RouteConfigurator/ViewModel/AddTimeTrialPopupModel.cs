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
    public class AddTimeTrialPopupModel : ViewModelBase
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

        private ObservableCollection<TimeTrial> _timeTrials = new ObservableCollection<TimeTrial>();

        private string _modelText = "";
        private ObservableCollection<Model.Model> _models = new ObservableCollection<Model.Model>();
        private Model.Model _selectedModel;

        private DateTime? _date;
        private int? _salesOrder;
        private int? _productionNum;
        private decimal? _driveTime;
        private decimal? _AVTime;

        private int? _numOptions;
        private bool _hasOptions = false;
        private ObservableCollection<TimeTrialsOptionTime> _TTOptions = new ObservableCollection<TimeTrialsOptionTime>();

        private string _informationText;
        #endregion

        #region RelayCommands
        public RelayCommand loadedCommand { get; set; }
        public RelayCommand addTTCommand { get; set; }
        public RelayCommand submitCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public AddTimeTrialPopupModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            //models = _serviceProxy.getModels();

            loadedCommand = new RelayCommand(loaded);
            addTTCommand = new RelayCommand(addTT);
            submitCommand = new RelayCommand(submit);
        }
        #endregion

        #region Commands
        private void loaded()
        {
            models = _serviceProxy.getModels();
        }

        /// <summary>
        /// Adds a time trial to the list to be submitted
        /// Calls checkComplete and checkValid
        /// </summary>
        private void addTT()
        {
            informationText = "";
            if (checkComplete())
            {
                if (checkValid())
                {
                    TimeTrial newTT = new TimeTrial()
                    {
                        ProductionNumber = (int)productionNum,
                        SalesOrder = (int)salesOrder,
                        Date = (DateTime)date,
                        DriveTime = (decimal)driveTime,
                        AVTime = (decimal)AVTime,
                        Model = selectedModel,
                        NumOptions = numOptions == null ? 0 : (int)numOptions,
                        TTOptionTimes = new ObservableCollection<TimeTrialsOptionTime>(),

                        TotalTime = calcTotalTime()
                    };

                    foreach (TimeTrialsOptionTime TTOption in TTOptions)
                    {
                        TTOption.OptionCode = TTOption.OptionCode.ToUpper();
                        TTOption.ProductionNumber = (int)productionNum;
                        TTOption.TimeTrial = newTT;

                        newTT.TTOptionTimes.Add(TTOption);
                    }

                    //Sort list alphabetically
                    newTT.TTOptionTimes = new ObservableCollection<TimeTrialsOptionTime>(newTT.TTOptionTimes.OrderBy(i => i.OptionCode));

                    newTT.OptionsText = getOptionsText(newTT.TTOptionTimes);

                    timeTrials.Add(newTT);

                    // Clear input boxes

                    //modelText = "";
                    //date = null;
                    //salesOrder = null;
                    productionNum++;
                    driveTime = null;
                    AVTime = null;
                    numOptions = null;
                    TTOptions = new ObservableCollection<TimeTrialsOptionTime>();

                    informationText = "Time Trial added";
                }
            }
        }

        /// <summary>
        /// Submits the time trials to the database
        /// </summary>
        private void submit()
        {
            informationText = "";

            if (timeTrials.Count > 0)
            {
                try
                {
                    _serviceProxy.addTimeTrials(timeTrials);
                    informationText = "Time Trials submitted to database.";
                    timeTrials.Clear();
                }
                catch (Exception e)
                {
                    informationText = "There was a problem accessing the database";
                    Console.WriteLine(e);
                    return;
                }
            }
            else
            {
                informationText = "No time trials to submit.";
            }
        }
        #endregion

        #region Public Variables
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

        /// <summary>
        /// Updates the model list if empty
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

                if(models == null || models.Count == 0)
                {
                    models = _serviceProxy.getModels();
                }
            }
        }

        public ObservableCollection<Model.Model> models 
        {
            get
            {
                return _models;
            }
            set
            {
                _models = value;
                RaisePropertyChanged("models");
            }
        }

        public Model.Model selectedModel
        {
            get
            {
                return _selectedModel;
            }
            set
            {
                _selectedModel = value;
                RaisePropertyChanged("selectedModel");
            }
        }

        public DateTime? date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                RaisePropertyChanged("date");

                informationText = "";
            }
        }

        public int? salesOrder
        {
            get
            {
                return _salesOrder;
            }
            set
            {
                _salesOrder = value;
                RaisePropertyChanged("salesOrder");

                informationText = "";
            }
        }

        public int? productionNum
        {
            get
            {
                return _productionNum;
            }
            set
            {
                _productionNum = value;
                RaisePropertyChanged("productionNum");

                informationText = "";
            }
        }

        public decimal? driveTime
        {
            get
            {
                return _driveTime;
            }
            set
            {
                _driveTime = value;
                RaisePropertyChanged("driveTime");

                informationText = "";
            }
        }

        public decimal? AVTime
        {
            get
            {
                return _AVTime;
            }
            set
            {
                _AVTime = value;
                RaisePropertyChanged("AVTime");

                informationText = "";
            }
        }

        /// <summary>
        /// Updates hasOptions if number of options is greater than 0
        /// Adds or removes list entries from TTOptions as necessary to match the quantity of options
        /// </summary>
        public int? numOptions
        {
            get
            {
                return _numOptions;
            }
            set
            {
                _numOptions = value;
                RaisePropertyChanged("numOptions");

                informationText = "";

                if(value != null && value > 0)
                {
                    hasOptions = true;

                    int size = (int)(numOptions - TTOptions.Count());
                    if (size > 0)
                    {
                        for (int i = 0; i < size; i++)
                        {
                            TTOptions.Add(new TimeTrialsOptionTime());
                        }
                    }
                    else
                    {
                        size = Math.Abs(size);
                        for (int i = 0; i < size; i++)
                        {
                            TTOptions.RemoveAt(TTOptions.IndexOf(TTOptions.Last()));
                        }
                    }
                }
                else
                {
                    hasOptions = false;
                    TTOptions.Clear();
                }
            }
        }

        public bool hasOptions
        {
            get
            {
                return _hasOptions;
            }
            set
            {
                _hasOptions = value;
                RaisePropertyChanged("hasOptions");
            }
        }

        public ObservableCollection<TimeTrialsOptionTime> TTOptions
        {
            get
            {
                return _TTOptions;
            }
            set
            {
                _TTOptions = value;
                RaisePropertyChanged("TTOptions");
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
        /// Calculates the total time by adding the drive time, AV time, and option times
        /// </summary>
        /// <returns>Total time</returns>
        private decimal calcTotalTime()
        {
            informationText = "";

            decimal totalTime = 0;

            totalTime = (decimal)(driveTime + AVTime);
            foreach(TimeTrialsOptionTime TTOption in TTOptions)
            {
                totalTime += TTOption.Time;
            }

            return totalTime;
        }

        /// <summary>
        /// 
        /// Calls checkComplete
        /// </summary>
        /// <returns></returns>
        private bool checkValid()
        {
            bool valid = checkComplete();
            if (valid)
            {
                //If everything is filled out
                using (RouteConfiguratorDB context = new RouteConfiguratorDB())
                {
                    //Check if time trial production number exists in the database
                    if (context.TimeTrials.Find(productionNum) != null)
                    {
                        informationText = string.Format("Time Trial for Production Number {0} already exists.", productionNum);
                        valid = false;
                    }
                }
                foreach(TimeTrial TT in timeTrials)
                {
                    if(productionNum == TT.ProductionNumber)
                    {
                        informationText = string.Format("Time Trial for Production Number {0} is already ready to submit", productionNum);
                        valid = false;
                    }
                }
            }
            return valid;
        }

        /// <summary>
        /// Checks to see if all necessary fields are filled out before the time
        /// trial can be added.  
        /// </summary>
        /// <returns> true if the form is complete, otherwise false </returns>
        private bool checkComplete()
        {
            informationText = "";
            bool complete = true;

            if (string.IsNullOrWhiteSpace(modelText) || selectedModel == null)
            {
                complete = false;
                modelText = "";
                informationText = "Please select a valid model.";
            }
            else if (salesOrder == null || salesOrder <= 0 ||
               productionNum == null || productionNum <= 0 ||
               driveTime == null || driveTime <= 0 ||
               AVTime == null || AVTime <= 0)
            {
                complete = false;
                informationText = "Fill out all fields before adding time trial.";
            }
            else
            {
                foreach (TimeTrialsOptionTime option in TTOptions)
                {
                    if (string.IsNullOrWhiteSpace(option.OptionCode) ||
                        option.Time <= 0)
                    {
                        complete = false;
                        informationText = "Fill out all options before adding time trial.";
                    }
                    else
                    {
                        if (option.OptionCode.Length != 2)
                        {
                            informationText = "Invalid Option Code Format.  Must be 2 letters";
                            complete = false;
                        }
                        else
                        {
                            if (!option.OptionCode.ElementAt(0).Equals('P') && !option.OptionCode.ElementAt(0).Equals('T') &&
                                !option.OptionCode.ElementAt(0).Equals('p') && !option.OptionCode.ElementAt(0).Equals('t'))
                            {
                                informationText = "Option Code must start with a 'P' or 'T'";
                                complete = false;
                            }
                        }
                    }
                }
            }

            return complete;
        }

        /// <summary>
        /// Concatentates the option codes together to form the options 
        /// text part of the model number
        /// </summary>
        /// <param name="options"> list of options for the model </param>
        /// <returns> options text </returns>
        private string getOptionsText(ICollection<TimeTrialsOptionTime> options)
        {
            if (options.Count > 0)
            {
                List<char> powerOptions = new List<char>();
                List<char> controlOptions = new List<char>();

                foreach (TimeTrialsOptionTime option in options)
                {
                    char optionType = option.OptionCode.ElementAt(0);

                    switch (optionType)
                    {
                        case 'P':
                            {
                                powerOptions.Add(option.OptionCode.ElementAt(1));
                                break;
                            }
                        case 'T':
                            {
                                controlOptions.Add(option.OptionCode.ElementAt(1));
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }

                powerOptions.Sort();
                controlOptions.Sort();

                //Form the options text
                string optionsText = "P";
                foreach (char c in powerOptions)
                {
                    optionsText = string.Concat(optionsText, c);
                }

                optionsText = string.Concat(optionsText, "T");
                foreach (char c in controlOptions)
                {
                    optionsText = string.Concat(optionsText, c);
                }

                return optionsText;
            }
            else
            {
                return "";
            }
        }
        #endregion
    }
}
