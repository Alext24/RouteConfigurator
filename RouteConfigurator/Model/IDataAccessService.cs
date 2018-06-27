using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator.Model
{
    public interface IDataAccessService
    {
        //ObservableCollection<Day> getDays();
        Model getModel(string modelName);

        decimal getTotalOptionsTime(string boxSize, List<string> options);

        ObservableCollection<TimeTrial> getTimeTrials(string modelBase, List<string> options);
    }
}
