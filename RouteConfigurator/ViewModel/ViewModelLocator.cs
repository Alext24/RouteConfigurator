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
using RouteConfigurator.ViewModelEngineered;
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
            SimpleIoc.Default.Register<ManagerViewModel>();
            SimpleIoc.Default.Register<AddModelPopupModel>();
            SimpleIoc.Default.Register<AddOptionPopupModel>();
            SimpleIoc.Default.Register<AddTimeTrialPopupModel>();
            SimpleIoc.Default.Register<ModifyModelPopupModel>();
            SimpleIoc.Default.Register<OverrideModelPopupModel>();
            SimpleIoc.Default.Register<ModifyOptionPopupModel>();
            SimpleIoc.Default.Register<RequestsViewModel>();

            SimpleIoc.Default.Register<EngineeredHomeViewModel>();
            SimpleIoc.Default.Register<EngineeredSupervisorViewModel>();
            SimpleIoc.Default.Register<AddComponentPopupModel>();
            SimpleIoc.Default.Register<ModifyComponentsPopupModel>();
            SimpleIoc.Default.Register<ModifyEnclosuresPopupModel>();
        }

        public static void setupNavigation()
        {
            var navigationService = new FrameNavigationService();

            navigationService.Configure("HomeView", new System.Uri("/View/HomeView.xaml", UriKind.Relative));
            navigationService.Configure("SupervisorView", new System.Uri("/View/SupervisorView.xaml", UriKind.Relative));
            navigationService.Configure("ManagerView", new System.Uri("/View/ManagerView.xaml", UriKind.Relative));

            navigationService.Configure("EngineeredHomeView", new System.Uri("/ViewEngineered/EngineeredHomeView.xaml", UriKind.Relative));
            navigationService.Configure("EngineeredSupervisorView", new System.Uri("/ViewEngineered/EngineeredSupervisorView.xaml", UriKind.Relative));

            SimpleIoc.Default.Unregister<IFrameNavigationService>();
            SimpleIoc.Default.Register<IFrameNavigationService>(() => navigationService);
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            SimpleIoc.Default.Unregister<HomeViewModel>();
            SimpleIoc.Default.Unregister<SupervisorViewModel>();
            SimpleIoc.Default.Unregister<ManagerViewModel>();
            SimpleIoc.Default.Unregister<AddModelPopupModel>();
            SimpleIoc.Default.Unregister<AddOptionPopupModel>();
            SimpleIoc.Default.Unregister<AddTimeTrialPopupModel>();
            SimpleIoc.Default.Unregister<ModifyModelPopupModel>();
            SimpleIoc.Default.Unregister<OverrideModelPopupModel>();
            SimpleIoc.Default.Unregister<ModifyOptionPopupModel>();
            SimpleIoc.Default.Unregister<RequestsViewModel>();

            SimpleIoc.Default.Unregister<EngineeredHomeViewModel>();
            SimpleIoc.Default.Unregister<EngineeredSupervisorViewModel>();
            SimpleIoc.Default.Unregister<AddComponentPopupModel>();
            SimpleIoc.Default.Unregister<ModifyComponentsPopupModel>();
            SimpleIoc.Default.Unregister<ModifyEnclosuresPopupModel>();
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

        public ManagerViewModel Manager 
        {
            get
            {
                SimpleIoc.Default.Unregister<ManagerViewModel>();
                SimpleIoc.Default.Register<ManagerViewModel>();
                return ServiceLocator.Current.GetInstance<ManagerViewModel>();
            }
        }

        public AddModelPopupModel AddModel
        {
            get
            {
                SimpleIoc.Default.Unregister<AddModelPopupModel>();
                SimpleIoc.Default.Register<AddModelPopupModel>();
                return ServiceLocator.Current.GetInstance<AddModelPopupModel>();
            }
        }

        public AddOptionPopupModel AddOption
        {
            get
            {
                SimpleIoc.Default.Unregister<AddOptionPopupModel>();
                SimpleIoc.Default.Register<AddOptionPopupModel>();
                return ServiceLocator.Current.GetInstance<AddOptionPopupModel>();
            }
        }

        public AddTimeTrialPopupModel AddTimeTrial 
        {
            get
            {
                SimpleIoc.Default.Unregister<AddTimeTrialPopupModel>();
                SimpleIoc.Default.Register<AddTimeTrialPopupModel>();
                return ServiceLocator.Current.GetInstance<AddTimeTrialPopupModel>();
            }
        }

        public ModifyModelPopupModel ModifyModel 
        {
            get
            {
                SimpleIoc.Default.Unregister<ModifyModelPopupModel>();
                SimpleIoc.Default.Register<ModifyModelPopupModel>();
                return ServiceLocator.Current.GetInstance<ModifyModelPopupModel>();
            }
        }

        public OverrideModelPopupModel OverrideModel 
        {
            get
            {
                SimpleIoc.Default.Unregister<OverrideModelPopupModel>();
                SimpleIoc.Default.Register<OverrideModelPopupModel>();
                return ServiceLocator.Current.GetInstance<OverrideModelPopupModel>();
            }
        }

        public ModifyOptionPopupModel ModifyOption
        {
            get
            {
                SimpleIoc.Default.Unregister<ModifyOptionPopupModel>();
                SimpleIoc.Default.Register<ModifyOptionPopupModel>();
                return ServiceLocator.Current.GetInstance<ModifyOptionPopupModel>();
            }
        }

        public RequestsViewModel Requests 
        {
            get
            {
                SimpleIoc.Default.Unregister<RequestsViewModel>();
                SimpleIoc.Default.Register<RequestsViewModel>();
                return ServiceLocator.Current.GetInstance<RequestsViewModel>();
            }
        }

        public EngineeredHomeViewModel EngineeredHome
        {
            get
            {
                SimpleIoc.Default.Unregister<EngineeredHomeViewModel>();
                SimpleIoc.Default.Register<EngineeredHomeViewModel>();
                return ServiceLocator.Current.GetInstance<EngineeredHomeViewModel>();
            }
        }

        public EngineeredSupervisorViewModel EngineeredSupervisor
        {
            get
            {
                SimpleIoc.Default.Unregister<EngineeredSupervisorViewModel>();
                SimpleIoc.Default.Register<EngineeredSupervisorViewModel>();
                return ServiceLocator.Current.GetInstance<EngineeredSupervisorViewModel>();
            }
        }

        public AddComponentPopupModel AddComponent 
        {
            get
            {
                SimpleIoc.Default.Unregister<AddComponentPopupModel>();
                SimpleIoc.Default.Register<AddComponentPopupModel>();
                return ServiceLocator.Current.GetInstance<AddComponentPopupModel>();
            }
        }

        public ModifyComponentsPopupModel ModifyComponents
        {
            get
            {
                SimpleIoc.Default.Unregister<ModifyComponentsPopupModel>();
                SimpleIoc.Default.Register<ModifyComponentsPopupModel>();
                return ServiceLocator.Current.GetInstance<ModifyComponentsPopupModel>();
            }
        }

        public ModifyEnclosuresPopupModel ModifyEnclosures
        {
            get
            {
                SimpleIoc.Default.Unregister<ModifyEnclosuresPopupModel>();
                SimpleIoc.Default.Register<ModifyEnclosuresPopupModel>();
                return ServiceLocator.Current.GetInstance<ModifyEnclosuresPopupModel>();
            }
        }
    }
}