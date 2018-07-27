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
        #region Model Read
        Model getModel(string modelName);
        IEnumerable<Model> getModels();
        IEnumerable<Model> getFilteredModels(string modelFilter, string boxSizeFilter);
        IEnumerable<string> getDriveTypes();
        IEnumerable<Model> getNumModelsFound(string drive, string av, string boxSize, bool exact);
        #endregion

        #region Option Read
        IEnumerable<Option> getOptions();
        IEnumerable<Option> getFilteredOptions(string optionFilter, string optionBoxSizeFilter, bool exact);
        IEnumerable<Option> getModelOptions(List<string> optionsList, string boxSize);
        decimal getTotalOptionsTime(string boxSize, List<string> options);
        IEnumerable<string> getOptionCodes();
        IEnumerable<Option> getNumOptionsFound(string optionCode, string boxSize, bool exact);
        #endregion

        #region Override Read
        Override getModelOverride(string modelNum);
        IEnumerable<Override> getOverrides();
        IEnumerable<Override> getFilteredOverrides(string overrideFilter);
        #endregion

        #region Time Trial Read
        TimeTrial getTimeTrial(int productionNumber);
        IEnumerable<TimeTrial> getTimeTrials(string modelBase);
        IEnumerable<TimeTrial> getTimeTrials(string modelBase, List<string> options);
        IEnumerable<TimeTrial> getFilteredTimeTrials(string modelBase, string optionTextFilter, string salesFilter, string productionNumFilter, bool exact);
        #endregion

        #region Modifications Read
        IEnumerable<Modification> getFilteredNewModels(string Sender, string Base, string BoxSize);
        IEnumerable<Modification> getFilteredNewOptions(string Sender, string OptionCode, string BoxSize);
        IEnumerable<Modification> getFilteredModifiedModels(string Sender, string ModelName);
        IEnumerable<Modification> getFilteredModifiedOptions(string Sender, string OptionCode, string BoxSize);
        #endregion

        #region Override Requests Read
        IEnumerable<OverrideRequest> getFilteredOverrideRequests(string Sender, string ModelNum);
        #endregion

        void addTimeTrials(ObservableCollection<TimeTrial> timeTrials);
        void addModificationRequest(Modification mod);
        void addOverrideRequest(OverrideRequest ov);
    }
}
