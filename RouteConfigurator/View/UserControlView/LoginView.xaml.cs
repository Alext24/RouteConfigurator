using RouteConfigurator.ViewModel.SecurityHelpers;
using System.Windows.Controls;

namespace RouteConfigurator.View.UserControlView
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl, IHavePassword
    {
        public LoginView()
        {
            InitializeComponent();
        }

        public System.Security.SecureString Password
        {
            get
            {
                return UserPassword.SecurePassword;
            }
        }

        public System.Security.SecureString ConfirmPassword
        {
            get
            {
                return null;
            }
        }
    }
}
