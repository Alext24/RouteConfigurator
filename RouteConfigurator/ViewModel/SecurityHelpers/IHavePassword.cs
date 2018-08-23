namespace RouteConfigurator.ViewModel.SecurityHelpers
{
    public interface IHavePassword
    {
        System.Security.SecureString Password { get; }
        System.Security.SecureString ConfirmPassword { get; }
    }
}