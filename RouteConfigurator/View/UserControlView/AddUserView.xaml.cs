using RouteConfigurator.ViewModel.SecurityHelpers;
using System.Windows.Controls;

namespace RouteConfigurator.View.UserControlView
{
    /// <summary>
    /// Interaction logic for AddUserView.xaml
    /// </summary>
    public partial class AddUserView : UserControl, IHavePassword
    {
        public AddUserView()
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
                return UserConfirmPassword.SecurePassword;
            }
        }
    }
}