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
            SimpleIoc.Default.Register<AddModelPopupModel>();
            SimpleIoc.Default.Register<AddOptionPopupModel>();
            SimpleIoc.Default.Register<AddTimeTrialPopupModel>();
            SimpleIoc.Default.Register<ModifyModelViewModel>();
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

        public ModifyModelViewModel ModifyModel 
        {
            get
            {
                SimpleIoc.Default.Unregister<ModifyModelViewModel>();
                SimpleIoc.Default.Register<ModifyModelViewModel>();
                return ServiceLocator.Current.GetInstance<ModifyModelViewModel>();
            }
        }

        public OverrideModelViewModel OverrideModel 
        {
            get
            {
                SimpleIoc.Default.Unregister<OverrideModelViewModel>();
                SimpleIoc.Default.Register<OverrideModelViewModel>();
                return ServiceLocator.Current.GetInstance<OverrideModelViewModel>();
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
            SimpleIoc.Default.Unregister<HomeViewModel>();
            SimpleIoc.Default.Unregister<SupervisorViewModel>();
            SimpleIoc.Default.Unregister<AddModelPopupModel>();
            SimpleIoc.Default.Unregister<AddOptionPopupModel>();
            SimpleIoc.Default.Unregister<AddTimeTrialPopupModel>();
            SimpleIoc.Default.Unregister<ModifyModelViewModel>();
            SimpleIoc.Default.Unregister<OverrideModelViewModel>();
        }

        public static void setupNavigation()
        {
            var navigationService = new FrameNavigationService();

            navigationService.Configure("HomeView", new System.Uri("/View/HomeView.xaml", UriKind.Relative));
            navigationService.Configure("SupervisorView", new System.Uri("/View/SupervisorView.xaml", UriKind.Relative));
            navigationService.Configure("ModifyModelView", new System.Uri("/View/ModifyModelView.xaml", UriKind.Relative));
            navigationService.Configure("OverrideModelView", new System.Uri("/View/OverrideModelView.xaml", UriKind.Relative));

            SimpleIoc.Default.Unregister<IFrameNavigationService>();
            SimpleIoc.Default.Register<IFrameNavigationService>(() => navigationService);
        }
    }
}