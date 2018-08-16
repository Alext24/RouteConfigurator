using GalaSoft.MvvmLight.Views;

namespace RouteConfigurator.Services.Interface
{
    public interface IFrameNavigationService : INavigationService
    {
        object Parameter { get; }
    }
}
