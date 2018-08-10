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
        IEnumerable<Modification> getModifications();

        IEnumerable<Modification> getFilteredModifications(string ModelBase, string BoxSize, string OptionCode, string Sender, string Reviewer);
        IEnumerable<Modification> getFilteredWaitingModifications(string ModelBase, string BoxSize, string OptionCode, string Sender, string Reviewer);
        IEnumerable<Modification> getFilteredStateModifications(int State, string ModelBase, string BoxSize, string OptionCode, string Sender, string Reviewer);

        IEnumerable<Modification> getFilteredNewModels(string Sender, string Base, string BoxSize);
        IEnumerable<Modification> getFilteredNewOptions(string Sender, string OptionCode, string BoxSize);
        IEnumerable<Modification> getFilteredModifiedModels(string Sender, string ModelName);
        IEnumerable<Modification> getFilteredModifiedOptions(string Sender, string OptionCode, string BoxSize);

        bool checkDuplicateOverrideDeletion(Modification mod);
        #endregion

        #region Override Requests Read
        IEnumerable<OverrideRequest> getOverrideRequests();
        IEnumerable<OverrideRequest> getFilteredOverrideRequests(string Sender, string ModelNum);
        IEnumerable<OverrideRequest> getFilteredOverrideRequests(int State, string ModelNum, string Sender, string Reviewer);
        #endregion

        void addTimeTrials(ObservableCollection<TimeTrial> timeTrials);
        void addModificationRequest(Modification mod);
        void addOverrideRequest(OverrideRequest ov);
        void addModel(Model model);
        void addOption(Option option);
        void updateModel(string modelBase, decimal newDriveTime, decimal newAVTime);
        void updateOption(string optionCode, string boxSize, decimal newTime, string newName);
        void deleteOverride(string modelNum);
        void addOverride(Override ov, string modelBase);

        void updateModification(Modification modification);
        void updateOverrideRequest(OverrideRequest overrideRequest);

        void deleteTimeTrial(TimeTrial tt);

        #region Engineered Orders
        IEnumerable<string> getEnclosureTypes();
        IEnumerable<string> getEnclosureSizes();
        IEnumerable<WireGauge> getWireGauges();

        IEnumerable<Component> getFilteredComponents(string name, string enclosureSize);
        IEnumerable<Enclosure> getFilteredEnclosures(string enclosureType, string enclosureSize);
        IEnumerable<WireGauge> getFilteredWireGauges(string wireGuage);

        void addEngineeredModificationRequest(EngineeredModification mod);
        #endregion
    }
}
