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
    public class ModifyModelPopupModel : ViewModelBase
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

        private ObservableCollection<string> _driveTypes = new ObservableCollection<string>();

        private string _selectedDrive = "";

        private bool _driveNotSelected = true;

        private string _AVText = "";

        private string _boxSize = "";

        private bool _exactBoxSize = false;

        private ObservableCollection<Model.Model> _modelsFound = new ObservableCollection<Model.Model>();

        private Model.Model _selectedModel;

        private decimal? _newDriveTime;

        private decimal? _newAVTime;

        private string _description;

        private string _informationText;
        #endregion

        #region RelayCommands
        public RelayCommand loadedCommand { get; set; }
        public RelayCommand submitCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public ModifyModelPopupModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            loadedCommand = new RelayCommand(loaded);
            submitCommand = new RelayCommand(submit);
        }
        #endregion

        #region Commands
        private void loaded()
        {
            driveTypes = new ObservableCollection<string>(_serviceProxy.getDriveTypes());
        }

        /// <summary>
        /// Submits each model modification to the database
        /// </summary>
        private void submit()
        {
            if (modelsFound.Count <= 0)
            {
                informationText = "No models selected to update.";
            }
            else if (checkValid())
            {
                try
                {
                    foreach (Model.Model model in modelsFound)
                    {
                        Modification modifiedModel = new Modification()
                        {
                            RequestDate = DateTime.Now,
                            ModelBase = model.Base,
                            BoxSize = model.BoxSize,
                            Description = string.IsNullOrWhiteSpace(description) ? "no description entered" : description,
                            State = 0,
                            Sender = "TEMPORARY PLACEHOLDER",
                            IsOption = false,
                            IsNew = false,
                            NewDriveTime = newDriveTime == null || newDriveTime <= 0 ? model.DriveTime : (decimal)newDriveTime,
                            NewAVTime = newAVTime == null || newAVTime <= 0 ? model.AVTime : (decimal)newAVTime,
                            OldModelDriveTime = model.DriveTime,
                            OldModelAVTime = model.AVTime
                        };

                        _serviceProxy.addModificationRequest(modifiedModel);
                    }

                    //Clear input boxes
                    selectedDrive = null;
                    AVText = "";
                    boxSize = "";
                    modelsFound.Clear();
                    newDriveTime = null;
                    newAVTime = null;
                    description = "";
                    informationText = "Model modifications have been submitted.  Waiting for director approval.";
                }
                catch (Exception e)
                {
                    informationText = "There was a problem accessing the database";
                    Console.WriteLine(e);
                    return;
                }
            }
        }
        #endregion

        #region Public Variables
        public ObservableCollection<string> driveTypes
        {
            get
            {
                return _driveTypes;
            }
            set
            {
                _driveTypes = value;
                RaisePropertyChanged("driveTypes");
            }
        }

        /// <summary>
        /// Updates driveNotSelected
        /// Calls updateModelsTable
        /// </summary>
        public string selectedDrive
        {
            get
            {
                return _selectedDrive;
            }
            set
            {
                _selectedDrive = value;
                RaisePropertyChanged("selectedDrive");
                informationText = "";

                updateModelsTable();
                driveNotSelected = value != null ? false : true;
            }
        }

        public bool driveNotSelected
        {
            get
            {
                return _driveNotSelected;
            }
            set
            {
                _driveNotSelected = value;
                RaisePropertyChanged("driveNotSelected");
            }
        }

        /// <summary>
        /// Calls updateModelsTable
        /// </summary>
        public string AVText
        {
            get
            {
                return _AVText;
            }
            set
            {
                _AVText = value.ToUpper();
                RaisePropertyChanged("AVText");
                informationText = "";

                updateModelsTable();
            }
        }

        /// <summary>
        /// Calls updateModelsTable
        /// </summary>
        public string boxSize
        {
            get
            {
                return _boxSize;
            }
            set
            {
                _boxSize = value.ToUpper();
                RaisePropertyChanged("boxSize");
                informationText = "";

                updateModelsTable();
            }
        }

        /// <summary>
        /// Calls updateModelsTable
        /// </summary>
        public bool exactBoxSize
        {
            get
            {
                return _exactBoxSize;
            }
            set
            {
                _exactBoxSize = value;
                RaisePropertyChanged("exactBoxSize");
                informationText = "";

                updateModelsTable();
            }
        }

        public ObservableCollection<Model.Model> modelsFound
        {
            get
            {
                return _modelsFound;
            }
            set
            {
                _modelsFound = value;
                RaisePropertyChanged("modelsFound");
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

        public decimal? newDriveTime
        {
            get
            {
                return _newDriveTime;
            }
            set
            {
                _newDriveTime = value;
                RaisePropertyChanged("newDriveTime");
            }
        }

        public decimal? newAVTime
        {
            get
            {
                return _newAVTime;
            }
            set
            {
                _newAVTime = value;
                RaisePropertyChanged("newAVTime");
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
        /// Updates the models table with the filtered information
        /// </summary>
        private void updateModelsTable()
        {
            if(string.IsNullOrWhiteSpace(selectedDrive) && string.IsNullOrWhiteSpace(AVText) && string.IsNullOrWhiteSpace(boxSize))
            {
                modelsFound.Clear();
            }
            else
            {
                modelsFound = new ObservableCollection<Model.Model>(_serviceProxy.getNumModelsFound(selectedDrive, AVText, boxSize, exactBoxSize));
            }
        }

        /// <summary>
        /// Calls checkComplete
        /// </summary>
        /// <returns> checkComplete value </returns>
        private bool checkValid()
        {
            bool valid = checkComplete();

            /*  I'm not sure how to handle an already existing modification request for a model 
            *  so I will allow multiple of the same model requests.
            if (valid)
            {
                //Check if the model already exists in the database as a model modification request
                string modelBase = string.Concat(selectedDrive, AVText);
                List<Modification> mods = _serviceProxy.getFilteredModifiedModels("", modelBase).ToList();
                if (mods.Count > 0)
                {
                    informationText = string.Format("Model {0} is already waiting for approval.", modelBase);
                    valid = false;
                }
            }
            */

            return valid;
        }

        /// <summary>
        /// Checks to see if new drive time or new av time is filled out correctly
        /// before the modification can be added.  
        /// </summary>
        /// <returns> true if the form is complete, otherwise false </returns>
        private bool checkComplete()
        {
            bool complete = true;

            if ((newDriveTime == null || newDriveTime <= 0) && (newAVTime == null || newAVTime <= 0))
            {
                informationText = "No new information associated with modification.";
                complete = false;
            }

            return complete;
        }
        #endregion
    }
}
