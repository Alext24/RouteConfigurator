using RouteConfigurator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.Entity;

namespace RouteConfigurator.Design
{
    public class DataAccessService : IDataAccessService
    {
        #region Constructor
        public DataAccessService() { }
        #endregion

        #region Model Read
        /// <param name="modelName"> base name for a model </param>
        /// <returns> returns the model specified by the model name </returns>
        public Model.Model getModel(string modelName)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Models.Find(modelName) ;
            }
        }

        /// <returns> returns all models </returns>
        public IEnumerable<Model.Model> getModels()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Models.ToList();
            }
        }

        /// <param name="modelFilter"> base name for the model </param>
        /// <param name="boxSizeFilter"> box size for the model </param>
        /// <returns> returns a list of the models that meet the filters</returns>
        public IEnumerable<Model.Model> getFilteredModels(string modelFilter, string boxSizeFilter)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Models.Where(model => model.Base.Contains(modelFilter) &&
                                                     model.BoxSize.Contains(boxSizeFilter)).ToList();
            }
        }

        /// <returns> list of unique drive types</returns>
        public IEnumerable<string> getDriveTypes()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Models.Select(x => x.Base.Substring(0, 4)).ToList().Distinct();
            }
        }

        /// <param name="drive"></param>
        /// <param name="av"></param>
        /// <param name="boxSize"></param>
        /// <param name="exact"></param>
        /// <returns> a list of models that meet the filters</returns>
        public IEnumerable<Model.Model> getNumModelsFound(string drive, string av, string boxSize, bool exact)
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

        /// <param name="optionFilter"> name for the option </param>
        /// <param name="optionBoxSizeFilter"> box size for the option </param>
        /// <param name="exact"> if box size needs to be exact</param>
        /// <returns> returns a list of the options that meet the filters</returns>
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

        /// <param name="optionsList"> option codes to search for </param>
        /// <param name="boxSize"> box size of model </param>
        /// <returns> list of options </returns>
        public IEnumerable<Option> getModelOptions(List<string> optionsList, string boxSize)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Options.Where(option => optionsList.Any(optionCode => option.OptionCode.Contains(optionCode)) &&
                                                       option.BoxSize.Equals(boxSize)).ToList();
            }
        }

        /// <param name="boxSize"> box size for the model </param>
        /// <param name="options"> list of options</param>
        /// <returns> total time for the options </returns>
        public decimal getTotalOptionsTime(string boxSize, List<string> options)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Options.Where(x => x.BoxSize.Equals(boxSize) &&
                                                  options.Contains(x.OptionCode)).Sum(y => y.Time);
            }
        }

        public IEnumerable<string> getOptionCodes()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Options.Select(x => x.OptionCode).ToList().Distinct();
            }
        }

        public IEnumerable<Option> getNumOptionsFound(string optionCode, string boxSize, bool exact)
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
        /// <param name="modelNum"> the entire model number </param>
        /// <returns> returns the override info for a model</returns>
        public Override getModelOverride(string modelNum)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Overrides.Find(modelNum) as Override;
            }
        }

        /// <returns> list of active overrides</returns>
        public IEnumerable<Override> getOverrides()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Overrides.Include(x => x.Model).ToList();
            }
        }

        /// <param name="overrideFilter"> model number to filter overrides by</param>
        /// <returns> list of active overrides that contain the model number</returns>
        public IEnumerable<Override> getFilteredOverrides(string overrideFilter)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Overrides.Include(x => x.Model).Where(item => item.ModelNum.Contains(overrideFilter)).ToList();
            }
        }
        #endregion

        #region Time Trial Read
        public TimeTrial getTimeTrial(int productionNumber)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.TimeTrials.Find(productionNumber);
            }
        }

        /// <param name="modelBase"> model base to get time trials for</param>
        /// <returns> list of time trials with the model base </returns>
        public IEnumerable<TimeTrial> getTimeTrials(string modelBase)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.TimeTrials.Include(a => a.TTOptionTimes).Where(x => x.Model.Base.Equals(modelBase)).ToList();
            }
        }

        /// <param name="modelBase">model base name</param>
        /// <param name="options">list of options</param>
        /// <returns> list of time trials for a specific model </returns>
        public IEnumerable<TimeTrial> getTimeTrials(string modelBase, List<string> options)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.TimeTrials.Include(a => a.Model).Include(a => a.TTOptionTimes).Where(x => 
                               x.Model.Base.Equals(modelBase) &&
                               x.TTOptionTimes.Count == options.Count &&
                               x.TTOptionTimes.All(y => options.Contains(y.OptionCode))).ToList();
            }
        }

        /// <param name="modelBase"> base model name </param>
        /// <param name="optionTextFilter"> option text for the model number </param>
        /// <param name="salesFilter"> sales number for the time trials</param>
        /// <param name="productionNumFilter"> production number for the time trials</param>
        /// <param name="exact"> if Options text needs to be exact</param>
        /// <returns> a list of time trials that meet the filters</returns>
        public IEnumerable<TimeTrial> getFilteredTimeTrials(string modelBase, string optionTextFilter, string salesFilter, string productionNumFilter, bool exact)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                if (exact)
                {
                    return context.TimeTrials.Include(a => a.Model).Include(a => a.TTOptionTimes).Where(tt => 
                                   tt.Model.Base.Contains(modelBase) &&
                                   tt.OptionsText.Equals(optionTextFilter) &&
                                   tt.SalesOrder.ToString().Contains(salesFilter) &&
                                   tt.ProductionNumber.ToString().Contains(productionNumFilter)).ToList();
                }
                else
                {
                    return context.TimeTrials.Include(a => a.Model).Include(a => a.TTOptionTimes).Where(tt => 
                                   tt.Model.Base.Contains(modelBase) &&
                                   tt.OptionsText.Contains(optionTextFilter) &&
                                   tt.SalesOrder.ToString().Contains(salesFilter) &&
                                   tt.ProductionNumber.ToString().Contains(productionNumFilter)).ToList();
                }
            }
        }

        #endregion

        #region Modifications Read
        public IEnumerable<Modification> getModifications()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Modifications.ToList();
            }
        }

        // Include all states but filter other parameters
        public IEnumerable<Modification> getFilteredModifications(string ModelBase, string BoxSize, string OptionCode, string Sender, string Reviewer)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Modifications.Where(item => item.ModelBase.Contains(ModelBase) &&
                                                           item.BoxSize.Contains(BoxSize) &&
                                                           item.OptionCode.Contains(OptionCode) &&
                                                           item.Sender.Contains(Sender) &&
                                                           item.Reviewer.Contains(Reviewer)).ToList();
            }
        }

        // If filtered for waiting state, also include states 3 and 4 (Currently checked to approve or decline)
        public IEnumerable<Modification> getFilteredWaitingModifications(string ModelBase, string BoxSize, string OptionCode, string Sender, string Reviewer)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Modifications.Where(item => (item.State == 0 || item.State == 3 || item.State == 4) &&
                                                            item.ModelBase.Contains(ModelBase) &&
                                                            item.BoxSize.Contains(BoxSize) &&
                                                            item.OptionCode.Contains(OptionCode) &&
                                                            item.Sender.Contains(Sender) &&
                                                            item.Reviewer.Contains(Reviewer)).ToList();
            }
        }

        public IEnumerable<Modification> getFilteredStateModifications(int State, string ModelBase, string BoxSize, string OptionCode, string Sender, string Reviewer)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Modifications.Where(item => item.State == State &&
                                                           item.ModelBase.Contains(ModelBase) &&
                                                           item.BoxSize.Contains(BoxSize) &&
                                                           item.OptionCode.Contains(OptionCode) &&
                                                           item.Sender.Contains(Sender) &&
                                                           item.Reviewer.Contains(Reviewer)).ToList();
            }
        }

        /// <param name="Sender"> User who sent the modification </param>
        /// <param name="Base"> Model base (drive and av) </param>
        /// <param name="BoxSize"> Box size for the model</param>
        /// <returns> List of unapproved new models that meet the filters</returns>
        public IEnumerable<Modification> getFilteredNewModels(string Sender, string Base, string BoxSize)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Modifications.Where(item => item.IsOption == false && item.IsNew == true &&
                                                           item.State == 0 &&
                                                           item.Sender.Contains(Sender) &&
                                                           item.ModelBase.Contains(Base) &&
                                                           item.BoxSize.Contains(BoxSize)).ToList();
            }
        }

        /// <param name="Sender"> User who sent the modification </param>
        /// <param name="OptionCode"> Option code </param>
        /// <param name="BoxSize"> Box size for the option</param>
        /// <returns> List of unapproved new options that meet the filters</returns>
        public IEnumerable<Modification> getFilteredNewOptions(string Sender, string OptionCode, string BoxSize)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Modifications.Where(item => item.IsOption == true && item.IsNew == true &&
                                                           item.State == 0 &&
                                                           item.Sender.Contains(Sender) &&
                                                           item.OptionCode.Contains(OptionCode) &&
                                                           item.BoxSize.Contains(BoxSize)).ToList();
            }
        }

        /// <param name="Sender"> User who sent the modification </param>
        /// <param name="ModelName"> Model name to filter</param>
        /// <returns> List of unapproved modified models that meet the filters</returns>
        public IEnumerable<Modification> getFilteredModifiedModels(string Sender, string ModelName)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Modifications.Where(item => item.IsOption == false && item.IsNew == false &&
                                                           item.State == 0 &&
                                                           item.Sender.Contains(Sender) &&
                                                           item.ModelBase.Contains(ModelName)).ToList();
            }
        }

        /// <param name="Sender"> User who sent the modification </param>
        /// <param name="OptionCode"> Option code</param>
        /// <param name="BoxSize"> Box size for the option</param>
        /// <returns> List of unapproved modified options that meet the filters</returns>
        public IEnumerable<Modification> getFilteredModifiedOptions(string Sender, string OptionCode, string BoxSize)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Modifications.Where(item => item.IsOption == true && item.IsNew == false &&
                                                           item.State == 0 &&
                                                           item.Sender.Contains(Sender) &&
                                                           item.OptionCode.Contains(OptionCode) &&
                                                           item.BoxSize.Contains(BoxSize)).ToList();
            }
        }

        /// <param name="mod"></param>
        /// <returns> true if modification is a duplicate, false otherwise </returns>
        public bool checkDuplicateOverrideDeletion(Modification mod)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return (context.Modifications.Where(item => item.Description.Equals(mod.Description) &&
                                                           (item.State == 0 || item.State == 3 || item.State == 4)).FirstOrDefault() == null ? false : true);
            }
        }
        #endregion

        #region Override Requests Read
        public IEnumerable<OverrideRequest> getOverrideRequests()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.OverrideRequests.ToList();
            }
        }

        /// <param name="Sender"> User who sent the request </param>
        /// <param name="ModelNum"> Full model number with options </param>
        /// <returns> List of override requests that meet the filters</returns>
        public IEnumerable<OverrideRequest> getFilteredOverrideRequests(string Sender, string ModelNum)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.OverrideRequests.Where(item => item.State == 0 &&
                                                              item.Sender.Contains(Sender) &&
                                                              item.ModelNum.Contains(ModelNum)).ToList();
            }
        }

        public IEnumerable<OverrideRequest> getFilteredOverrideRequests(int State, string ModelNum, string Sender, string Reviewer)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                // Include all states but filter other parameters
                if(State == -1)
                {
                    return context.OverrideRequests.Where(item => item.ModelNum.Contains(ModelNum) &&
                                                                  item.Sender.Contains(Sender) &&
                                                                  item.Reviewer.Contains(Reviewer)).ToList();
                }
                // If filtered for waiting state, also include states 3 and 4 (Currently checked to approve or decline)
                else if (State == 0)
                {
                    return context.OverrideRequests.Where(item => (item.State == 0 || item.State == 3 || item.State == 4) &&
                                                                   item.ModelNum.Contains(ModelNum) &&
                                                                   item.Sender.Contains(Sender) &&
                                                                   item.Reviewer.Contains(Reviewer)).ToList();
                }
                else
                {
                    return context.OverrideRequests.Where(item => item.State == State &&
                                                                  item.ModelNum.Contains(ModelNum) &&
                                                                  item.Sender.Contains(Sender) &&
                                                                  item.Reviewer.Contains(Reviewer)).ToList();
                }
            }
        }

            
        #endregion 

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
                    Model.Model model = context.Models.Find(TT.Model.Base);
                    TT.Model = null;    //Added this to prevent an error

                    model.TimeTrials.Add(TT);
                }
                context.SaveChanges();
            }
        }

        public void addModificationRequest(Modification mod)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                context.Modifications.Add(mod);
                context.SaveChanges();
            }
        }

        public void addOverrideRequest(OverrideRequest ov)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                context.OverrideRequests.Add(ov);
                context.SaveChanges();
            }
        }

        public void addModel(Model.Model model)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                context.Models.Add(model);
                context.SaveChanges();
            }
        }

        public void addOption(Option option)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                context.Options.Add(option);
                context.SaveChanges();
            }
        }

        public void updateModel(string modelBase, decimal newDriveTime, decimal newAVTime)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                Model.Model model = context.Models.Find(modelBase);

                model.DriveTime = newDriveTime;
                model.AVTime = newAVTime;

                context.SaveChanges();
            }
        }

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

        public void deleteOverride(string modelNum)
        {
            using(RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                Override ov = context.Overrides.Find(modelNum);

                context.Overrides.Remove(ov);
                context.SaveChanges();
            }
        }

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

                Model.Model model = context.Models.Find(modelBase);
                model.Overrides.Add(ov);

                context.SaveChanges();
            }
        }

        public void updateModification(Modification modification)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                Modification mod = context.Modifications.Find(modification.ModificationID);

                //Update modification info for approval or denial
                mod.ReviewDate = modification.ReviewDate;
                mod.Reviewer = modification.Reviewer;

                if (modification.State == 3)
                {
                    bool changed = false;

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
                        //Deny the old request.  Create a new request for the manager.  Add it.
                        mod.State = 2;   //Database Override Request Denied

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
                        mod.State = 1;
                    }
                }
                else if (modification.State == 4) 
                {
                    mod.State = 2;
                }

                context.SaveChanges();
            }
        }

        public void updateOverrideRequest(OverrideRequest overrideRequest)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                OverrideRequest or = context.OverrideRequests.Find(overrideRequest.OverrideRequestID);

                //Update modification info for approval or denial
                or.ReviewDate = overrideRequest.ReviewDate;
                or.Reviewer = overrideRequest.Reviewer;

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
                        //Deny the old request.  Create a new request for the manager.  Add it.
                        or.State = 2;   //Database Override Request Denied

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
                            if(context.Models.Find(newOR.ModelNum.Substring(0, 8)) != null)
                            {
                                newOR.ModelBase = newOR.ModelNum.Substring(0, 8);
                                newOR.ModelTime = 0;
                                newOR.ModelRoute = 0;
                            }
                            else
                            {
                                throw new Exception("Invalid Model");
                            }
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
                        or.State = 1;   //Approved
                    }
                }
                else if(overrideRequest.State == 4)
                {
                    or.State = 2;   //Denied
                }

                context.SaveChanges();
            }
        }

        public void deleteTimeTrial(TimeTrial tt)
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                TimeTrial timeTrial = context.TimeTrials.Find(tt.ProductionNumber);
                context.TimeTrials.Remove(timeTrial);
                context.SaveChanges();
            }
        }


        #region Engineered Orders
        public IEnumerable<string> getEnclosureTypes()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Enclosures.Select(x => x.EnclosureType).ToList().Distinct();
            }
        }

        public IEnumerable<string> getEnclosureSizes()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.Enclosures.Select(x => x.EnclosureSize).ToList().Distinct();
            }
        }

        public IEnumerable<WireGauge> getWireGauges()
        {
            using (RouteConfiguratorDB context = new RouteConfiguratorDB())
            {
                return context.WireGauges.ToList();
            }
        }
        #endregion
    }
}