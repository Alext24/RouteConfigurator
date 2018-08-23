using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RouteConfigurator.DTOs;
using RouteConfigurator.Services;
using RouteConfigurator.Services.Interface;
using RouteConfigurator.ViewModel.SecurityHelpers;
using System;
using System.Threading.Tasks;

namespace RouteConfigurator.ViewModel.UserControlViewModel
{
    public class LoginViewModel : ViewModelBase
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
        /// Email being used to log in
        /// </summary>
        private string _email;

        private string _informationText;

        private bool _loading = false;
        #endregion

        #region RelayCommands
        public RelayCommand<IHavePassword> loginCommand { get; private set; }
        public RelayCommand goBackCommand { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for the Login view model
        /// </summary>
        public LoginViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            loginCommand = new RelayCommand<IHavePassword>((IHavePassword parameter) => loginAsync(parameter));
            goBackCommand = new RelayCommand(goBack);
        }
        #endregion

        #region Commands
        private async void loginAsync(IHavePassword parameter)
        {
            loading = true;
            informationText = "";
            try
            {
                UserDTO user = null; 
                await Task.Run(() => (user = login(parameter)));

                if(user != null)
                {
                    //Set the user of the program
                    _navigationService.user = user;

                    //Navigate to a page according to their employee type
                    if (user.EmployeeType.Equals("Manager"))
                    {
                        _navigationService.NavigateTo("ManagerView");
                    }
                    else if (user.EmployeeType.Equals("Supervisor"))
                    {
                        _navigationService.NavigateTo("SupervisorView", true);
                    }
                }
            }
            catch (Exception e)
            {
                informationText = e.Message;
            }
            finally
            {
                loading = false;
            }
        }

        /// <summary>
        /// Function for attempting to log in the user
        /// </summary>
        /// <param name="parameter"> Password Box </param>
        /// <returns> returns the user information needed for the program </returns>
        private UserDTO login(IHavePassword parameter)
        {
            PasswordHelper passwordHelper = new PasswordHelper();

            if (parameter != null)
            {
                //Grab the Secure String from the password container object
                var secureString = parameter.Password;

                if (string.IsNullOrWhiteSpace(email))
                {
                    informationText = "Enter your email";
                }
                else if (secureString.Length == 0)
                {
                    informationText = "Enter your password";
                }
                else
                {
                    //Grab the User DTO data
                    UserLoginCredentialsDTO userDTO = _serviceProxy.GetUserLoginCredentials(email);
                    if(userDTO == null)
                    {
                        informationText = "User does not exist";
                        return null;
                    }

                    //Unsecure the password object and compare against the database salt and password hash
                    if (userDTO.PasswordHash == passwordHelper.GenerateSHA256String(passwordHelper.ConvertToUnsecureString(secureString) + userDTO.Salt))
                    {
                        //login success
                        try
                        {
                            return _serviceProxy.GetUser(email);
                        }
                        catch (Exception e)
                        {
                            informationText = "There was a problem accessing the database";
                            Console.WriteLine(e);
                        }
                    }
                    else
                    {
                        informationText = "Incorrect password";
                    }
                }
            }
            return null;
        }

        private void goBack()
        {
            _navigationService.GoBack();
        }
        #endregion

        #region Public Variables
        public string email 
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                RaisePropertyChanged("email");
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
        #endregion
    }
}
