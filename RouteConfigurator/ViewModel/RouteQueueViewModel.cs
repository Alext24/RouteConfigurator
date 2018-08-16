using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Design;
using RouteConfigurator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RouteConfigurator.ViewModel
{
    public class RouteQueueViewModel : ViewModelBase
    {
        #region PrivateVariables
        /// <summary>
        /// Navigation service to help navigate to other pages
        /// </summary>
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Data access service to retrieve data from a data source
        /// </summary>
        private IDataAccessService _serviceProxy = new DataAccessService();

        private ObservableCollection<RouteQueue> _routes = new ObservableCollection<RouteQueue>();

        private RouteQueue _selectedRoute;

        private string _modelNumberFilter = "";

        private string _informationText;

        private bool _loading = false;
        #endregion

        #region RelayCommands
        public RelayCommand loadedCommand { get; set; }
        public RelayCommand deleteCommand { get; set; }
        public RelayCommand submitRoutesCommand { get; set; }
        public RelayCommand goBackCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public RouteQueueViewModel (IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            loadedCommand = new RelayCommand(loaded);
            deleteCommand = new RelayCommand(delete);
            submitRoutesCommand = new RelayCommand(submitRoutes);
            goBackCommand = new RelayCommand(goBack);
        }
        #endregion

        #region Commands
        private void loaded()
        {
            updateRouteQueuesTableAsync();
        }

        private void delete()
        {
            informationText = "";
            if (selectedRoute != null)
            {
                try
                {
                    _serviceProxy.deleteQueuedRoute(selectedRoute);
                    updateRouteQueuesTableAsync();
                    informationText = "Deleted route";
                }
                catch (Exception e)
                {
                    informationText = "There was a problem accessing the database.";
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void submitRoutes()
        {
            informationText = "";
            string tempSubmissionString = "Placeholder for real submission\n";
            foreach(RouteQueue route in routes)
            {
                if (route.IsApproved)
                {
                    tempSubmissionString += string.Format("Submitted model {0} with route {1}.\n", route.ModelNumber, route.Route);
                }
            }
            MessageBox.Show(tempSubmissionString);
        }

        private void goBack()
        {
            _navigationService.GoBack();
        }
        #endregion

        #region Public Variables
        public ObservableCollection<RouteQueue> routes 
        {
            get { return _routes; }
            set
            {
                _routes = value;
                RaisePropertyChanged("routes");
            }
        }

        public RouteQueue selectedRoute 
        {
            get { return _selectedRoute; }
            set
            {
                _selectedRoute = value;
                RaisePropertyChanged("selectedRoute");
                informationText = "";
            }
        }

        /// <summary>
        /// Calls updateRouteQueuesTableAsync
        /// </summary>
        public string modelNumberFilter
        {
            get { return _modelNumberFilter; }
            set
            {
                _modelNumberFilter = value.ToUpper();
                RaisePropertyChanged("modelNumberFilter");
                informationText = "";

                updateRouteQueuesTableAsync();
            }
        }
        
        public string informationText 
        {
            get
            {
                return _informationText;
            }
            set
            {
                _informationText = value;
                RaisePropertyChanged("informationText");
            }
        }

        public bool loading
        {
            get
            {
                return _loading;
            }
            set
            {
                _loading = value;
                RaisePropertyChanged("loading");
            }
        }
        #endregion

        #region Private Functions
        private async void updateRouteQueuesTableAsync()
        {
            loading = true;
            informationText = "Loading tables...";
            await Task.Run(() => updateRouteQueuesTable());
            loading = false;
            informationText = "";
        }

        private void updateRouteQueuesTable()
        {
            try
            {
                routes = new ObservableCollection<RouteQueue>(_serviceProxy.getFilteredRouteQueues(modelNumberFilter));
            }
            catch (Exception e)
            {
                informationText = "There was a problem accessing the database";
                Console.WriteLine(e);
            }
        }
        #endregion
    }
}
