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

        private string _AVText = "";

        private string _boxSize = "";

        private bool _exactBoxSize = false;

        private ObservableCollection<Model.Model> _modelsFound = new ObservableCollection<Model.Model>();

        private Model.Model _selectedModel;

        private decimal? _newDriveTime;

        private decimal? _newAVTime;

        private string _description;

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
        public ModifyModelPopupModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            loadedCommand = new RelayCommand(loaded);
            submitCommand = new RelayCommand(submitAsync);
        }
        #endregion

        #region Commands
        private async void loaded()
        {
            loading = true;
            await Task.Run(() => getDriveTypes());
            loading = false;
        }

        private void getDriveTypes()
        {
            try
            {
                informationText = "Loading drive types...";
                driveTypes = new ObservableCollection<string>(_serviceProxy.getDriveTypes());
                informationText = "";
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }

        private async void submitAsync()
        {
            loading = true;
            await Task.Run(() => submit());
            loading = false;
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
                            OldModelAVTime = model.AVTime,

                            Reviewer = "",
                            ReviewDate = new DateTime(1900,1,1),
                            NewName = "",
                            OldOptionName = "",
                            OptionCode = "",
                            NewTime = 0,
                            OldOptionTime = 0,
                            ProductLine = ""
                        };

                        _serviceProxy.addModificationRequest(modifiedModel);
                    }

                    //Clear input boxes
                    _selectedDrive = null;
                    RaisePropertyChanged("selectedDrive");
                    _AVText = "";
                    RaisePropertyChanged("AVText");
                    _boxSize = "";
                    RaisePropertyChanged("boxSize");

                    modelsFound = new ObservableCollection<Model.Model>();
                    newDriveTime = null;
                    newAVTime = null;
                    description = "";

                    informationText = "Model modifications have been submitted.  Waiting for manager approval.";
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
        /// Calls updateModelsTableAsync
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

                updateModelsTableAsync();
            }
        }

        /// <summary>
        /// Calls updateModelsTableAsync
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

                updateModelsTableAsync();
            }
        }

        /// <summary>
        /// Calls updateModelsTableAsync
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

                updateModelsTableAsync();
            }
        }

        /// <summary>
        /// Calls updateModelsTableAsync
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

                updateModelsTableAsync();
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
                informationText = "";
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
        private async void updateModelsTableAsync()
        {
            loading = true;
            await Task.Run(() => updateModelsTable());
            loading = false;
        }

        /// <summary>
        /// Updates the models table with the filtered information
        /// </summary>
        private void updateModelsTable()
        {
            if(string.IsNullOrWhiteSpace(selectedDrive) && string.IsNullOrWhiteSpace(AVText) && string.IsNullOrWhiteSpace(boxSize))
            {
                modelsFound = new ObservableCollection<Model.Model>();
            }
            else
            {
                try
                {
                    informationText = "Loading models...";
                    modelsFound = new ObservableCollection<Model.Model>(_serviceProxy.getModelsFound(selectedDrive, AVText, boxSize, exactBoxSize));
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
