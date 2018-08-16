using RouteConfigurator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.Entity;
using RouteConfigurator.DTOs;
using RouteConfigurator.Model.Context;
using RouteConfigurator.Model.EF_EngineeredModels;
using RouteConfigurator.Model.EF_StandardModels;
using RouteConfigurator.Services.Interface;

namespace RouteConfigurator.Services
{
    public class DataAccessService : IDataAccessService
    {
        #region Constructor
        public DataAccessService() { }
        #endregion

        #region Model Read
        /// <param name="modelName"> primary key for Models </param>
        /// <remarks> modelName: base name for a model (first 8 characters) </remarks>
        /// <returns> returns the model specified by the model name </returns>
        public StandardModel getModel(string modelName)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Models.Find(modelName) ;
            }
        }

        /// <returns> returns all models </returns>
        public IEnumerable<StandardModel> getModels()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Models.ToList();
            }
        }

        /// <param name="modelFilter"> base name for the model (first 8 characters) </param>
        /// <param name="boxSizeFilter"> box size for the model </param>
        /// <returns> returns a list of the models that contain the filters</returns>
        public IEnumerable<StandardModel> getFilteredModels(string modelFilter, string boxSizeFilter)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Models.Where(model => model.Base.Contains(modelFilter) &&
                                                     model.BoxSize.Contains(boxSizeFilter)).ToList();
            }
        }

        /// <returns> returns a list of unique drive types (first 4 characters of the model base) 
        ///           ex. A1C1 or Z1B1 </returns>
        public IEnumerable<string> getDriveTypes()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Models.Select(x => x.Base.Substring(0, 4)).ToList().Distinct();
            }
        }

        /// <param name="drive"> Drive for the model (first 4 characters of the model base) ex. A1C1 or Z1B1 </param>
        /// <param name="av"> Amps and Voltage for the model (second 4 characters of the model base) ex. A002 or D124 </param>
        /// <param name="boxSize"> box size for the model </param>
        /// <param name="exact"> determines if the box size needs to be equal to or just contained in the model's box size </param>
        /// <returns> returns a list of models that contain the filters</returns>
        public IEnumerable<StandardModel> getModelsFound(string drive, string av, string boxSize, bool exact)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                if (exact)
                {
                    return context.Models.Where(x => x.Base.Contains(drive) &&
                                                     x.Base.Contains(av) &&
                                                     x.BoxSize.Equals(boxSize)).ToList();
                }
                else
                {
                    return context.Models.Where(x => x.Base.Contains(drive) &&
                                                     x.Base.Contains(av) &&
                                                     x.BoxSize.Contains(boxSize)).ToList();
                }
            }
        }
        #endregion

        #region Option Read
        /// <returns> returns all options </returns>
        public IEnumerable<Option> getOptions()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Options.ToList();
            }
        }

        /// <param name="optionFilter"> option code for the option ex. PB or TW </param>
        /// <param name="optionBoxSizeFilter"> box size for the option </param>
        /// <param name="exact"> determines if the box size needs to be equal to or just contained in the model's box size </param>
        /// <returns> returns a list of the options that contain the filters</returns>
        public IEnumerable<Option> getFilteredOptions(string optionFilter, string optionBoxSizeFilter, bool exact)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                if (exact)
                {
                    return context.Options.Where(option => option.OptionCode.Contains(optionFilter) &&
                                                           option.BoxSize.Equals(optionBoxSizeFilter)).ToList();
                }
                else
                {
                    return context.Options.Where(option => option.OptionCode.Contains(optionFilter) &&
                                                           option.BoxSize.Contains(optionBoxSizeFilter)).ToList();
                }
            }
        }

        /// <summary> Takes in a list of option codes that are part of a model and returns a 
        ///           list of the actual options that were found in the database that matched the
        ///           option code and box size of the passed in optionsList </summary>
        /// <param name="optionsList"> option codes in string format to find actual options for </param>
        /// <param name="boxSize"> box size to find the options for </param>
        /// <returns> returns a list of options that were found from optionsList </returns>
        public IEnumerable<Option> getModelOptions(List<string> optionsList, string boxSize)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Options.Where(x => optionsList.Any(optionCode => x.OptionCode.Contains(optionCode)) &&
                                                                                x.BoxSize.Equals(boxSize)).ToList();
            }
        }

        /// <summary> Takes in a list of option codes that are part of a model and returns the
        ///           total time that these options with the box size are predicted to take </summary>
        /// <param name="boxSize"> box size for the options </param>
        /// <param name="options"> option codes in string format to find actual options for </param>
        /// <returns> total time that the list of options is predicted to take </returns>
        public decimal getTotalOptionsTime(string boxSize, List<string> options)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Options.Where(x => x.BoxSize.Equals(boxSize) &&
                                                  options.Contains(x.OptionCode)).Sum(y => y.Time);
            }
        }

        /// <returns> returns a list of unique option codes </returns>
        public IEnumerable<string> getOptionCodes()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Options.Select(x => x.OptionCode).ToList().Distinct();
            }
        }

        /// <remarks> optionCode and boxSize should be "" instead of null to allow all options to be returned
        ///           when neither are entered. </remarks>
        /// <param name="optionCode"> option code to filter for </param>
        /// <param name="boxSize"> box size for the option </param>
        /// <param name="exact"> determines if the box size needs to be equal to or just contained in the option's box size </param>
        /// <returns> returns a list of options that contain the filters </returns>
        public IEnumerable<Option> getOptionsFound(string optionCode, string boxSize, bool exact)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                if (exact)
                {
                    return context.Options.Where(option => option.OptionCode.Contains(optionCode) &&
                                                           option.BoxSize.Equals(boxSize)).ToList();
                }
                else
                {
                    return context.Options.Where(option => option.OptionCode.Contains(optionCode) &&
                                                           option.BoxSize.Contains(boxSize)).ToList();
                }
            }
        }
        #endregion

        #region Override Read
        /// <param name="modelNum"> the entire model number (Base 8 characters plus all options) </param>
        /// <returns> returns the override for a model if one exists, otherwise null </returns>
        public Override getModelOverride(string modelNum)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Overrides.Find(modelNum);
            }
        }

        /// <returns> returns a list of active overrides</returns>
        public IEnumerable<Override> getOverrides()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Overrides.Include(a => a.Model).ToList();
            }
        }

        /// <param name="overrideFilter"> model number (Base 8 characters plus all options) to filter overrides by </param>
        /// <returns> returns a list of active overrides that contain the model number </returns>
        public IEnumerable<Override> getFilteredOverrides(string overrideFilter)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Overrides.Include(a => a.Model).Where(x => x.ModelNum.Contains(overrideFilter)).ToList();
            }
        }
        #endregion

        #region Time Trial Read
        /// <param name="productionNumber"> Primary key for Time Trials </param>
        /// <returns> returns the time trial with the specified production number </returns>
        public TimeTrial getTimeTrial(int productionNumber)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.TimeTrials.Find(productionNumber);
            }
        }

        /// <param name="modelBase"> model base to get time trials for </param>
        /// <remarks> modelBase: first 8 characters </remarks>
        /// <returns> returns a list of time trials for the model base </returns>
        public IEnumerable<TimeTrial> getTimeTrials(string modelBase)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.TimeTrials.Include(a => a.TTOptionTimes).Where(x => x.Model.Base.Equals(modelBase)).ToList();
            }
        }

        /// <summary> Finds the time trials that have been entered for a specific model number
        ///           by matching the model base with the time trial model base and matching all
        ///           of the options with the time trials option times </summary>
        /// <param name="modelBase"> model base (first 8 characters) </param>
        /// <param name="options"> list of option codes </param>
        /// <returns> returns a list of time trials for a specific model number </returns>
        public IEnumerable<TimeTrial> getTimeTrials(string modelBase, List<string> options)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.TimeTrials.Include(a => a.Model).Include(a => a.TTOptionTimes)
                                         .Where(x => x.Model.Base.Equals(modelBase) &&
                                                     x.TTOptionTimes.Count == options.Count &&
                                                     x.TTOptionTimes.All(y => options.Contains(y.OptionCode))).ToList();
            }
        }

        /// <param name="modelBase"> model base (first 8 characters) </param>
        /// <param name="optionTextFilter"> option text for the model number </param>
        /// <remarks> option text: combination of model options into one string ex. PBCTWZ </remarks>
        /// <param name="salesFilter"> sales number for the time trials</param>
        /// <param name="productionNumFilter"> production number for the time trials</param>
        /// <param name="exact"> determines if the options text needs to be equal to or just contained in the time trial's option text </param>
        /// <returns> returns a list of time trials that meet the filters</returns>
        public IEnumerable<TimeTrial> getFilteredTimeTrials(string modelBase, string optionTextFilter, string salesFilter, string productionNumFilter, bool exact)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                if (exact)
                {
                    return context.TimeTrials.Include(a => a.Model).Include(a => a.TTOptionTimes)
                                             .Where(x => x.Model.Base.Contains(modelBase) &&
                                                         x.OptionsText.Equals(optionTextFilter) &&
                                                         x.SalesOrder.ToString().Contains(salesFilter) &&
                                                         x.ProductionNumber.ToString().Contains(productionNumFilter)).ToList();
                }
                else
                {
                    return context.TimeTrials.Include(a => a.Model).Include(a => a.TTOptionTimes)
                                             .Where(x => x.Model.Base.Contains(modelBase) &&
                                                         x.OptionsText.Contains(optionTextFilter) &&
                                                         x.SalesOrder.ToString().Contains(salesFilter) &&
                                                         x.ProductionNumber.ToString().Contains(productionNumFilter)).ToList();
                }
            }
        }
        #endregion

        #region Modifications Read
        /// <returns> returns a list of all modifications </returns>
        public IEnumerable<Modification> getModifications()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Modifications.ToList();
            }
        }

        /// <remarks> Includes all states in the returned list but filters the other parameters </remarks>
        /// <param name="ModelBase"> model base (first 8 characters) </param>
        /// <param name="BoxSize"> model or option box size </param>
        /// <param name="OptionCode"> option code of the option </param>
        /// <param name="Sender"> user who sent the request </param>
        /// <param name="Reviewer"> user who approved or declined the request </param>
        /// <returns> returns a list of the modifications that meet the filters </returns>
        public IEnumerable<Modification> getFilteredModifications(string ModelBase, string BoxSize, string OptionCode, string Sender, string Reviewer)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Modifications.Where(x => x.ModelBase.Contains(ModelBase) &&
                                                        x.BoxSize.Contains(BoxSize) &&
                                                        x.OptionCode.Contains(OptionCode) &&
                                                        x.Sender.Contains(Sender) &&
                                                        x.Reviewer.Contains(Reviewer)).ToList();
            }
        }

        /// <remarks> Includes only the waiting states (state = 0 (waiting), 3 (currently checked to approve), or 4 (currently checked to decline))
        ///           in the returned list and filters the other parameters </remarks>
        /// <param name="ModelBase"> model base (first 8 characters) </param>
        /// <param name="BoxSize"> model or option box size </param>
        /// <param name="OptionCode"> option code of the option </param>
        /// <param name="Sender"> user who sent the request </param>
        /// <param name="Reviewer"> user who approved or declined the request </param>
        /// <returns> returns a list of the modifications that meet the filters </returns>
        public IEnumerable<Modification> getFilteredWaitingModifications(string ModelBase, string BoxSize, string OptionCode, string Sender, string Reviewer)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Modifications.Where(x => (x.State == 0 || x.State == 3 || x.State == 4) &&
                                                         x.ModelBase.Contains(ModelBase) &&
                                                         x.BoxSize.Contains(BoxSize) &&
                                                         x.OptionCode.Contains(OptionCode) &&
                                                         x.Sender.Contains(Sender) &&
                                                         x.Reviewer.Contains(Reviewer)).ToList();
            }
        }

        /// <remarks> Used to filter for approved state or declined state modifications while still 
        ///           filtering for the other parameters </remarks>
        /// <param name="State"> current state of the modification </param>
        /// <param name="ModelBase"> model base (first 8 characters) </param>
        /// <param name="BoxSize"> model or option box size </param>
        /// <param name="OptionCode"> option code of the option </param>
        /// <param name="Sender"> user who sent the request </param>
        /// <param name="Reviewer"> user who approved or declined the request </param>
        /// <returns> returns a list of the modifications that meet the filters </returns>
        public IEnumerable<Modification> getFilteredStateModifications(int State, string ModelBase, string BoxSize, string OptionCode, string Sender, string Reviewer)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Modifications.Where(x => x.State == State &&
                                                        x.ModelBase.Contains(ModelBase) &&
                                                        x.BoxSize.Contains(BoxSize) &&
                                                        x.OptionCode.Contains(OptionCode) &&
                                                        x.Sender.Contains(Sender) &&
                                                        x.Reviewer.Contains(Reviewer)).ToList();
            }
        }

        /// <param name="Sender"> User who sent the modification </param>
        /// <param name="Base"> model base (first 8 characters) </param>
        /// <param name="BoxSize"> box size for the new model </param>
        /// <returns> returns a list of unapproved new models requests that meet the filters</returns>
        public IEnumerable<Modification> getFilteredNewModels(string Sender, string Base, string BoxSize)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Modifications.Where(x => x.IsOption == false && x.IsNew == true &&
                                                        x.State == 0 &&
                                                        x.Sender.Contains(Sender) &&
                                                        x.ModelBase.Contains(Base) &&
                                                        x.BoxSize.Contains(BoxSize)).ToList();
            }
        }

        /// <param name="Sender"> User who sent the modification </param>
        /// <param name="OptionCode"> Option code </param>
        /// <param name="BoxSize"> Box size for the new option</param>
        /// <returns> returns a list of unapproved new options requests that meet the filters</returns>
        public IEnumerable<Modification> getFilteredNewOptions(string Sender, string OptionCode, string BoxSize)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Modifications.Where(x => x.IsOption == true && x.IsNew == true &&
                                                        x.State == 0 &&
                                                        x.Sender.Contains(Sender) &&
                                                        x.OptionCode.Contains(OptionCode) &&
                                                        x.BoxSize.Contains(BoxSize)).ToList();
            }
        }

        /// <param name="Sender"> User who sent the modification </param>
        /// <param name="ModelBase"> Model base (first 8 characters) </param>
        /// <returns> List of unapproved modified model requests that meet the filters</returns>
        public IEnumerable<Modification> getFilteredModifiedModels(string Sender, string ModelBase)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Modifications.Where(x => x.IsOption == false && x.IsNew == false &&
                                                        x.State == 0 &&
                                                        x.Sender.Contains(Sender) &&
                                                        x.ModelBase.Contains(ModelBase)).ToList();
            }
        }

        /// <param name="Sender"> User who sent the modification </param>
        /// <param name="OptionCode"> Option code </param>
        /// <param name="BoxSize"> Box size for the option </param>
        /// <returns> List of unapproved modified option requests that meet the filters </returns>
        public IEnumerable<Modification> getFilteredModifiedOptions(string Sender, string OptionCode, string BoxSize)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Modifications.Where(x => x.IsOption == true && x.IsNew == false &&
                                                        x.State == 0 &&
                                                        x.Sender.Contains(Sender) &&
                                                        x.OptionCode.Contains(OptionCode) &&
                                                        x.BoxSize.Contains(BoxSize)).ToList();
            }
        }

        /// <summary> Searches the current waiting modifications to check if the same 
        ///           override is already waiting to be deleted </summary>
        /// <param name="mod"> modification that provides the description to match 
        ///                    to the possible duplicate deletion </param>
        /// <returns> returns true if override is waiting to be deleted, false otherwise </returns>
        public bool checkDuplicateOverrideDeletion(Modification mod)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return (context.Modifications.Where(x => x.Description.Equals(mod.Description) &&
                                                        (x.State == 0 || x.State == 3 || x.State == 4)).FirstOrDefault() == null ? false : true);
            }
        }
        #endregion

        #region Override Requests Read
        /// <returns> returns a list of all overrides requests </returns>
        public IEnumerable<OverrideRequest> getOverrideRequests()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.OverrideRequests.ToList();
            }
        }

        /// <param name="Sender"> User who sent the override request </param>
        /// <param name="ModelNum"> Full model number with options </param>
        /// <returns> returns a list of override requests that meet the filters</returns>
        public IEnumerable<OverrideRequest> getFilteredOverrideRequests(string Sender, string ModelNum)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.OverrideRequests.Where(x => x.State == 0 &&
                                                           x.Sender.Contains(Sender) &&
                                                           x.ModelNum.Contains(ModelNum)).ToList();
            }
        }

        /// <param name="State"> current state of the override request </param>
        /// <param name="ModelNum"> full model number with options </param>
        /// <param name="Sender"> user who sent the request </param>
        /// <param name="Reviewer"> user who approved or declined the request </param>
        /// <returns> returns a list of the override requests that meet the filters </returns>
        public IEnumerable<OverrideRequest> getFilteredOverrideRequests(int State, string ModelNum, string Sender, string Reviewer)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                // Include all states but filter other parameters
                if(State == -1)
                {
                    return context.OverrideRequests.Where(x => x.ModelNum.Contains(ModelNum) &&
                                                               x.Sender.Contains(Sender) &&
                                                               x.Reviewer.Contains(Reviewer)).ToList();
                }
                // If filtered for waiting state, also include states 3 and 4 (Currently checked to approve or decline)
                else if (State == 0)
                {
                    return context.OverrideRequests.Where(x => (x.State == 0 || x.State == 3 || x.State == 4) &&
                                                                x.ModelNum.Contains(ModelNum) &&
                                                                x.Sender.Contains(Sender) &&
                                                                x.Reviewer.Contains(Reviewer)).ToList();
                }
                // Otherwise make sure the state is factored into the filters
                else
                {
                    return context.OverrideRequests.Where(x => x.State == State &&
                                                               x.ModelNum.Contains(ModelNum) &&
                                                               x.Sender.Contains(Sender) &&
                                                               x.Reviewer.Contains(Reviewer)).ToList();
                }
            }
        }
        #endregion

        #region Modify Standard
        /// <summary>
        /// Adds a list of time trials to the database
        /// </summary>
        /// <param name="timeTrials"> list of new time trials </param>
        public void addTimeTrials(ObservableCollection<TimeTrial> timeTrials)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                foreach (TimeTrial TT in timeTrials)
                {
                    StandardModel model = context.Models.Find(TT.Model.Base);
                    TT.Model = null;    //Added this to prevent an error

                    model.TimeTrials.Add(TT);
                }
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Adds a modification request to the database
        /// </summary>
        /// <param name="mod"> the modification to add </param>
        public void addModificationRequest(Modification mod)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                context.Modifications.Add(mod);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Adds an override request to the database
        /// </summary>
        /// <param name="ov"> the override request to add </param>
        public void addOverrideRequest(OverrideRequest ov)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                context.OverrideRequests.Add(ov);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Adds a model to the database
        /// </summary>
        /// <param name="model"> the model to add </param>
        public void addModel(StandardModel model)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                context.Models.Add(model);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Adds an option to the database
        /// </summary>
        /// <param name="option"> the option to add </param>
        public void addOption(Option option)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                context.Options.Add(option);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates the model found with the primary key modelBase with the new 
        /// drive time and av time.
        /// </summary>
        /// <remarks> if either of the new times are not new they will be set to
        ///           the old time so setting them will not change their times </remarks>
        /// <param name="modelBase"> first 8 characters of model </param>
        /// <param name="newDriveTime"> new drive time for the model </param>
        /// <param name="newAVTime"> new av time for the model </param>
        public void updateModel(string modelBase, decimal newDriveTime, decimal newAVTime)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                StandardModel model = context.Models.Find(modelBase);

                model.DriveTime = newDriveTime;
                model.AVTime = newAVTime;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates the option found with the primary composite key optionCode and boxSize with
        /// the new time for the option and a new name/description for the option
        /// </summary>
        /// <remarks> if either the new time or new name is not new they will be set to
        ///           the old value so setting them will not change their value </remarks>
        /// <param name="optionCode"> option code </param>
        /// <param name="boxSize"> box size for the option </param>
        /// <param name="newTime"> new time for the option </param>
        /// <param name="newName"> new name for the option </param>
        public void updateOption(string optionCode, string boxSize, decimal newTime, string newName)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                Option option = context.Options.Find(optionCode, boxSize);

                option.Time = newTime;
                option.Name = newName;

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes the override from the database
        /// </summary>
        /// <param name="modelNum"> primary key for the override, full model with options </param>
        public void deleteOverride(string modelNum)
        {
            using(RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                Override ov = context.Overrides.Find(modelNum);

                context.Overrides.Remove(ov);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Adds the override to the database
        /// </summary>
        /// <param name="ov"> the override to add </param>
        /// <param name="modelBase"> first 8 characters of the model </param>
        public void addOverride(Override ov, string modelBase)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                //If there is already an override for the model number, delete it
                //and then add the new one.
                Override currentOverride = context.Overrides.Find(ov.ModelNum);
                if (currentOverride != null)
                {
                    context.Overrides.Remove(currentOverride);
                }

                StandardModel model = context.Models.Find(modelBase);
                model.Overrides.Add(ov);

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates the modification upon approval or denial of the request
        /// </summary>
        /// <param name="modification"> the modification to update </param>
        public void updateModification(Modification modification)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                Modification mod = context.Modifications.Find(modification.ModificationID);

                //Update modification info for approval or denial
                mod.ReviewDate = modification.ReviewDate;
                mod.Reviewer = modification.Reviewer;

                //If modification is checked to approve
                if (modification.State == 3)
                {
                    bool changed = false;

                    //Check if the manager changed anything.  Only matters for accepted requests
                    if ((!modification.ModelBase.Equals(mod.ModelBase)) ||
                        (!modification.BoxSize.Equals(mod.BoxSize)) ||
                        (modification.NewDriveTime != mod.NewDriveTime) ||
                        (modification.NewAVTime != mod.NewAVTime) ||
                        (!modification.OptionCode.Equals(mod.OptionCode)) ||
                        (modification.NewTime != mod.NewTime) ||
                        (!modification.NewName.Equals(mod.NewName)))
                    {
                        changed = true;
                    }

                    if (changed)
                    {
                        //Deny the old request.  Create a new request from the manager.  Add it.
                        mod.State = 2;   //Database Supervisor Override Request Denied

                        Modification newMod = new Modification()
                        {
                            RequestDate = DateTime.Now,
                            ReviewDate = DateTime.Now,
                            Description = "Manager updated supervisor request",
                            State = 1,
                            Sender = modification.Reviewer,
                            Reviewer = modification.Reviewer,
                            IsOption = modification.IsOption,
                            IsNew = modification.IsNew,
                            BoxSize = modification.BoxSize,
                            ModelBase = modification.ModelBase,
                            NewDriveTime = modification.NewDriveTime,
                            NewAVTime = modification.NewAVTime,
                            OldModelDriveTime = modification.OldModelDriveTime,
                            OldModelAVTime = modification.OldModelAVTime,
                            OptionCode = modification.OptionCode,
                            NewTime = modification.NewTime,
                            NewName = modification.NewName,
                            OldOptionTime = modification.OldOptionTime,
                            OldOptionName = modification.OldOptionName
                        };

                        context.Modifications.Add(newMod);
                    }
                    else
                    { 
                        //If nothing is changed accept the modification
                        mod.State = 1; 
                    }
                }
                //If modification is checked to deny
                else if (modification.State == 4) 
                {
                    mod.State = 2;  //Request denied
                }

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates the override request upon approval or denial of the request
        /// </summary>
        /// <param name="overrideRequest"> the override request to update </param>
        public void updateOverrideRequest(OverrideRequest overrideRequest)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                OverrideRequest or = context.OverrideRequests.Find(overrideRequest.OverrideRequestID);

                //Update override request info for approval or denial
                or.ReviewDate = overrideRequest.ReviewDate;
                or.Reviewer = overrideRequest.Reviewer;

                //If override request is checked to approve
                if (overrideRequest.State == 3)
                {
                    //Check if the Database Override Request has any different elements than the passed in Override Request
                    //Only if the request is getting approved.
                    bool changed = false;
                    bool modelNumChanged = false;

                    if (!overrideRequest.ModelNum.Equals(or.ModelNum))
                    {
                        changed = true;
                        modelNumChanged = true;
                    }

                    if (overrideRequest.OverrideTime != or.OverrideTime ||
                        overrideRequest.OverrideRoute != or.OverrideRoute)
                    {
                        changed = true;
                    }

                    if (changed)
                    {
                        //Deny the old request.  Create a new request from the manager.  Add it.
                        or.State = 2;   //Database Supervisor Override Request Denied

                        OverrideRequest newOR = new OverrideRequest()
                        {
                            RequestDate = DateTime.Now,
                            Description = "Manager updated supervisor request",
                            State = 1,
                            Sender = overrideRequest.Reviewer,
                            Reviewer = overrideRequest.Reviewer,
                            ReviewDate = DateTime.Now,

                            OverrideTime = overrideRequest.OverrideTime,
                            OverrideRoute = overrideRequest.OverrideRoute,
                            ModelNum = overrideRequest.ModelNum
                        };

                        if (modelNumChanged)
                        {
                            newOR.ModelBase = newOR.ModelNum.Substring(0, 8);
                            newOR.ModelTime = 0;
                            newOR.ModelRoute = 0;
                        }
                        else
                        {
                            newOR.ModelBase = overrideRequest.ModelBase;
                            newOR.ModelTime = overrideRequest.ModelTime;
                            newOR.ModelRoute = overrideRequest.ModelRoute;
                        }

                        context.OverrideRequests.Add(newOR);
                    }
                    else
                    {
                        //If nothing is changed accept the override request
                        or.State = 1;   //Approved
                    }
                }
                //If override request is checked to deny
                else if(overrideRequest.State == 4)
                {
                    or.State = 2;   //Denied
                }

                context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes a time trial from the database
        /// </summary>
        /// <param name="tt"> the time trial to delete </param>
        public void deleteTimeTrial(TimeTrial tt)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                TimeTrial timeTrial = context.TimeTrials.Find(tt.ProductionNumber);
                context.TimeTrials.Remove(timeTrial);
                context.SaveChanges();
            }
        }
        #endregion

        /// <summary>
        /// Filters to return a distinct list of every component, disregarding enclosure size
        /// </summary>
        /// <returns> returns a list containing all possible components </returns>
        public IEnumerable<EngineeredModelDTO> getModelComponents()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Components.GroupBy(x => x.ComponentName)
                                         .Select(y => y.FirstOrDefault())
                                         .Select(z => new EngineeredModelDTO
                                         {
                                             ComponentName = z.ComponentName,
                                             Quantity = 0,
                                             TotalTime = 0
                                         }).ToList();
            }
        }

        #region Components Read
        /// <param name="name"> name of the component </param>
        /// <param name="enclosureSize"> enclosure size of the component </param>
        /// <returns> returns the component found with the primary keys, otherwise null </returns>
        public Component getComponent(string name, string enclosureSize)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Components.Find(name, enclosureSize);
            }
        }

        /// <returns> returns a distinct list of all component names </returns>
        public IEnumerable<string> getComponentNames()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Components.Select(x => x.ComponentName).ToList().Distinct();
            }
        }

        /// <param name="enclosureSize"> enclosure size for the components </param>
        /// <returns> returns a list of components for a selected enclosure size </returns>
        public IEnumerable<Component> getEnclosureSizeComponents(string enclosureSize)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Components.Where(x => x.EnclosureSize.Equals(enclosureSize)).ToList();
            }
        }

        /// <param name="name"> component name</param>
        /// <param name="enclosureSize"> enclosure size for component </param>
        /// <returns> returns a list of components that contain the filters </returns>
        public IEnumerable<Component> getFilteredComponents(string name, string enclosureSize)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Components.Where(x => x.ComponentName.Contains(name) &&
                                                     x.EnclosureSize.Contains(enclosureSize)).ToList();
            }
        }
        #endregion

        #region Enclosure Read
        /// <param name="enclosureType"> enclosure type ex. T1, T12, T3R </param>
        /// <param name="enclosureSize"> enclosure size </param>
        /// <returns> returns an enclosure found by the primary keys, otherwise null </returns>
        public Enclosure getEnclosure(string enclosureType, string enclosureSize)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Enclosures.Find(enclosureType, enclosureSize);
            }
        }

        /// <returns> returns a list of distinct enclosure types </returns>
        public IEnumerable<string> getEnclosureTypes()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Enclosures.Select(x => x.EnclosureType).ToList().Distinct();
            }
        }

        /// <returns> returns a list of distinct enclosure sizes </returns>
        public IEnumerable<string> getEnclosureSizes()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Enclosures.Select(x => x.EnclosureSize).ToList().Distinct();
            }
        }

        /// <param name="enclosureType"> enclosure type </param>
        /// <param name="enclosureSize"> enclosure size </param>
        /// <returns> returns a list of enclosures that contain the filters </returns>
        public IEnumerable<Enclosure> getFilteredEnclosures(string enclosureType, string enclosureSize)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Enclosures.Where(x => x.EnclosureType.Contains(enclosureType) &&
                                                     x.EnclosureSize.Contains(enclosureSize)).ToList();
            }
        }

        /// <remarks> if either filter is null, method only filters for the other filter </remarks>
        /// <param name="enclosureType"> enclosure type </param>
        /// <param name="enclosureSize"> enclosure size </param>
        /// <returns> returns a list of enclosures that match the filters </returns>
        public IEnumerable<Enclosure> getExactEnclosures(string enclosureType, string enclosureSize)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                if (string.IsNullOrWhiteSpace(enclosureType))
                {
                    return context.Enclosures.Where(x => x.EnclosureSize.Equals(enclosureSize)).ToList();
                }
                else if (string.IsNullOrWhiteSpace(enclosureSize))
                {
                    return context.Enclosures.Where(x => x.EnclosureType.Equals(enclosureType)).ToList();
                }
                else
                {
                    return context.Enclosures.Where(x => x.EnclosureType.Equals(enclosureType) &&
                                                         x.EnclosureSize.Equals(enclosureSize)).ToList();
                }
            }
        }
        #endregion

        #region Wire Gauge Read
        /// <param name="gauge"> gauge of the wire </param>
        /// <returns> returns the wire gauge found by the primary key, otherwise null </returns>
        public WireGauge getWireGauge(string gauge)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.WireGauges.Find(gauge);
            }
        }

        /// <returns> returns a list of all wire gauges </returns>
        public IEnumerable<WireGauge> getWireGauges()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.WireGauges.ToList();
            }
        }

        /// <param name="gauge"> gauge of the wire </param>
        /// <returns> returns a list of wire gauges that contain the filter </returns>
        public IEnumerable<WireGauge> getFilteredWireGauges(string gauge)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.WireGauges.Where(x => x.Gauge.Contains(gauge)).ToList();
            }
        }
        #endregion

        #region Engineered Modifications Read
        /// <remarks> Includes all states in the returned list but filters the other parameters </remarks>
        /// <param name="ComponentName"> component name </param>
        /// <param name="EnclosureSize"> enclosure size </param>
        /// <param name="EnclosureType"> enclosure type </param>
        /// <param name="Gauge"> gauge of the wire </param>
        /// <param name="Sender"> user who sent the modification </param>
        /// <param name="Reviewer"> user who approved/denied the modification </param>
        /// <returns> returns a list of engineered modifications that contain the filters </returns>
        public IEnumerable<EngineeredModification> getFilteredEngineeredModifications(string ComponentName, string EnclosureSize, string EnclosureType, string Gauge, string Sender, string Reviewer)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.EngineeredModifications.Where(x => x.ComponentName.Contains(ComponentName) &&
                                                                  x.EnclosureSize.Contains(EnclosureSize) &&
                                                                  x.EnclosureType.Contains(EnclosureType) &&
                                                                  x.Gauge.Contains(Gauge) &&
                                                                  x.Sender.Contains(Sender) &&
                                                                  x.Reviewer.Contains(Reviewer)).ToList();
            }
        }

        /// <remarks> Includes only the waiting states (state = 0 (waiting), 3 (currently checked to approve), or 4 (currently checked to decline))
        ///           in the returned list and filters the other parameters </remarks>
        /// <param name="ComponentName"> component name </param>
        /// <param name="EnclosureSize"> enclosure size </param>
        /// <param name="EnclosureType"> enclosure type </param>
        /// <param name="Gauge"> gauge of the wire </param>
        /// <param name="Sender"> user who sent the modification </param>
        /// <param name="Reviewer"> user who approved/denied the modification </param>
        /// <returns> returns a list of engineered modifications that contain the filters </returns>
        public IEnumerable<EngineeredModification> getFilteredWaitingEngineeredModifications(string ComponentName, string EnclosureSize, string EnclosureType, string Gauge, string Sender, string Reviewer)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.EngineeredModifications.Where(x => (x.State == 0 || x.State == 3 || x.State == 4) &&
                                                                   x.ComponentName.Contains(ComponentName) &&
                                                                   x.EnclosureSize.Contains(EnclosureSize) &&
                                                                   x.EnclosureType.Contains(EnclosureType) &&
                                                                   x.Gauge.Contains(Gauge) &&
                                                                   x.Sender.Contains(Sender) &&
                                                                   x.Reviewer.Contains(Reviewer)).ToList();
            }
        }

        /// <remarks> Used to filter for approved state or declined state modifications while still 
        ///           filtering for the other parameters </remarks>
        /// <param name="State"></param>
        /// <param name="ComponentName"> component name </param>
        /// <param name="EnclosureSize"> enclosure size </param>
        /// <param name="EnclosureType"> enclosure type </param>
        /// <param name="Gauge"> gauge of the wire </param>
        /// <param name="Sender"> user who sent the modification </param>
        /// <param name="Reviewer"> user who approved/denied the modification </param>
        /// <returns> returns a list of engineered modifications that contain the filters </returns>
        public IEnumerable<EngineeredModification> getFilteredStateEngineeredModifications(int State, string ComponentName, string EnclosureSize, string EnclosureType, string Gauge, string Sender, string Reviewer)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.EngineeredModifications.Where(x => x.State == State &&
                                                                  x.ComponentName.Contains(ComponentName) &&
                                                                  x.EnclosureSize.Contains(EnclosureSize) &&
                                                                  x.EnclosureType.Contains(EnclosureType) &&
                                                                  x.Gauge.Contains(Gauge) &&
                                                                  x.Sender.Contains(Sender) &&
                                                                  x.Reviewer.Contains(Reviewer)).ToList();
            }
        }

        /// <remarks> Matching EnclosureType and Gauge to "", with isNew == true ensures it is a new component request </remarks>
        /// <param name="Sender"> user who sent the modification </param>
        /// <param name="ComponentName"> component name </param>
        /// <param name="EnclosureSize"> enclosure size </param>
        /// <returns> returns a list of unapproved new component requests that contain the filters</returns>
        public IEnumerable<EngineeredModification> getFilteredNewComponents(string Sender, string ComponentName, string EnclosureSize)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.EngineeredModifications.Where(x => x.IsNew == true &&
                                                                  x.State == 0 &&
                                                                  x.Sender.Contains(Sender) &&
                                                                  x.ComponentName.Contains(ComponentName) &&
                                                                  x.EnclosureSize.Contains(EnclosureSize) &&
                                                                  x.EnclosureType.Equals("") &&
                                                                  x.Gauge.Equals("")).ToList();
            }
        }

        /// <remarks> Matching EnclosureType and Gauge to "", with isNew == false, ensures it is a modified component request </remarks>
        /// <param name="Sender"> user who sent the modification </param>
        /// <param name="ComponentName"> component name </param>
        /// <param name="EnclosureSize"> enclosure size </param>
        /// <returns> returns a list of unapproved modified component requests that contain the filters</returns>
        public IEnumerable<EngineeredModification> getFilteredModifiedComponents(string Sender, string ComponentName, string EnclosureSize)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.EngineeredModifications.Where(x => x.IsNew == false &&
                                                                  x.State == 0 &&
                                                                  x.Sender.Contains(Sender) &&
                                                                  x.ComponentName.Contains(ComponentName) &&
                                                                  x.EnclosureSize.Contains(EnclosureSize) &&
                                                                  x.EnclosureType.Equals("") &&
                                                                  x.Gauge.Equals("")).ToList();
            }
        }

        /// <remarks> Matching ComponentName and Gauge to "", with isNew == false, ensures it is a modified enclosure request </remarks>
        /// <param name="Sender"> user who sent the modification </param>
        /// <param name="EnclosureSize"> enclosure size </param>
        /// <param name="EnclosureType"> enclosure type </param>
        /// <returns> returns a list of unapproved modified component requests that contain the filters</returns>
        public IEnumerable<EngineeredModification> getFilteredModifiedEnclosures(string Sender, string EnclosureSize, string EnclosureType)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.EngineeredModifications.Where(x => x.IsNew == false &&
                                                                  x.State == 0 &&
                                                                  x.Sender.Contains(Sender) &&
                                                                  x.EnclosureSize.Contains(EnclosureSize) &&
                                                                  x.EnclosureType.Contains(EnclosureType) &&
                                                                  x.ComponentName.Equals("") &&
                                                                  x.Gauge.Equals("")).ToList();
            }
        }

        /// <remarks> Matching ComponentName, EnclosureSize, and EnclosureType to "", ensures it is a wire gauge request </remarks>
        /// <param name="Sender"> user who sent the modification </param>
        /// <param name="Gauge"> gauge of the wire </param>
        /// <param name="IsNew"> true if the request is for a new item, false otherwise </param>
        /// <returns> returns a list of unapproved wire gauge requests that contain the filters</returns>
        public IEnumerable<EngineeredModification> getFilteredWireGaugeMods(string Sender, string Gauge, bool IsNew)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                if (IsNew)
                {
                    //If IsNew filter is true, then only return new wire gauge requests
                    return context.EngineeredModifications.Where(x => x.IsNew == IsNew &&
                                                                      x.State == 0 &&
                                                                      x.Sender.Contains(Sender) &&
                                                                      x.Gauge.Contains(Gauge) &&
                                                                      x.EnclosureSize.Equals("") &&
                                                                      x.EnclosureType.Equals("") &&
                                                                      x.ComponentName.Equals("")).ToList();
                }
                else
                {
                    //If IsNew filter is false, then return all wire gauge requests
                    return context.EngineeredModifications.Where(x => x.State == 0 &&
                                                                      x.Sender.Contains(Sender) &&
                                                                      x.Gauge.Contains(Gauge) &&
                                                                      x.EnclosureSize.Equals("") &&
                                                                      x.EnclosureType.Equals("") &&
                                                                      x.ComponentName.Equals("")).ToList();
                }
            }
        }

        /// <remarks> Ensures that the wire gauge size matches exactly </remarks>
        /// <remarks> Matching ComponentName, EnclosureSize, and EnclosureType to "", ensures it is a wire gauge request </remarks>
        /// <param name="Gauge"> gauge of the wire </param>
        /// <returns> returns a list of unapproved wire gauge requests that meet the filters</returns>
        public IEnumerable<EngineeredModification> getNewWireGaugeMods(string Gauge)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.EngineeredModifications.Where(x => x.IsNew == true &&
                                                                  x.State == 0 &&
                                                                  x.Gauge.Equals(Gauge) &&
                                                                  x.EnclosureSize.Equals("") &&
                                                                  x.EnclosureType.Equals("") &&
                                                                  x.ComponentName.Equals("")).ToList();
            }
        }
        #endregion

        #region Modify Engineered
        /// <summary>
        /// Adds a engineered modification to the database
        /// </summary>
        /// <param name="mod"> the modification to add </param>
        public void addEngineeredModificationRequest(EngineeredModification mod)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                context.EngineeredModifications.Add(mod);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Adds a component to the database
        /// </summary>
        /// <param name="component"> the component to add </param>
        public void addComponent(Component component)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                context.Components.Add(component);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Adds a wire gauge to the database
        /// </summary>
        /// <param name="wireGauge"> the wire gauge to add </param>
        public void addWireGauge(WireGauge wireGauge)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                context.WireGauges.Add(wireGauge);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates the component with a new time
        /// </summary>
        /// <param name="name"> component name </param>
        /// <param name="enclosureSize"> enclosure size of the component </param>
        /// <param name="newTime"> new time to associate with component </param>
        public void updateComponent(string name, string enclosureSize, decimal newTime)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                Component component = context.Components.Find(name, enclosureSize);
                component.Time = newTime;
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates the enclosure with a new time
        /// </summary>
        /// <param name="enclosureType"> enclosuer type </param>
        /// <param name="enclosureSize"> enclosure size </param>
        /// <param name="newTime"> new time to associate with enclosure </param>
        public void updateEnclosure(string enclosureType, string enclosureSize, decimal newTime)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                Enclosure enclosure = context.Enclosures.Find(enclosureType, enclosureSize);
                enclosure.Time = newTime;
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates the wire gauge with a new time percentage
        /// </summary>
        /// <param name="gauge"> gauge of the wire </param>
        /// <param name="newTimePercentage"> new time percentage to associate with wire gauge </param>
        public void updateWireGauge(string gauge, decimal newTimePercentage)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                WireGauge wireGauge = context.WireGauges.Find(gauge);
                wireGauge.TimePercentage = newTimePercentage;
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates the engineered modification upon approval or denial
        /// </summary>
        /// <param name="modification"> the engineered modification to update </param>
        public void updateEngineeredModification(EngineeredModification modification)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                EngineeredModification mod = context.EngineeredModifications.Find(modification.ModificationID);

                //Update modification info for approval or denial
                mod.ReviewedDate = modification.ReviewedDate;
                mod.Reviewer = modification.Reviewer;

                //If modification is checked to approve
                if (modification.State == 3)
                {
                    bool changed = false;

                    //Check if the manager changed anything.  Only matters for accepted requests
                    if ((!modification.ComponentName.Equals(mod.ComponentName)) ||
                        (!modification.EnclosureSize.Equals(mod.EnclosureSize)) ||
                        (!modification.EnclosureType.Equals(mod.EnclosureType)) ||
                        (!modification.Gauge.Equals(mod.Gauge)) ||
                        (modification.NewTime != mod.NewTime) ||
                        (modification.NewTimePercentage != mod.NewTimePercentage))
                    {
                        changed = true;
                    }

                    if (changed)
                    {
                        //Deny the old request.  Create a new request for the manager.  Add it.
                        mod.State = 2;   //Database Supervisor Modification Request Denied

                        EngineeredModification newMod = new EngineeredModification()
                        {
                            RequestDate = DateTime.Now,
                            ReviewedDate = DateTime.Now,
                            Description = "Manager updated supervisor request",
                            State = 1,
                            Sender = modification.Reviewer,
                            Reviewer = modification.Reviewer,
                            IsNew = modification.IsNew,
                            ComponentName = modification.ComponentName,
                            EnclosureSize = modification.EnclosureSize,
                            EnclosureType = modification.EnclosureType,
                            NewTime = modification.NewTime,
                            OldTime = modification.OldTime,
                            Gauge = modification.Gauge,
                            NewTimePercentage = modification.NewTimePercentage,
                            OldTimePercentage = modification.OldTimePercentage
                        };

                        context.EngineeredModifications.Add(newMod);
                    }
                    else
                    {
                        //If nothing is changed accept the modification
                        mod.State = 1;
                    }
                }
                //If modification is checked to deny
                else if (modification.State == 4) 
                {
                    mod.State = 2; //Request denied
                }

                context.SaveChanges();
            }
        }
        #endregion

        #region RouteQueue
        /// <remarks> Only returns the queued routes that have not been approved yet </remarks>
        /// <param name="modelNumber"> full model number (standard or engineered) with options </param>
        /// <returns> returns a list of queued routes that contain the filter </returns>
        public IEnumerable<RouteQueue> getFilteredRouteQueues(string modelNumber)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.RouteQueues.Where(x => x.IsApproved == false &&
                                                      x.ModelNumber.Contains(modelNumber)).ToList();
            }
        }

        /// <summary>
        /// Adds a route to the queue
        /// </summary>
        /// <param name="route"> route to be added </param>
        public void addRouteQueue(RouteQueue route)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                context.RouteQueues.Add(route);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes a route from the queue
        /// </summary>
        /// <param name="selectedRoute"> route to be deleted </param>
        public void deleteQueuedRoute(RouteQueue selectedRoute)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                RouteQueue route = context.RouteQueues.Find(selectedRoute.RouteQueueID);

                context.RouteQueues.Remove(route);
                context.SaveChanges();
            }
        }
        #endregion
    }
}