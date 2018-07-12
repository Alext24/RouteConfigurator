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

        private Model.Model _selectedModel;

        private DateTime? _date;

        private int? _salesOrder;

        private int? _productionNum;

        private decimal? _driveTime;

        private decimal? _AVTime;

        private int? _numOptions;

        private bool _hasOptions = false;

        private ObservableCollection<TimeTrialsOptionTime> _TTOptions = new ObservableCollection<TimeTrialsOptionTime>();

        private TimeTrial _timeTrial;

        private string _informationText;
        #endregion

        #region RelayCommands
        public RelayCommand goBackCommand { get; set; }
        public RelayCommand addTTCommand { get; set; }
        public RelayCommand submitCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public OverrideModelPopupModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;
            /*
            timeTrial = navigationService.Parameter as TimeTrial;

            selectedModel = timeTrial.Model;
            modelText = timeTrial.Model.Base;
            date = timeTrial.Date;
            salesOrder = timeTrial.SalesOrder;
            productionNum = timeTrial.ProductionNumber;
            driveTime = timeTrial.DriveTime;
            AVTime = timeTrial.AVTime;
            numOptions = timeTrial.NumOptions;

            foreach(TimeTrialsOptionTime TTOption in timeTrial.TTOptionTimes)
            {
                TTOptions.Add(TTOption);
            }
            */

            goBackCommand = new RelayCommand(goBack);
            

//            loadedCommand = new RelayCommand(loaded);
            //addTTCommand = new RelayCommand(addTT);
            //submitCommand = new RelayCommand(submit);
        }
        #endregion

        #region Commands
        /*
                private void loaded()
                {
                    models = _serviceProxy.getModels();
                }
        */

        private void goBack()
        {
            _navigationService.GoBack();
        }
        #endregion

        #region Public Variables
        public TimeTrial timeTrial 
        {
            get
            {
                return _timeTrial;
            }
            set
            {
                _timeTrial = value;
                RaisePropertyChanged("timeTrial");
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
    }
}