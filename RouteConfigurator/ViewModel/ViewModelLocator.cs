/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:RouteConfigurator.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using RouteConfigurator.Design;
using RouteConfigurator.Model;
using System;

namespace RouteConfigurator.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            setupNavigation();

            SimpleIoc.Default.Register<HomeViewModel>();
            SimpleIoc.Default.Register<SupervisorViewModel>();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
            }
        }

        public HomeViewModel Home
        {
            get
            {
                SimpleIoc.Default.Unregister<HomeViewModel>();
                SimpleIoc.Default.Register<HomeViewModel>();
                return ServiceLocator.Current.GetInstance<HomeViewModel>();
            }
        }

        public SupervisorViewModel Supervisor 
        {
            get
            {
                SimpleIoc.Default.Unregister<SupervisorViewModel>();
                SimpleIoc.Default.Register<SupervisorViewModel>();
                return ServiceLocator.Current.GetInstance<SupervisorViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            SimpleIoc.Default.Unregister<HomeViewModel>();
            SimpleIoc.Default.Unregister<SupervisorViewModel>();
        }

        public static void setupNavigation()
        {
            var navigationService = new FrameNavigationService();

            navigationService.Configure("HomeView", new System.Uri("/View/HomeView.xaml", UriKind.Relative));
            navigationService.Configure("SupervisorView", new System.Uri("/View/SupervisorView.xaml", UriKind.Relative));

            SimpleIoc.Default.Unregister<IFrameNavigationService>();
            SimpleIoc.Default.Register<IFrameNavigationService>(() => navigationService);
        }
    }
}