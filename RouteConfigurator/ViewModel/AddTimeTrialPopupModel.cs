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

        private TimeTrial _selectedTT;

        private string _modelText = "";

        private ObservableCollection<Model.Model> _models = new ObservableCollection<Model.Model>();

        private Model.Model _selectedModel;

        private DateTime? _date;

        private int? _salesOrder;

        private int? _productionNum;

        private decimal? _driveTime;

        private decimal? _AVTime;

        private int? _numOptions;

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

        private void addTT()
        {
            TimeTrial newTT = new TimeTrial()
            {
                ProductionNumber = (int)productionNum,
                SalesOrder = (int)salesOrder,
                Date = (DateTime)date,
                DriveTime = (decimal)driveTime,
                AVTime = (decimal)AVTime,
                Model = selectedModel,
                NumOptions = (int)numOptions,
                TTOptionTimes = new ObservableCollection<TimeTrialsOptionTime>(),

                TotalTime = calcTotalTime()
            };

            foreach(TimeTrialsOptionTime TTOption in TTOptions)
            {
                TTOption.OptionCode = TTOption.OptionCode.ToUpper();
                TTOption.ProductionNumber = (int)productionNum;
                TTOption.TimeTrial = newTT;

                newTT.TTOptionTimes.Add(TTOption);
            }

            timeTrials.Add(newTT);

            modelText = "";
            date = null;
            salesOrder = null;
            productionNum = null;
            driveTime = null;
            AVTime = null;
            numOptions = null;
            TTOptions = new ObservableCollection<TimeTrialsOptionTime>();

            informationText = "Time Trial added";
        }

        private void submit()
        {
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

        public TimeTrial selectedTT
        {
            get
            {
                return _selectedTT;
            }
            set
            {
                _selectedTT = value;
                RaisePropertyChanged("selectedTT");
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
            }
        }

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
                if(value > 0)
                {
//FIX THIS
                    for(int i = 0; i < numOptions; i++)
                    {
                        TTOptions.Add(new TimeTrialsOptionTime());
                    }
                }
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
        public decimal calcTotalTime()
        {
            decimal totalTime = 0;

            totalTime = (decimal)(driveTime + AVTime);
            foreach(TimeTrialsOptionTime TTOption in TTOptions)
            {
                totalTime += TTOption.Time;
            }

            return totalTime;
        }

        #endregion
    }
}
