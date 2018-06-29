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
        ObservableCollection<Model> getModels();

        ObservableCollection<Model> getFilteredModels(string modelFilter, string boxSizeFilter);

        ObservableCollection<Option> getOptions();

        ObservableCollection<Option> getFilteredOptions(string optionFilter, string optionBoxSizeFilter);

        Model getModel(string modelName);

        Override getModelOverride(string modelNum);

        decimal getTotalOptionsTime(string boxSize, List<string> options);

        ObservableCollection<TimeTrial> getTimeTrials(string modelBase, List<string> options);

        ObservableCollection<Option> getModelOptions(List<string> optionsList, string boxSize);
    }
}
