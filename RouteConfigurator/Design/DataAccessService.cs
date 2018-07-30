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
        private RouteConfiguratorDB context;

        #region Constructor
        public DataAccessService() { }
        #endregion

        #region Model Read
        /// <param name="modelName"> base name for a model </param>
        /// <returns> returns the model specified by the model name </returns>
        public Model.Model getModel(string modelName)
        {
            using (context = new RouteConfiguratorDB())
            {
                return context.Models.Find(modelName) as Model.Model;
            }
        }

        /// <returns> returns all models </returns>
        public IEnumerable<Model.Model> getModels()
        {
            using (context = new RouteConfiguratorDB())
            {
                return context.Models.ToList();
            }
        }

        /// <param name="modelFilter"> base name for the model </param>
        /// <param name="boxSizeFilter"> box size for the model </param>
        /// <returns> returns a list of the models that meet the filters</returns>
        public IEnumerable<Model.Model> getFilteredModels(string modelFilter, string boxSizeFilter)
        {
            using (context = new RouteConfiguratorDB())
            {
                return context.Models.Where(model => model.Base.Contains(modelFilter) &&
                                                     model.BoxSize.Contains(boxSizeFilter)).ToList();
            }
        }

        /// <returns> list of unique drive types</returns>
        public IEnumerable<string> getDriveTypes()
        {
            using (context = new RouteConfiguratorDB())
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

            using (context = new RouteConfiguratorDB())
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
            using (context = new RouteConfiguratorDB())
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
            using (context = new RouteConfiguratorDB())
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
            using (context = new RouteConfiguratorDB())
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
            using (context = new RouteConfiguratorDB())
            {
                return context.Options.Where(x => x.BoxSize.Equals(boxSize) &&
                                                  options.Contains(x.OptionCode)).Sum(y => y.Time);
            }
        }

        public IEnumerable<string> getOptionCodes()
        {
            using (context = new RouteConfiguratorDB())
            {
                return context.Options.Select(x => x.OptionCode).ToList().Distinct();
            }
        }

        public IEnumerable<Option> getNumOptionsFound(string optionCode, string boxSize, bool exact)
        {
            using (context = new RouteConfiguratorDB())
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
            using (context = new RouteConfiguratorDB())
            {
                return context.Overrides.Find(modelNum) as Override;
            }
        }

        /// <returns> list of active overrides</returns>
        public IEnumerable<Override> getOverrides()
        {
            using (context = new RouteConfiguratorDB())
            {
                return context.Overrides.ToList();
            }
        }

        /// <param name="overrideFilter"> model number to filter overrides by</param>
        /// <returns> list of active overrides that contain the model number</returns>
        public IEnumerable<Override> getFilteredOverrides(string overrideFilter)
        {
            using (context = new RouteConfiguratorDB())
            {
                return context.Overrides.Where(item => item.ModelNum.Contains(overrideFilter)).ToList();
            }
        }
        #endregion

        #region Time Trial Read
        public TimeTrial getTimeTrial(int productionNumber)
        {
            using (context = new RouteConfiguratorDB())
            {
                return context.TimeTrials.Find(productionNumber) as TimeTrial;
            }
        }

        /// <param name="modelBase"> model base to get time trials for</param>
        /// <returns> list of time trials with the model base </returns>
        public IEnumerable<TimeTrial> getTimeTrials(string modelBase)
        {
            using (context = new RouteConfiguratorDB())
            {
                return context.TimeTrials.Include(a => a.TTOptionTimes).Where(x => x.Model.Base.Equals(modelBase)).ToList();
            }
        }

        /// <param name="modelBase">model base name</param>
        /// <param name="options">list of options</param>
        /// <returns> list of time trials for a specific model </returns>
        public IEnumerable<TimeTrial> getTimeTrials(string modelBase, List<string> options)
        {
            using (context = new RouteConfiguratorDB())
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
            using (context = new RouteConfiguratorDB())
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
            using (context = new RouteConfiguratorDB())
            {
                return context.Modifications.ToList();
            }
        }

        /// <param name="Sender"> User who sent the modification </param>
        /// <param name="Base"> Model base (drive and av) </param>
        /// <param name="BoxSize"> Box size for the model</param>
        /// <returns> List of unapproved new models that meet the filters</returns>
        public IEnumerable<Modification> getFilteredNewModels(string Sender, string Base, string BoxSize)
        {
            using (context = new RouteConfiguratorDB())
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
            using (context = new RouteConfiguratorDB())
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
            using (context = new RouteConfiguratorDB())
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
            using (context = new RouteConfiguratorDB())
            {
                return context.Modifications.Where(item => item.IsOption == true && item.IsNew == false &&
                                                           item.State == 0 &&
                                                           item.Sender.Contains(Sender) &&
                                                           item.OptionCode.Contains(OptionCode) &&
                                                           item.BoxSize.Contains(BoxSize)).ToList();
            }
        }
        #endregion

        #region Override Requests Read
        public IEnumerable<OverrideRequest> getOverrideRequests()
        {
            using (context = new RouteConfiguratorDB())
            {
                return context.OverrideRequests.ToList();
            }
        }

        /// <param name="Sender"> User who sent the request </param>
        /// <param name="ModelNum"> Full model number with options </param>
        /// <returns> List of override requests that meet the filters</returns>
        public IEnumerable<OverrideRequest> getFilteredOverrideRequests(string Sender, string ModelNum)
        {
            using (context = new RouteConfiguratorDB())
            {
                return context.OverrideRequests.Where(item => item.State == 0 &&
                                                              item.Sender.Contains(Sender) &&
                                                              item.ModelNum.Contains(ModelNum)).ToList();
            }
        }
        #endregion 

        /// <summary>
        /// Adds a list of time trials to the database
        /// </summary>
        /// <param name="timeTrials"> list of new time trials </param>
        public void addTimeTrials(ObservableCollection<TimeTrial> timeTrials)
        {
            using (context = new RouteConfiguratorDB())
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
            using (context = new RouteConfiguratorDB())
            {
                context.Modifications.Add(mod);
                context.SaveChanges();
            }
        }

        public void addOverrideRequest(OverrideRequest ov)
        {
            using (context = new RouteConfiguratorDB())
            {
                context.OverrideRequests.Add(ov);
                context.SaveChanges();
            }
        }

        public void addModel(Model.Model model)
        {
            using (context = new RouteConfiguratorDB())
            {
                context.Models.Add(model);
                context.SaveChanges();
            }
        }

        public void addOption(Option option)
        {
            using (context = new RouteConfiguratorDB())
            {
                context.Options.Add(option);
                context.SaveChanges();
            }
        }

        public void updateModel(string modelBase, decimal newDriveTime, decimal newAVTime)
        {
            using (context = new RouteConfiguratorDB())
            {
                Model.Model model = context.Models.Find(modelBase);

                model.DriveTime = newDriveTime;
                model.AVTime = newAVTime;

                context.SaveChanges();
            }
        }

        public void updateOption(string optionCode, string boxSize, decimal newTime, string newName)
        {
            using (context = new RouteConfiguratorDB())
            {
                Option option = context.Options.Find(optionCode, boxSize);

                option.Time = newTime;
                option.Name = newName;

                context.SaveChanges();
            }
        }

        public void addOverride(Override ov, string modelBase)
        {
            using (context = new RouteConfiguratorDB())
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
            using (context = new RouteConfiguratorDB())
            {
                Modification mod = context.Modifications.Find(modification.ModificationID);

                //Update modification info for approval or denial
                mod.ReviewDate = modification.ReviewDate;
                mod.Reviewer = modification.Reviewer;
                mod.State = modification.State == 3 ? 1 : modification.State == 4 ? 2 : modification.State;

                //Update modification info for possible director changes
                mod.ModelBase = modification.ModelBase;
                mod.BoxSize = modification.BoxSize;
                mod.NewDriveTime = modification.NewDriveTime;
                mod.NewAVTime = modification.NewAVTime;

                context.SaveChanges();
            }
        }

        public void updateOverrideRequest(OverrideRequest overrideRequest)
        {
            using (context = new RouteConfiguratorDB())
            {
                OverrideRequest or = context.OverrideRequests.Find(overrideRequest.OverrideRequestID);

                //Update modification info for possible director changes
                or.ModelNum = overrideRequest.ModelNum;
                or.OverrideTime = overrideRequest.OverrideTime;
                or.OverrideRoute = overrideRequest.OverrideRoute;

                //Update modification info for approval or denial
                or.ReviewDate = overrideRequest.ReviewDate;
                or.Reviewer = overrideRequest.Reviewer;
                or.State = overrideRequest.State == 3 ? 1 : overrideRequest.State == 4 ? 2 : overrideRequest.State;

                context.SaveChanges();
            }
        }
    }
}