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
        IEnumerable<Model> getModels();

        IEnumerable<Model> getFilteredModels(string modelFilter, string boxSizeFilter);

        IEnumerable<Option> getOptions();

        IEnumerable<Option> getFilteredOptions(string optionFilter, string optionBoxSizeFilter);

        Model getModel(string modelName);

        Override getModelOverride(string modelNum);

        decimal getTotalOptionsTime(string boxSize, List<string> options);

        IEnumerable<TimeTrial> getTimeTrials(string modelBase);

        IEnumerable<TimeTrial> getTimeTrials(string modelBase, List<string> options);

        IEnumerable<TimeTrial> getFilteredTimeTrials(string modelBase, string optionTextFilter, string salesFilter, string productionNumFilter);

        IEnumerable<TimeTrial> getStrictFilteredTimeTrials(string modelBase, string optionTextFilter, string salesFilter, string productionNumFilter);

        IEnumerable<Option> getModelOptions(List<string> optionsList, string boxSize);

        IEnumerable<string> getDriveTypes();

        IEnumerable<Model> getNumModelsFound(string drive, string av, string boxSize, bool exact);

        IEnumerable<string> getOptionCodes();

        IEnumerable<Option> getNumOptionsFound(string optionCode, string boxSize, bool exact);

        IEnumerable<Override> getOverrides();

        IEnumerable<Override> getFilteredOverrides(string overrideFilter);

        void addTimeTrials(ObservableCollection<TimeTrial> timeTrials);

        IEnumerable<Modification> getFilteredNewModels(string Sender, string Base, string BoxSize);
        IEnumerable<Modification> getFilteredNewOptions(string Sender, string OptionCode, string BoxSize);
        IEnumerable<Modification> getFilteredModifiedModels(string Sender, string ModelName);
        IEnumerable<Modification> getFilteredModifiedOptions(string Sender, string OptionCode, string BoxSize);

        IEnumerable<OverrideRequest> getFilteredOverrideRequests(string Sender, string ModelNum);

        void addModificationRequest(Modification mod);
    }
}
