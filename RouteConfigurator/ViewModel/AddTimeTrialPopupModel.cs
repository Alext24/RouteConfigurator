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
        private decimal? _totalTime;

        private bool _haveOptionTimes = false;

        private int? _numOptions;
        private bool _hasOptions = false;
        private ObservableCollection<TimeTrialsOptionTime> _TTOptions = new ObservableCollection<TimeTrialsOptionTime>();

        private string _informationText;

        private bool _loading = false;
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

            loadedCommand = new RelayCommand(loaded);
            addTTCommand = new RelayCommand(addTTAsync);
            submitCommand = new RelayCommand(submitAsync);
        }
        #endregion

        #region Commands
        private async void loaded()
        {
            loading = true;
            await Task.Run(() => getModels());
            loading = false;
        }

        private void getModels()
        {
            try
            {
                informationText = "Loading models...";
                models = new ObservableCollection<Model.Model>(_serviceProxy.getModels());
                informationText = "";
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        private async void addTTAsync()
        {
            loading = true;
            await Task.Run(() => addTT());
            loading = false;
        }

        /// <summary>
        /// Adds a time trial to the list to be submitted
        /// Calls checkValid and getOptionsText
        /// </summary>
        private void addTT()
        {
            informationText = "";
            if (checkValid())
            {
                informationText = "Adding time trial...";
                TimeTrial newTT = new TimeTrial()
                {
                    ProductionNumber = (int)productionNum,
                    SalesOrder = (int)salesOrder,
                    Date = (DateTime)date,
                    DriveTime = driveTime == null ? 0 : (decimal)driveTime,
                    AVTime = AVTime == null ? 0 : (decimal)AVTime,
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

                // Since the observable collection was created on the UI thread 
                // we have to add the override to the list using a delegate function.
                App.Current.Dispatcher.Invoke(delegate
                {
                    timeTrials.Add(newTT);
                });

                // Clear necessary input boxes
                productionNum++;
                driveTime = null;
                AVTime = null;
                totalTime = null;
                numOptions = null;
                TTOptions = new ObservableCollection<TimeTrialsOptionTime>();

                informationText = "Time Trial added";
            }
        }

        private async void submitAsync()
        {
            loading = true;
            await Task.Run(() => submit());
            loading = false;
        }

        /// <summary>
        /// Submits each of the time trials to the database
        /// </summary>
        private void submit()
        {
            informationText = "";

            if (timeTrials.Count > 0)
            {
                try
                {
                    informationText = "Submitting time trials...";
                    _serviceProxy.addTimeTrials(timeTrials);
                }
                catch (Exception e)
                {
                    informationText = "There was a problem accessing the database";
                    Console.WriteLine(e);
                    return;
                }
                //Clear input boxes
                selectedModel = null;
                date = null;
                salesOrder = null;
                productionNum = null;
                driveTime = null;
                AVTime = null;
                numOptions = null;

                timeTrials = new ObservableCollection<TimeTrial>();

                informationText = "Time Trials submitted to database.";
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
                informationText = "";
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

        public decimal? totalTime 
        {
            get
            {
                return _totalTime;
            }
            set
            {
                _totalTime = value;
                RaisePropertyChanged("totalTime");
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

                //Change the count of TTOptions to match the number of options.
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
                    TTOptions = new ObservableCollection<TimeTrialsOptionTime>();
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
        /// Calculates the total time by adding the drive time, AV time, and option times
        /// </summary>
        /// <returns>Total time</returns>
        private decimal calcTotalTime()
        {
            decimal totTime = 0;

            if (_haveOptionTimes && AVTime != null && AVTime > 0 && driveTime != null && driveTime > 0)
            {
                totTime = (decimal)(driveTime + AVTime);
                foreach (TimeTrialsOptionTime TTOption in TTOptions)
                {
                    totTime += TTOption.Time;
                }

                totalTime = totTime;
            }
            else
            {
                totTime = (decimal)totalTime;
            }

            return totTime;
        }

        /// <summary>
        /// Checks that the time trials does not already exist
        /// Calls checkComplete
        /// </summary>
        /// <returns></returns>
        private bool checkValid()
        {
            bool valid = checkComplete();
            if (valid)
            {
                try
                {
                    //Check if time trial production number exists in the database
                    if (_serviceProxy.getTimeTrial((int)productionNum) != null)
                    {
                        informationText = string.Format("Time Trial for Production Number {0} already exists.", productionNum);
                        valid = false;
                    }
                    else
                    {
                        //Check if time trial is a duplicate in the ready to submit list
                        foreach (TimeTrial TT in timeTrials)
                        {
                            if (productionNum == TT.ProductionNumber)
                            {
                                informationText = string.Format("Time Trial for Production Number {0} is already ready to submit", productionNum);
                                valid = false;
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
        /// before the time trial can be added.  
        /// </summary>
        /// <returns> true if the form is complete, otherwise false </returns>
        private bool checkComplete()
        {
            informationText = "";
            bool complete = true;
            _haveOptionTimes = true;

            if (string.IsNullOrWhiteSpace(modelText) || selectedModel == null)
            {
                complete = false;
                modelText = "";
                informationText = "Please select a valid model.";
            }
            else if (date == null ||
                     salesOrder == null || salesOrder <= 0 ||
                     productionNum == null || productionNum <= 0)
            {
                complete = false;
                informationText = "Fill out all fields before adding time trial.";
            }
            else
            {
                foreach (TimeTrialsOptionTime option in TTOptions)
                {
                    if (!complete)
                    {
                        break;
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(option.OptionCode))
                        {
                            complete = false;
                            informationText = "Fill out all options before adding time trial.";
                        }
                        else if (option.OptionCode.Length != 2)
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
                            else if (option.OptionCode.ElementAt(1).Equals('P') || option.OptionCode.ElementAt(1).Equals('T') ||
                                     option.OptionCode.ElementAt(1).Equals('p') || option.OptionCode.ElementAt(1).Equals('t'))
                            {
                                informationText = "Option Code cannot end with a 'P' or 'T'";
                                complete = false;
                            }
                        }
                    }

                    if (option.Time <= 0)
                    {
                        _haveOptionTimes = false;
                    }
                }

                if (complete && TTOptions.GroupBy(n => n.OptionCode).Any(c => c.Count() > 1))
                {
                    complete = false;
                    informationText = "Invalid Options.  Duplicate option exists in list.";
                }
            }

            if (complete)
            {
                if(!_haveOptionTimes || driveTime == null || driveTime <= 0 || AVTime == null || AVTime <= 0)
                {
                    if(totalTime == null || totalTime <= 0)
                    {
                        complete = false;
                        informationText = "Enter the total time or fill out the drive time, av time, and all option times.";
                    }
                    else if(driveTime != null || AVTime != null)
                    {
                        if (totalTime < ((driveTime == null ? 0 : driveTime) + (AVTime == null ? 0 : AVTime)))
                        {
                            complete = false;
                            informationText = "Total time cannot be less than drive time plus av time.";
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

                string optionsText = "";

                //Form the options text
                if (powerOptions.Count > 0)
                {
                    optionsText = "P";
                    foreach (char c in powerOptions)
                    {
                        optionsText = string.Concat(optionsText, c);
                    }
                }

                if (controlOptions.Count > 0)
                {
                    optionsText = string.Concat(optionsText, "T");
                    foreach (char c in controlOptions)
                    {
                        optionsText = string.Concat(optionsText, c);
                    }
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
