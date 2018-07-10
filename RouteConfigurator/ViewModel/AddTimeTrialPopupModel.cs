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
//        public RelayCommand loadedCommand { get; set; }
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

//            loadedCommand = new RelayCommand(loaded);
            addTTCommand = new RelayCommand(addTT);
            submitCommand = new RelayCommand(submit);
        }
        #endregion

        #region Commands
/*
        private void loaded()
        {
            models = _serviceProxy.getModels();
        }
*/

        /// <summary>
        /// Adds a time trial to the list to be submitted
        /// Calls checkComplete
        /// </summary>
        private void addTT()
        {
            informationText = "";
            if (checkComplete())
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

        /// <summary>
        /// Submits the time trials to the database
        /// </summary>
        private void submit()
        {
            informationText = "";
            MessageBox.Show("Placeholder for adding time trials to database");
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
                //Set(ref _timeTrials, value);
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
                informationText = "";

                if(models == null || models.Count == 0)
                {
                    models = _serviceProxy.getModels();
                }

                _modelText = value.ToUpper();
                RaisePropertyChanged("modelText");
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
                informationText = "";
                _date = value;
                RaisePropertyChanged("date");
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
                informationText = "";
                _salesOrder = value;
                RaisePropertyChanged("salesOrder");
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
                informationText = "";
                _productionNum = value;
                RaisePropertyChanged("productionNum");
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
                informationText = "";
                _driveTime = value;
                RaisePropertyChanged("driveTime");
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
                informationText = "";
                _AVTime = value;
                RaisePropertyChanged("AVTime");
            }
        }

        /// <summary>
        /// Updates hasOptions if greater than 0
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
                informationText = "";
                _numOptions = value;
                RaisePropertyChanged("numOptions");

                if(value > 0 && value != null)
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
                else if(value == 0)
                {
                    hasOptions = false;
                    TTOptions.Clear();
                }
                else
                {
                    hasOptions = false;
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
        /// Checks to see if all necessary fields are filled out before the time
        /// trial can be added.  
        /// </summary>
        /// <returns> if the form is complete </returns>
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
            List<char> powerOptions = new List<char>();
            List<char> controlOptions = new List<char>();

            foreach(TimeTrialsOptionTime option in options)
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
                    default :
                        {
                            break;
                        }
                }
            }

            powerOptions.Sort();
            controlOptions.Sort();

            string optionsText = "P";
            foreach(char c in powerOptions)
            {
                optionsText = string.Concat(optionsText, c);
            }

            optionsText = string.Concat(optionsText, "T");
            foreach(char c in controlOptions)
            {
                optionsText = string.Concat(optionsText, c);
            }

            return optionsText;
        }
        #endregion
    }
}
