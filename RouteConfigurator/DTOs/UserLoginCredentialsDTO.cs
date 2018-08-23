namespace RouteConfigurator.DTOs
{
    public class UserLoginCredentialsDTO
    {
        public string PasswordHash { get; set; }
        public byte[] Salt { get; set; }
        public string EmployeeType { get; set; }
    }
}
