using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Model.EF_EngineeredModels;
using RouteConfigurator.Services;
using RouteConfigurator.Services.Interface;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RouteConfigurator.ViewModel.EngineeredModelViewModel
{
    public class AddComponentPopupModel : ViewModelBase
    {
        #region PrivateVariables
        /// <summary>
        /// Navigation service to help navigate to other pages
        /// </summary>
        private readonly IFrameNavigationService _navigationService;

        /// <summary>
        /// Data access service to retrieve data from a data source
        /// </summary>
        private IDataAccessService _serviceProxy = new DataAccessService();

        private string _componentName;
        public ObservableCollection<string> _enclosureSizes = new ObservableCollection<string>();

        /// <summary>
        /// Used to hold a list of all the different enclosure sizes so a user can enter the time it takes
        /// the component for each enclosure size
        /// </summary>
        public ObservableCollection<Component> _enclosureSizeTimes = new ObservableCollection<Component>();
        private string _description;

        private ObservableCollection<EngineeredModification> _modificationsToSubmit = new ObservableCollection<EngineeredModification>();

        private string _informationText;

        private bool _loading = false;
        #endregion

        #region RelayCommands
        public RelayCommand loadedCommand { get; set; }
        public RelayCommand addComponentCommand { get; set; }
        public RelayCommand submitCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public AddComponentPopupModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            loadedCommand = new RelayCommand(loaded);
            addComponentCommand = new RelayCommand(addComponentAsync);
            submitCommand = new RelayCommand(submitAsync);
        }
        #endregion

        #region Commands
        private async void loaded()
        {
            loading = true;
            await Task.Run(() => getEnclosureSizes());
            loading = false;

            newEnclosureSizeTimesAsync();
        }

        /// <summary>
        /// Loads the different enclosure sizes
        /// </summary>
        private void getEnclosureSizes()
        {
            try
            {
                informationText = "Loading enclosure sizes...";
                enclosureSizes = new ObservableCollection<string>(_serviceProxy.getEnclosureSizes());
                informationText = "";
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        private async void addComponentAsync()
        {
            loading = true;
            await Task.Run(() => addComponent());
            loading = false;
        }

        /// <summary>
        /// Creates the new component modifications for each enclosure size and adds them to the modifications to submit list
        /// Calls checkValid and newEnclosureSizeTimes
        /// </summary>
        private void addComponent()
        {
            informationText = "";

            if (checkValid())
            {
                informationText = "Adding components...";
                foreach (Component component in enclosureSizeTimes)
                {
                    EngineeredModification mod = new EngineeredModification()
                    {
                        RequestDate = DateTime.Now,
                        ReviewedDate = new DateTime(1900, 1, 1),
                        Description = string.IsNullOrWhiteSpace(description) ? "no description entered" : description,
                        State = 0,
                        Sender = string.Format("{0} {1}", _navigationService.user.FirstName, _navigationService.user.LastName),
                        Reviewer = "",
                        IsNew = true,
                        ComponentName = component.ComponentName,
                        EnclosureSize = component.EnclosureSize,
                        EnclosureType = "",
                        NewTime = component.Time,
                        OldTime = 0,
                        Gauge = "",
                        NewTimePercentage = 0,
                        OldTimePercentage = 0
                    };

                    // Since the observable collection was created on the UI thread 
                    // we have to add the override to the list using a delegate function.
                    App.Current.Dispatcher.Invoke(delegate
                    {
                        modificationsToSubmit.Add(mod);
                    });
                }

                //Clear input boxes
                componentName = "";
                newEnclosureSizeTimes();
                informationText = "Components added";
            }
        }

        private async void submitAsync()
        {
            loading = true;
            await Task.Run(() => submit());
            loading = false;
        }

        /// <summary>
        /// Submits each of the new component modifications to the database
        /// Calls newEnclosureSizeTimes
        /// </summary>
        private void submit()
        {
            informationText = "";

            if (modificationsToSubmit.Count > 0)
            {
                try
                {
                    informationText = "Submitting components...";
                    foreach (EngineeredModification mod in modificationsToSubmit)
                    {
                        _serviceProxy.addEngineeredModificationRequest(mod);
                    }
                }
                catch (Exception e)
                {
                    informationText = "There was a problem accessing the database";
                    Console.WriteLine(e);
                    return;
                }
                //Clear input boxes
                componentName = "";
                description = "";
                newEnclosureSizeTimes();

                modificationsToSubmit = new ObservableCollection<EngineeredModification>();

                informationText = "Components have been submitted.  Waiting for manager approval.";
            }
            else
            {
                informationText = "No components to submit.";
            }
        }
        #endregion

        #region Public Variables
        public string componentName
        {
            get
            {
                return _componentName;
            }
            set
            {
                _componentName = value.ToUpper();
                RaisePropertyChanged("componentName");
                informationText = "";
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

        public ObservableCollection<Component> enclosureSizeTimes
        {
            get
            {
                return _enclosureSizeTimes;
            }
            set
            {
                _enclosureSizeTimes = value;
                RaisePropertyChanged("enclosureSizeTimes");
                informationText = "";
            }
        }

        public string description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                RaisePropertyChanged("description");
                informationText = "";
            }
        }

        public ObservableCollection<EngineeredModification> modificationsToSubmit
        {
            get
            {
                return _modificationsToSubmit;
            }
            set
            {
                _modificationsToSubmit = value;
                RaisePropertyChanged("modificationsToSubmit");
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
        private async void newEnclosureSizeTimesAsync()
        {
            loading = true;
            await Task.Run(() => newEnclosureSizeTimes());
            loading = false;
        }

        /// <summary>
        /// Creates a new list for the user to enter component times for each enclosure size
        /// </summary>
        private void newEnclosureSizeTimes()
        {
            enclosureSizeTimes = new ObservableCollection<Component>();
            foreach (string enclosureSize in enclosureSizes)
            {
                enclosureSizeTimes.Add(new Component
                {
                    ComponentName = "",
                    EnclosureSize = enclosureSize,
                    Time = 0
                });
            }
        }

        /// <summary>
        /// Checks that the component does not already exist
        /// Calls checkComplete
        /// </summary>
        /// <returns> true if the component is valid and doesn't already exist, false otherwise </returns>
        private bool checkValid()
        {
            bool valid = checkComplete();
            if (valid)
            {
                try
                {
                    //Check if the component already exists in the database as an component
                    if (_serviceProxy.getComponentNames().Contains(componentName))
                    {
                        informationText = "This component already exists";
                        valid = false;
                    }
                    //Check if the component already exists in the database as a new component request
                    else if (_serviceProxy.getFilteredNewComponents("", componentName, "").ToList().Count > 0)
                    {
                        informationText = "Component is already waiting for approval.";
                        valid = false;
                    }
                    else
                    {
                        //Check if the component is a duplicate in the ready to submit list
                        foreach (EngineeredModification component in modificationsToSubmit)
                        {
                            if (component.ComponentName.Equals(componentName))
                            {
                                informationText = "This component is already ready to submit";
                                valid = false;
                                break;
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
        /// before the component can be added.  Also modifies component list to set the 
        /// component name to make adding them easier.
        /// </summary>
        /// <returns> true if the form is complete, otherwise false</returns>
        private bool checkComplete()
        {
            bool complete = true;

            if (string.IsNullOrWhiteSpace(componentName))
            {
                complete = false;
                informationText = "Enter a component name.";
            }
            else
            {
                foreach (Component component in enclosureSizeTimes)
                {
                    component.ComponentName = componentName;
                    if (component.Time < 0)
                    {
                        component.Time = 0;
                    }
                }
            }

            return complete;
        }
        #endregion
    }
}
