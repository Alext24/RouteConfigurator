using GalaSoft.MvvmLight.Views;
using RouteConfigurator.DTOs;

namespace RouteConfigurator.Services.Interface
{
    public interface IFrameNavigationService : INavigationService
    {
        UserDTO user { get; set; }
        object Parameter { get; }
    }
}
