using RouteConfigurator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RouteConfigurator.Design
{
    public class DataAccessService : IDataAccessService
    {
        //private WorkHoursDBContext context;
        private RouteConfiguratorDB context;

        public DataAccessService()
        {
            //   context = new WorkHoursDBContext();
            context = new RouteConfiguratorDB();
        }

        public Model.Model getModel(string modelName)
        {
            Model.Model returnModel = null;

            var model = context.Models.Find(modelName);

            if(model == null)
            {
                throw new Exception("Model not found");
            }

            returnModel = model as Model.Model;

            return returnModel;
        }

        public ObservableCollection<TimeTrial> getTimeTrials(string modelName)
        {
            ObservableCollection<TimeTrial> timeTrials = new ObservableCollection<TimeTrial>();

            foreach (var timeTrial in context.TimeTrials)
            {
                if (timeTrial.Model.ModelNum.Equals(modelName.ToUpper()))
                {
                    timeTrials.Add(timeTrial);
                }
            }

            return timeTrials;
        }
    }
}