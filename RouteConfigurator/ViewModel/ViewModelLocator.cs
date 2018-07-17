﻿/*
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
            SimpleIoc.Default.Register<ModifyModelPopupModel>();
            SimpleIoc.Default.Register<OverrideModelPopupModel>();
            SimpleIoc.Default.Register<ModifyOptionPopupModel>();
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
            SimpleIoc.Default.Unregister<ModifyModelPopupModel>();
            SimpleIoc.Default.Unregister<OverrideModelPopupModel>();
            SimpleIoc.Default.Unregister<ModifyOptionPopupModel>();
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