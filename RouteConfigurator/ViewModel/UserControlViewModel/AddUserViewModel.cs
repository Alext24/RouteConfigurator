using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.DTOs;
using RouteConfigurator.Model;
using RouteConfigurator.Services;
using RouteConfigurator.Services.Interface;
using RouteConfigurator.ViewModel.SecurityHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator.ViewModel.UserControlViewModel
{
    public class AddUserViewModel : ViewModelBase
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

        private string _email;
        private string _firstName;
        private string _lastName;
        private ObservableCollection<string> _employeeTypes = new ObservableCollection<string>();
        private string _employeeType;

        private string _informationText;

        private bool _loading = false;
        #endregion

        #region RelayCommands
        public RelayCommand<IHavePassword> createAccountCommand { get; private set; }
        public RelayCommand cancelCommand { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for the Login view model
        /// </summary>
        public AddUserViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            employeeTypes.Add("Manager");
            employeeTypes.Add("Supervisor");

            createAccountCommand = new RelayCommand<IHavePassword>((IHavePassword parameter) => createAccountAsync(parameter));
            cancelCommand = new RelayCommand(cancel);
        }
        #endregion

        #region Commands
        private async void createAccountAsync(IHavePassword parameter)
        {
            loading = true;
            informationText = "";
            try
            {
                await Task.Run(() => createAccount(parameter));
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
        private void createAccount(IHavePassword parameter)
        {
            PasswordHelper passwordHelper = new PasswordHelper();

            if (parameter != null)
            {
                //Grab the Secure String from the password container object
                var secureString1 = parameter.Password;
                var secureString2 = parameter.ConfirmPassword;

                if (string.IsNullOrWhiteSpace(email))
                {
                    informationText = "Enter an email";
                }
                else if (string.IsNullOrWhiteSpace(firstName))
                {
                    informationText = "Enter a first name";
                }
                else if (string.IsNullOrWhiteSpace(lastName))
                {
                    informationText = "Enter a last name";
                }
                else if (string.IsNullOrWhiteSpace(employeeType))
                {
                    informationText = "Select an employee type";
                }
                else if (secureString1.Length == 0)
                {
                    informationText = "Enter your password";
                }
                else if (secureString2.Length == 0)
                {
                    informationText = "Confirm your password";
                }
                else
                {
                    try
                    {
                        if (_serviceProxy.checkDuplicateUser(email))
                        {
                            informationText = "This email already has an account";
                        }
                        else if (!passwordHelper.ConvertToUnsecureString(secureString1).Equals(passwordHelper.ConvertToUnsecureString(secureString2)))
                        {
                            informationText = "Passwords do not match";
                        }
                        else
                        {
                            byte[] salt = getSalt(32);
                            User user = new User
                            {
                                Email = email,
                                FirstName = firstName,
                                LastName = lastName,
                                EmployeeType = employeeType,
                                Salt = salt,
                                PasswordHash = passwordHelper.GenerateSHA256String(passwordHelper.ConvertToUnsecureString(secureString1) + salt)
                            };

                            _serviceProxy.addUser(user);
                            informationText = "User added";
                        }
                    }
                    catch (Exception e)
                    {
                        informationText = "There was a problem accessing the database";
                        Console.WriteLine(e);
                    }
                }
            }
        }

        private void cancel()
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

        public string firstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
                RaisePropertyChanged("firstName");
                informationText = "";
            }
        }

        public string lastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
                RaisePropertyChanged("lastName");
                informationText = "";
            }
        }

        public ObservableCollection<string> employeeTypes
        {
            get
            {
                return _employeeTypes;
            }
            set
            {
                _employeeTypes = value;
                RaisePropertyChanged("employeeTypes");
            }
        }

        public string employeeType
        {
            get
            {
                return _employeeType;
            }
            set
            {
                _employeeType = value;
                RaisePropertyChanged("employeeType");
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
        private static byte[] getSalt(int maximumSaltLength)
        {
            var salt = new byte[maximumSaltLength];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return salt;
        }
        #endregion
    }
}
