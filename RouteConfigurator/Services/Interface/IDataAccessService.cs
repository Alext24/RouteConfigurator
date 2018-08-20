using RouteConfigurator.Model;
using RouteConfigurator.Model.EF_EngineeredModels;
using RouteConfigurator.Model.EF_StandardModels;
using RouteConfigurator.ViewModel.EngineeredModelViewModel.Helper;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RouteConfigurator.Services.Interface
{
    public interface IDataAccessService
    {
        #region Model Read
        StandardModel getModel(string modelName);
        IEnumerable<StandardModel> getModels();
        IEnumerable<StandardModel> getFilteredModels(string modelFilter, string boxSizeFilter);
        IEnumerable<string> getDriveTypes();
        IEnumerable<StandardModel> getModelsFound(string drive, string av, string boxSize, bool exact);
        #endregion

        #region Option Read
        IEnumerable<Option> getOptions();
        IEnumerable<Option> getFilteredOptions(string optionFilter, string optionBoxSizeFilter, bool exact);
        IEnumerable<Option> getModelOptions(List<string> optionsList, string boxSize);
        decimal getTotalOptionsTime(string boxSize, List<string> options);
        IEnumerable<string> getOptionCodes();
        IEnumerable<Option> getOptionsFound(string optionCode, string boxSize, bool exact);
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
        IEnumerable<Modification> getFilteredModifiedModels(string Sender, string ModelBase);
        IEnumerable<Modification> getFilteredModifiedOptions(string Sender, string OptionCode, string BoxSize);

        bool checkDuplicateOverrideDeletion(Modification mod);
        #endregion

        #region Override Requests Read
        IEnumerable<OverrideRequest> getOverrideRequests();
        IEnumerable<OverrideRequest> getFilteredOverrideRequests(string Sender, string ModelNum);
        IEnumerable<OverrideRequest> getFilteredOverrideRequests(int State, string ModelNum, string Sender, string Reviewer);
        #endregion

        #region Modify Standard
        void addTimeTrials(ObservableCollection<TimeTrial> timeTrials);
        void addModificationRequest(Modification mod);
        void addOverrideRequest(OverrideRequest ov);
        void addModel(StandardModel model);
        void addOption(Option option);
        void updateModel(string modelBase, decimal newDriveTime, decimal newAVTime);
        void updateOption(string optionCode, string boxSize, decimal newTime, string newName);
        void deleteOverride(string modelNum);
        void addOverride(Override ov, string modelBase);

        void updateModification(Modification modification);
        void updateOverrideRequest(OverrideRequest overrideRequest);

        void deleteTimeTrial(TimeTrial tt);
        #endregion

        IEnumerable<EngineeredModelComponentEntry> getModelComponents();

        #region Components Read
        Component getComponent(string name, string enclosureSize);
        IEnumerable<string> getComponentNames();
        IEnumerable<Component> getEnclosureSizeComponents(string enclosureSize);
        IEnumerable<Component> getFilteredComponents(string name, string enclosureSize);
        #endregion

        #region Enclosure Read
        Enclosure getEnclosure(string enclosureType, string enclosureSize);
        IEnumerable<string> getEnclosureTypes();
        IEnumerable<string> getEnclosureSizes();
        IEnumerable<Enclosure> getFilteredEnclosures(string enclosureType, string enclosureSize);
        IEnumerable<Enclosure> getExactEnclosures(string enclosureType, string enclosureSize);
        #endregion

        #region Wire Gauge Read
        WireGauge getWireGauge(string gauge);
        IEnumerable<WireGauge> getWireGauges();
        IEnumerable<WireGauge> getFilteredWireGauges(string guage);
        #endregion

        #region Engineered Modifications Read
        IEnumerable<EngineeredModification> getFilteredEngineeredModifications(string ComponentName, string EnclosureSize, string EnclosureType, string Gauge, string Sender, string Reviewer);
        IEnumerable<EngineeredModification> getFilteredWaitingEngineeredModifications(string ComponentName, string EnclosureSize, string EnclosureType, string Gauge, string Sender, string Reviewer);
        IEnumerable<EngineeredModification> getFilteredStateEngineeredModifications(int State, string ComponentName, string EnclosureSize, string EnclosureType, string Gauge, string Sender, string Reviewer);

        IEnumerable<EngineeredModification> getFilteredNewComponents(string Sender, string ComponentName, string EnclosureSize);
        IEnumerable<EngineeredModification> getFilteredModifiedComponents(string Sender, string ComponentName, string EnclosureSize);
        IEnumerable<EngineeredModification> getFilteredModifiedEnclosures(string Sender, string EnclosureSize, string EnclosureType);
        IEnumerable<EngineeredModification> getFilteredWireGaugeMods(string Sender, string Gauge, bool IsNew);
        IEnumerable<EngineeredModification> getNewWireGaugeMods(string Gauge);
        #endregion

        #region Modify Engineered
        void addEngineeredModificationRequest(EngineeredModification mod);

        void addComponent(Component component);
        void addWireGauge(WireGauge wireGauge);

        void updateComponent(string name, string enclosureSize, decimal newTime);
        void updateEnclosure(string enclosureType, string enclosureSize, decimal newTime);
        void updateWireGauge(string gauge, decimal newTimePercentage);

        void updateEngineeredModification(EngineeredModification modification);
        #endregion

        #region RouteQueue
        IEnumerable<RouteQueue> getFilteredRouteQueues(string modelNumber);
        void addRouteQueue(RouteQueue route);
        void deleteQueuedRoute(RouteQueue selectedRoute);
        #endregion
    }
}
