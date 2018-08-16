using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Design;
using RouteConfigurator.DTOs;
using RouteConfigurator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RouteConfigurator.ViewModelEngineered
{
    public class EngineeredHomeViewModel : ViewModelBase
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

        private ObservableCollection<string> _enclosureTypes = new ObservableCollection<string>();
        private string _selectedEnclosureType;

        private ObservableCollection<string> _enclosureSizes = new ObservableCollection<string>();
        private string _selectedEnclosureSize;

        private ObservableCollection<WireGauge> _wireGauges = new ObservableCollection<WireGauge>();
        private WireGauge _selectedWireGauge;

        private string _routeText;
        private string _prodSupCodeText;
        private string _materialNumber;

        private ObservableCollection<EngineeredModelDTO> _engineeredModelComponents = new ObservableCollection<EngineeredModelDTO>();
        private ObservableCollection<Component> _enclosureSizeComponents = new ObservableCollection<Component>();

        /// <summary>
        /// Total time for all of the components entered
        /// </summary>
        private decimal? _totalTime;

        private string _informationText;

        private bool _loading = false;
        #endregion

        #region RelayCommands
        public RelayCommand loadedCommand { get; set; }
        public RelayCommand submitToQueueCommand { get; set; }
        public RelayCommand supervisorLoginCommand { get; set; }
        public RelayCommand managerLoginCommand { get; set; }
        public RelayCommand routeQueueCommand { get; set; }
        public RelayCommand standardOrdersCommand { get; set; }
        public RelayCommand cellEditEndingCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public EngineeredHomeViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            loadedCommand = new RelayCommand(loaded);
            submitToQueueCommand = new RelayCommand(submitToQueueAsync);
            supervisorLoginCommand = new RelayCommand(supervisorLogin);
            managerLoginCommand = new RelayCommand(managerLogin);
            routeQueueCommand = new RelayCommand(routeQueue);
            standardOrdersCommand = new RelayCommand(standardOrders);
            cellEditEndingCommand = new RelayCommand (cellChanged);
        }
        #endregion

        #region Commands
        private void loaded()
        {
            enclosureTypes = new ObservableCollection<string>(_serviceProxy.getEnclosureTypes());
            selectedEnclosureType = enclosureTypes.Count > 0 ? enclosureTypes.ElementAt(0) : null;

            enclosureSizes = new ObservableCollection<string>(_serviceProxy.getEnclosureSizes());
            selectedEnclosureSize = enclosureSizes.Count > 0 ? enclosureSizes.ElementAt(0) : null;

            wireGauges = new ObservableCollection<WireGauge>(_serviceProxy.getWireGauges());
            selectedWireGauge = wireGauges.Count > 0 ? wireGauges.ElementAt(0) : null;

            engineeredModelComponents = new ObservableCollection<EngineeredModelDTO>(_serviceProxy.getModelComponents());
        }

        private async void submitToQueueAsync()
        {
            await Task.Run(() => submitToQueue());
        }

        private void submitToQueue()
        {
            if (string.IsNullOrWhiteSpace(prodSupCodeText))
            {
                informationText = "Complete route information before submitting route.";
            }
            else if (string.IsNullOrWhiteSpace(materialNumber))
            {
                informationText = "Enter the material number before submitting route.";
            }
            else
            {
                try
                {
                    informationText = "Adding route to queue...";
                    RouteQueue route = new RouteQueue
                    {
                        Route = int.Parse(routeText),
                        ModelNumber = materialNumber,
                        Line = "ENG-FLR", //Engineered Floor Mount Line
                        TotalTime = (decimal)totalTime,
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

        private void supervisorLogin()
        {
            _navigationService.NavigateTo("EngineeredSupervisorView", true);
        }

        private void managerLogin()
        {
            _navigationService.NavigateTo("EngineeredManagerView");
        }

        private void routeQueue()
        {
            _navigationService.NavigateTo("RouteQueueView");
        }

        private void standardOrders()
        {
            _navigationService.NavigateTo("HomeView");
        }

        /// <summary>
        /// Calls calcTotalTime
        /// </summary>
        private void cellChanged()
        {
            calcTotalTime();
        }
        #endregion

        #region Public Variables
        public ObservableCollection<string> enclosureTypes
        {
            get
            {
                return _enclosureTypes;
            }
            set
            {
                _enclosureTypes = value;
                RaisePropertyChanged("enclosureTypes");
            }
        }

        /// <summary>
        /// Calls calcTotalTime
        /// </summary>
        public string selectedEnclosureType
        {
            get
            {
                return _selectedEnclosureType;
            }
            set
            {
                _selectedEnclosureType = value;
                RaisePropertyChanged("selectedEnclosureType");
                informationText = "";

                calcTotalTime();
            }
        }

        public ObservableCollection<string> enclosureSizes
        {
            get
            {
                return _enclosureSizes;
            }
            set
            {
                _enclosureSizes = value;
                RaisePropertyChanged("enclosureSizes");
            }
        }

        /// <summary>
        /// Calls updateComponentsTable
        /// </summary>
        public string selectedEnclosureSize
        {
            get
            {
                return _selectedEnclosureSize;
            }
            set
            {
                _selectedEnclosureSize = value;
                RaisePropertyChanged("selectedEnclosureSize");
                informationText = "";

                updateComponentsTable();
            }
        }

        public ObservableCollection<WireGauge> wireGauges 
        {
            get
            {
                return _wireGauges;
            }
            set
            {
                _wireGauges = value;
                RaisePropertyChanged("wireGauges");
            }
        }

        /// <summary>
        /// Calls calcTotalTime
        /// </summary>
        public WireGauge selectedWireGauge
        {
            get
            {
                return _selectedWireGauge;
            }
            set
            {
                _selectedWireGauge = value;
                RaisePropertyChanged("selectedWireGauge");
                informationText = "";

                calcTotalTime();
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

        public string materialNumber
        {
            get
            {
                return _materialNumber;
            }
            set
            {
                _materialNumber = value.ToUpper();
                RaisePropertyChanged("materialNumber");
            }
        }

        public ObservableCollection<EngineeredModelDTO> engineeredModelComponents 
        {
            get
            {
                return _engineeredModelComponents;
            }
            set
            {
                _engineeredModelComponents = value;
                RaisePropertyChanged("engineeredModelComponents");
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
        private void updateComponentsTable()
        {
            _enclosureSizeComponents = new ObservableCollection<Component>(_serviceProxy.getEnclosureSizeComponents(selectedEnclosureSize));
            calcTotalTime();
        }

        /// <summary>
        /// Sums the component times
        /// Calls setProdSupCode and setRoute
        /// </summary>
        /// <returns> total production time for the model</returns>
        private void calcTotalTime()
        {
            totalTime = 0;

            if(selectedEnclosureType != null && selectedEnclosureSize != null)
            {
                totalTime += _serviceProxy.getEnclosure(selectedEnclosureType, selectedEnclosureSize).Time;
            }

            foreach (EngineeredModelDTO DTOcomponent in engineeredModelComponents)
            {
                Component component = _enclosureSizeComponents.Where(x => x.ComponentName.Equals(DTOcomponent.ComponentName)).FirstOrDefault();
                DTOcomponent.TotalTime = DTOcomponent.Quantity * component.Time;
                totalTime += DTOcomponent.TotalTime;
            }

            if(selectedWireGauge != null)
            {
                totalTime += (totalTime * selectedWireGauge.TimePercentage);
            }

            setProdSupCode((decimal)totalTime);

            TimeSpan time = TimeSpan.FromHours((double)totalTime);
            setRoute(time);
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
