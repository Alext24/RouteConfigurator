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

        ObservableCollection<TimeTrial> getTimeTrials(string modelBase);

        ObservableCollection<TimeTrial> getTimeTrials(string modelBase, List<string> options);

        ObservableCollection<TimeTrial> getFilteredTimeTrials(string modelBase, string optionTextFilter, string salesFilter, string productionNumFilter);

        ObservableCollection<Option> getModelOptions(List<string> optionsList, string boxSize);

        ObservableCollection<string> getDriveTypes();

        ObservableCollection<Model> getNumModelsFound(string drive, string av, string boxSize);

        ObservableCollection<string> getOptionCodes();

        ObservableCollection<Option> getNumOptionsFound(string optionCode, string boxSize);

        ObservableCollection<Override> getOverrides();

        ObservableCollection<Override> getFilteredOverrides(string overrideFilter);

        void addTimeTrials(ObservableCollection<TimeTrial> timeTrials);

        ObservableCollection<Modification> getFilteredNewModels(string Sender, string Base, string BoxSize);
        ObservableCollection<Modification> getFilteredNewOptions(string Sender, string OptionCode, string BoxSize);
        ObservableCollection<Modification> getFilteredModifiedModels(string Sender, string ModelName);
        ObservableCollection<Modification> getFilteredModifiedOptions(string Sender, string OptionCode);
    }
}
