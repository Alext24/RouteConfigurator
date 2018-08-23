using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Model.EF_StandardModels;
using RouteConfigurator.Services;
using RouteConfigurator.Services.Interface;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RouteConfigurator.ViewModel.StandardModelViewModel
{
    public class ModifyModelPopupModel : ViewModelBase
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

        /// <summary>
        /// List of unique drive types (first 4 characters of the model ex. A1C1)
        /// to populate a drop down box
        /// </summary>
        private ObservableCollection<string> _driveTypes = new ObservableCollection<string>();

        /// <summary>
        /// List of unique box sizes for all models to populate a drop down box
        /// </summary>
        private ObservableCollection<string> _boxSizes = new ObservableCollection<string>();

        //Input filters
        private string _selectedDrive = "";
        private string _AVText = "";
        private string _boxSize = "";

        /// <summary>
        /// All models found that meet the filters
        /// These are the models that will be modified when submitted
        /// </summary>
        private ObservableCollection<StandardModel> _modelsFound = new ObservableCollection<StandardModel>();

        //New information
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
            await Task.Run(() => getInformation());
            loading = false;
        }

        /// <summary>
        /// Loads the information, drive types and box sizes, needed for the page
        /// </summary>
        private void getInformation()
        {
            try
            {
                informationText = "Loading Information...";
                driveTypes = new ObservableCollection<string>(_serviceProxy.getDriveTypes());
                boxSizes = new ObservableCollection<string>(_serviceProxy.getModelBoxSizes());
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
        /// Calls checkComplete
        /// </summary>
        private void submit()
        {
            if (modelsFound.Count <= 0)
            {
                informationText = "No models selected to update.";
            }
            else if (checkComplete())
            {
                try
                {
                    foreach (StandardModel model in modelsFound)
                    {
                        Modification modifiedModel = new Modification()
                        {
                            RequestDate = DateTime.Now,
                            ModelBase = model.Base,
                            BoxSize = model.BoxSize,
                            Description = string.IsNullOrWhiteSpace(description) ? "no description entered" : description,
                            State = 0,
                            Sender = string.Format("{0} {1}", _navigationService.user.FirstName, _navigationService.user.LastName),
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
                    _boxSize = null; 
                    RaisePropertyChanged("boxSize");

                    modelsFound = new ObservableCollection<StandardModel>();
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

        public ObservableCollection<string> boxSizes
        {
            get
            {
                return _boxSizes;
            }
            set
            {
                _boxSizes = value;
                RaisePropertyChanged("boxSizes");
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
                _boxSize = value == null ? value : value.ToUpper();
                RaisePropertyChanged("boxSize");
                informationText = "";

                updateModelsTableAsync();
            }
        }

        public ObservableCollection<StandardModel> modelsFound
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
            //If no filters are entered clear the list so no models will be updated
            if(string.IsNullOrWhiteSpace(selectedDrive) && string.IsNullOrWhiteSpace(AVText) && string.IsNullOrWhiteSpace(boxSize))
            {
                modelsFound = new ObservableCollection<StandardModel>();
            }
            else
            {
                try
                {
                    informationText = "Loading models...";
                    //If a box size is entered get the models that are that box size and meet the other filters
                    if (!string.IsNullOrWhiteSpace(boxSize))
                    {
                        modelsFound = new ObservableCollection<StandardModel>(_serviceProxy.getModelsFound(selectedDrive, AVText, boxSize));
                    }
                    //If a box size is not entered get the models that meet the other filters
                    else
                    {
                        modelsFound = new ObservableCollection<StandardModel>(_serviceProxy.getModelsFound(selectedDrive, AVText));
                    }
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
        /// Checks to see if new drive time or new av time is filled out correctly
        /// before the modification can be added.  
        /// </summary>
        /// <remarks> Either newDriveTime or newAVTime need to be filled out </remarks>
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
