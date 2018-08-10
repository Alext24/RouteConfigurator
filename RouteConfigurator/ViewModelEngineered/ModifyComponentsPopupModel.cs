﻿using GalaSoft.MvvmLight;
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

namespace RouteConfigurator.ViewModelEngineered
{
    public class ModifyComponentsPopupModel : ViewModelBase
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

        public ObservableCollection<string> _components = new ObservableCollection<string>();
        private string _component = "";
        public ObservableCollection<string> _enclosureSizes = new ObservableCollection<string>();
        private string _enclosureSize = "";
        private decimal? _newTime;
        private string _description;

        private ObservableCollection<Component> _componentsFound = new ObservableCollection<Component>();

        private string _informationText;

        private bool _loading = false;
        #endregion

        #region RelayCommands
        public RelayCommand loadedCommand { get; set; }
        public RelayCommand submitCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public ModifyComponentsPopupModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            loadedCommand = new RelayCommand(loaded);
            submitCommand = new RelayCommand(submitAsync);
        }
        #endregion

        #region Commands
        private void loaded()
        {
            components = new ObservableCollection<string>(_serviceProxy.getComponents());
            enclosureSizes = new ObservableCollection<string>(_serviceProxy.getEnclosureSizes());
        }

        private async void submitAsync()
        {
            loading = true;
            await Task.Run(() => submit());
            loading = false;
        }

        /// <summary>
        /// Submits each of the component modifications to the database
        /// </summary>
        private void submit()
        {
            informationText = "";

            if (componentsFound.Count <= 0)
            {
                informationText = "No components selected to update.";
            }
            else if (checkComplete())
            {
                try
                {
                    foreach (Component component in componentsFound)
                    {
                        EngineeredModification modifiedComponent = new EngineeredModification()
                        {
                            RequestDate = DateTime.Now,
                            ReviewedDate = new DateTime(1900, 1, 1),
                            Description = string.IsNullOrWhiteSpace(description) ? "no description entered" : description,
                            State = 0,
                            Sender = "TEMPORARY SENDER",
                            Reviewer = "",
                            IsNew = false,
                            ComponentName = component.ComponentName,
                            EnclosureSize = component.EnclosureSize,
                            EnclosureType = "",
                            NewTime = (decimal)newTime,
                            OldTime = component.Time,
                            Gauge = "",
                            NewTimePercentage = 0,
                            OldTimePercentage = 0
                        };
                        _serviceProxy.addEngineeredModificationRequest(modifiedComponent);
                    }

                    //Clear input boxes
                    _component = "";
                    RaisePropertyChanged("component");
                    _enclosureSize = ""; 
                    RaisePropertyChanged("enclosureSize");

                    componentsFound = new ObservableCollection<Component>();
                    newTime = null;
                    description = "";

                    informationText = "Component modifications have been submitted.  Waiting for manager approval.";
                }
                catch (Exception e)
                {
                    informationText = "There was a problem accessing the database";
                    Console.WriteLine(e);
                }
            }
        }
        #endregion

        #region Public Variables
        public ObservableCollection<string> components
        {
            get
            {
                return _components;
            }
            set
            {
                _components = value;
                RaisePropertyChanged("components");
            }
        }

        /// <summary>
        /// Calls updateComponentsTableAsync
        /// </summary>
        public string component
        {
            get
            {
                return _component;
            }
            set
            {
                _component= value.ToUpper();
                RaisePropertyChanged("component");
                informationText = "";

                updateComponentsTableAsync();
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
        /// Calls updateComponentsTableAsync
        /// </summary>
        public string enclosureSize
        {
            get
            {
                return _enclosureSize;
            }
            set
            {
                _enclosureSize = value;
                RaisePropertyChanged("enclosureSize");
                informationText = "";

                updateComponentsTableAsync();
            }
        }

        public decimal? newTime
        {
            get
            {
                return _newTime;
            }
            set
            {
                _newTime = value;
                RaisePropertyChanged("newTime");
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

        public ObservableCollection<Component> componentsFound 
        {
            get
            {
                return _componentsFound;
            }
            set
            {
                _componentsFound = value;
                RaisePropertyChanged("componentsFound");
                informationText = "";
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
        private async void updateComponentsTableAsync()
        {
            loading = true;
            await Task.Run(() => updateComponentsTable());
            loading = false;
        }

        /// <summary>
        /// Updates the components table with the filtered information
        /// </summary>
        private void updateComponentsTable()
        {
            if(string.IsNullOrWhiteSpace(component) && string.IsNullOrWhiteSpace(enclosureSize))
            {
                componentsFound = new ObservableCollection<Component>();
            }
            else
            {
                try
                {
                    informationText = "Loading components...";
                    componentsFound = new ObservableCollection<Component>(_serviceProxy.getFilteredComponents(component, enclosureSize));
                    informationText = "";
                }
                catch (Exception e)
                {
                    informationText = "There was a problem accessing the database";
                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// Checks to see if all necessary fields are filled out with correct formatting
        /// before the option can be added.
        /// </summary>
        /// <returns> true if the form is complete, otherwise false</returns>
        private bool checkComplete()
        {
            bool complete = true;

            if(newTime == null || newTime <= 0)
            {
                informationText = "No new information associated with modification.";
                complete = false;
            }
            return complete;
        }
        #endregion
    }
}
