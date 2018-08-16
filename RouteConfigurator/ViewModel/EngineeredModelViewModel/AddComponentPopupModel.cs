﻿using GalaSoft.MvvmLight;
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
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Data access service to retrieve data from a data source
        /// </summary>
        private IDataAccessService _serviceProxy = new DataAccessService();

        private string _componentName;
        public ObservableCollection<string> _enclosureSizes = new ObservableCollection<string>();
        public ObservableCollection<Component> _componentList = new ObservableCollection<Component>();
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
        private void loaded()
        {
            enclosureSizes = new ObservableCollection<string>(_serviceProxy.getEnclosureSizes());
            newComponentList();
        }

        private async void addComponentAsync()
        {
            loading = true;
            await Task.Run(() => addComponent());
            loading = false;
        }

        /// <summary>
        /// Creates the new component modification and adds it to the modifications to submit list
        /// Calls checkValid
        /// </summary>
        private void addComponent()
        {
            informationText = "";

            if (checkValid())
            {
                informationText = "Adding components...";
                foreach (Component component in componentList)
                {
                    EngineeredModification mod = new EngineeredModification()
                    {
                        RequestDate = DateTime.Now,
                        ReviewedDate = new DateTime(1900, 1, 1),
                        Description = string.IsNullOrWhiteSpace(description) ? "no description entered" : description,
                        State = 0,
                        Sender = "TEMPORARY SENDER",
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
                newComponentList();
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
                newComponentList();

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

        public ObservableCollection<Component> componentList 
        {
            get
            {
                return _componentList;
            }
            set
            {
                _componentList = value;
                RaisePropertyChanged("componentList");
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
        private void newComponentList()
        {
            componentList = new ObservableCollection<Component>();
            foreach(string enclosureSize in enclosureSizes)
            {
                componentList.Add(new Component
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
                    //if (_serviceProxy.getFilteredComponents(componentName, enclosureSize).ToList().Count > 0)
                    if (_serviceProxy.getFilteredComponents(componentName, "").ToList().Count > 0)
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
                foreach(Component component in componentList)
                {
                    component.ComponentName = componentName;
                    if(component.Time < 0)
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