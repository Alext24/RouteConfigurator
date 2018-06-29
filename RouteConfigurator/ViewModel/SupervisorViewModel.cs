using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Design;
using RouteConfigurator.Model;
using RouteConfigurator.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator.ViewModel
{
    public class SupervisorViewModel : ViewModelBase
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

        private ObservableCollection<Model.Model> _models;

        private string _modelFilter = "";

        private string _boxSizeFilter = "";

        private ObservableCollection<Option> _options;

        private string _optionFilter = "";

        private string _optionBoxSizeFilter = "";

        private bool _TTVisible = false;
        #endregion

        #region RelayCommands
        public RelayCommand addModelCommand { get; set; }
        public RelayCommand loadModelsCommand { get; set; }
        public RelayCommand loadOptionsCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public SupervisorViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            addModelCommand = new RelayCommand(addModel);
            loadModelsCommand = new RelayCommand(loadModels);
            loadOptionsCommand = new RelayCommand(loadOptions);
        }
        #endregion

        #region Commands
        private void addModel()
        {
            AddModelPopup addModel = new AddModelPopup();
            addModel.Show();
        }

        private void loadModels()
        {
            models = _serviceProxy.getModels();
        }

        private void loadOptions()
        {
            options = _serviceProxy.getOptions();
        }

        #endregion

        #region Public Variables
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

        public string modelFilter
        {
            get
            {
                return _modelFilter;
            }
            set
            {
                _modelFilter = value.ToUpper();
                RaisePropertyChanged("modelFilter");

                /*
                 * Testing
                 */
                if (!string.IsNullOrWhiteSpace(value))
                {
                    TTVisible = true;
                }
                else
                {
                    TTVisible = false;
                }

                updateFilter();
            }
        }

        public string boxSizeFilter
        {
            get
            {
                return _boxSizeFilter;
            }
            set
            {
                _boxSizeFilter = value.ToUpper();
                RaisePropertyChanged("boxSizeFilter");
                updateFilter();
            }
        }

        public ObservableCollection<Option> options 
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value;
                RaisePropertyChanged("options");
            }
        }

        public string optionFilter
        {
            get
            {
                return _optionFilter;
            }
            set
            {
                _optionFilter = value.ToUpper();
                RaisePropertyChanged("optionFilter");
                updateOptionFilter();
            }
        }

        public string optionBoxSizeFilter
        {
            get
            {
                return _optionBoxSizeFilter;
            }
            set
            {
                _optionBoxSizeFilter = value.ToUpper();
                RaisePropertyChanged("optionBoxSizeFilter");
                updateOptionFilter();
            }
        }

        public bool TTVisible
        {
            get
            {
                return _TTVisible;
            }
            set
            {
                _TTVisible = value;
                RaisePropertyChanged("TTVisible");
            }
        }
        #endregion

        #region Private Functions
        private void updateFilter()
        {
            models = _serviceProxy.getFilteredModels(modelFilter, boxSizeFilter);
        }

        private void updateOptionFilter()
        {
            options = _serviceProxy.getFilteredOptions(optionFilter, optionBoxSizeFilter);
        }
        #endregion
    }
}
