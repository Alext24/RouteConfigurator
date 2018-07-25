using RouteConfigurator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RouteConfigurator.Design
{
    public class DataAccessService : IDataAccessService
    {
        private RouteConfiguratorDB context;

        #region Constructor
        public DataAccessService()
        {
            context = new RouteConfiguratorDB();
        }
        #endregion

        /// <returns> returns all models </returns>
        public IEnumerable<Model.Model> getModels()
        {
            return context.Models.ToList();
        }

        /// <param name="modelFilter"> base name for the model </param>
        /// <param name="boxSizeFilter"> box size for the model </param>
        /// <returns> returns a list of the models that meet the filters</returns>
        public IEnumerable<Model.Model> getFilteredModels(string modelFilter, string boxSizeFilter)
        {
            return context.Models.Where(model => model.Base.Contains(modelFilter) && 
                                                 model.BoxSize.Contains(boxSizeFilter)).ToList();
        }

        /// <returns> returns all options </returns>
        public IEnumerable<Option> getOptions()
        {
            return context.Options.ToList();
        }

        /// <param name="optionFilter"> name for the option </param>
        /// <param name="optionBoxSizeFilter"> box size for the option </param>
        /// <returns> returns a list of the options that meet the filters</returns>
        public IEnumerable<Option> getFilteredOptions(string optionFilter, string optionBoxSizeFilter)
        {
            return context.Options.Where(option => option.OptionCode.Contains(optionFilter) && 
                                                   option.BoxSize.Contains(optionBoxSizeFilter)).ToList();
        }

        /// <param name="modelName"> base name for a model </param>
        /// <returns> returns the model specified by the model name </returns>
        public Model.Model getModel(string modelName)
        {
            Model.Model returnModel = null;

            var model = context.Models.Find(modelName);

            returnModel = model as Model.Model;

            return returnModel;
        }

        /// <param name="modelNum"> the entire model number </param>
        /// <returns> returns the override info for a model</returns>
        public Override getModelOverride(string modelNum)
        {
            Override retOverride = null;

            var overrideVal = context.Overrides.Find(modelNum);

            if(overrideVal == null)
            {
                return retOverride;
            }

            retOverride = overrideVal as Override;

            return retOverride;
        }

        /// <param name="boxSize"> box size for the model </param>
        /// <param name="options"> list of options</param>
        /// <returns> total time for the options </returns>
        public decimal getTotalOptionsTime(string boxSize, List<string> options)
        {
            decimal totalTime = 0;

            if (options.Count() == 0)
            {
                return 0;
            }

            List<Option> optionsForBoxSize = context.Options.Where(o => o.BoxSize.Equals(boxSize)).ToList();

            string errorText = "";
            bool missedOption = false;
            bool foundOption;

            foreach(string option in options)
            {
                foundOption = false;
                foreach(Option op in optionsForBoxSize)
                {
                    if (op.OptionCode.Equals(option))
                    {
                        totalTime += op.Time;
                        foundOption = true;
                        break;
                    }
                }
                if (!foundOption)
                {
                    missedOption = true;
                    errorText += string.Format("{0} not found\n", option);
                }
            }

            if (missedOption)
            {
                errorText += "Information may be inaccurate.";
                MessageBox.Show(errorText);
            }
            return totalTime;
        }

        /// <param name="modelBase"> model base to get time trials for</param>
        /// <returns> list of time trials with the model base </returns>
        public ObservableCollection<TimeTrial> getTimeTrials(string modelBase)
        {
            ObservableCollection<TimeTrial> timeTrials = new ObservableCollection<TimeTrial>();

            //Search through each stored time trial
            foreach (var timeTrial in context.TimeTrials)
            {
                // Make sure the model base matches the entered model base
                if (timeTrial.Model.Base.Equals(modelBase.ToUpper()))
                {
                    timeTrials.Add(timeTrial);
                }
            }
            return timeTrials;
        }

        /// <param name="modelBase">model base name</param>
        /// <param name="options">list of options</param>
        /// <returns> list of time trials for a specific model </returns>
        public ObservableCollection<TimeTrial> getTimeTrials(string modelBase, List<string> options)
        {
            ObservableCollection<TimeTrial> timeTrials = new ObservableCollection<TimeTrial>();

            //Search through each stored time trial
            foreach (var timeTrial in context.TimeTrials)
            {
                // Make sure the model base matches the entered model base
                if (timeTrial.Model.Base.Equals(modelBase.ToUpper()))
                {
                    // Make sure the options with the time trial are the same as the options for entered model
                    bool hasAllSameOptions = true;
                    if (timeTrial.TTOptionTimes.Count() != options.Count())
                    {
                        hasAllSameOptions = false;
                    }
                    else
                    {
                        foreach(TimeTrialsOptionTime ttot in timeTrial.TTOptionTimes)
                        {
                            if (!options.Contains(ttot.OptionCode))
                            {
                                hasAllSameOptions = false;
                                break;
                            }
                        }

                        if (hasAllSameOptions)
                        {
                            timeTrials.Add(timeTrial);
                        }
                    }
                }
            }
            return timeTrials;
        }

        /// <param name="modelBase"> base model name </param>
        /// <param name="optionTextFilter"> option text for the model number </param>
        /// <param name="salesFilter"> sales number for the time trials</param>
        /// <param name="productionNumFilter"> production number for the time trials</param>
        /// <returns> a list of time trials that meet the filters</returns>
        public IEnumerable<TimeTrial> getFilteredTimeTrials(string modelBase, string optionTextFilter, string salesFilter, string productionNumFilter)
        {
            return context.TimeTrials.Where(tt => tt.Model.Base.Contains(modelBase) &&
                                                  tt.OptionsText.Contains(optionTextFilter) &&
                                                  tt.SalesOrder.ToString().Contains(salesFilter) &&
                                                  tt.ProductionNumber.ToString().Contains(productionNumFilter)).ToList();
        }

        /// <param name="modelBase"> base model name </param>
        /// <param name="optionTextFilter"> option text for the model number </param>
        /// <param name="salesFilter"> sales number for the time trials</param>
        /// <param name="productionNumFilter"> production number for the time trials</param>
        /// <returns> a list of time trials that meet the filters</returns>
        public IEnumerable<TimeTrial> getStrictFilteredTimeTrials(string modelBase, string optionTextFilter, string salesFilter, string productionNumFilter)
        {
            return context.TimeTrials.Where(tt => tt.Model.Base.Contains(modelBase) &&
                                                  tt.OptionsText.Equals(optionTextFilter) &&
                                                  tt.SalesOrder.ToString().Contains(salesFilter) &&
                                                  tt.ProductionNumber.ToString().Contains(productionNumFilter)).ToList();
        }


        /// <param name="optionsList"> option codes to search for </param>
        /// <param name="boxSize"> box size of model </param>
        /// <returns> list of options </returns>
        public IEnumerable<Option> getModelOptions(List<string> optionsList, string boxSize)
        {
            return context.Options.Where(option => optionsList.Any(optionCode => option.OptionCode.Contains(optionCode)) && 
                                                   option.BoxSize.Contains(boxSize)).ToList();
        }

        /// <returns> list of unique drive types</returns>
        public ObservableCollection<string> getDriveTypes()
        {
            ObservableCollection<string> driveTypes = new ObservableCollection<string>();

            string driveType = "";
            foreach(Model.Model model in context.Models)
            {
                driveType = model.Base.Substring(0, 4);
                if (!driveTypes.Contains(driveType))
                {
                    driveTypes.Add(driveType);
                }
            }

            return driveTypes;
        }

        /// <param name="drive"></param>
        /// <param name="av"></param>
        /// <param name="boxSize"></param>
        /// <returns> a list of models that meet the filters</returns>
        public ObservableCollection<Model.Model> getNumModelsFound(string drive, string av, string boxSize)
        {
            ObservableCollection<Model.Model> models = new ObservableCollection<Model.Model>();

            if (string.IsNullOrWhiteSpace(boxSize))
            {
                var result = context.Models.Where(model => model.Base.Contains(drive) &&
                                                           model.Base.Contains(av)).ToList();
                foreach(Model.Model model in result)
                {
                    models.Add(model);
                }
            }
            else
            {
                var result = context.Models.Where(model => model.Base.Contains(drive) &&
                                                           model.Base.Contains(av) &&
                                                           model.BoxSize.Equals(boxSize)).ToList();
                foreach(Model.Model model in result)
                {
                    models.Add(model);
                }
            }

            return models;
        }

        public ObservableCollection<string> getOptionCodes()
        {
            ObservableCollection<string> optionCodes = new ObservableCollection<string>();

            string optionCode = "";
            foreach(Option option in context.Options)
            {
                optionCode = option.OptionCode;
                if (!optionCodes.Contains(optionCode))
                {
                    optionCodes.Add(optionCode);
                }
            }
            return optionCodes;
        }

        public ObservableCollection<Option> getNumOptionsFound(string optionCode, string boxSize)
        {
            ObservableCollection<Option> options = new ObservableCollection<Option>();

            if (string.IsNullOrWhiteSpace(boxSize))
            {
                var result = context.Options.Where(option => option.OptionCode.Contains(optionCode)).ToList();
                foreach(Option option in result)
                {
                    options.Add(option);
                }
            }
            else
            {
                var result = context.Options.Where(option => option.OptionCode.Contains(optionCode) &&
                                                             option.BoxSize.Equals(boxSize)).ToList();
                foreach(Option option in result)
                {
                    options.Add(option);
                }
            }

            return options;
        }

        /// <returns> list of active overrides</returns>
        public IEnumerable<Override> getOverrides()
        {
            return context.Overrides.ToList();
        }

        /// <param name="overrideFilter"> model number to filter overrides by</param>
        /// <returns> list of active overrides that contain the model number</returns>
        public IEnumerable<Override> getFilteredOverrides(string overrideFilter)
        {
            return context.Overrides.Where(item => item.ModelNum.Contains(overrideFilter)).ToList();
        }

        /// <summary>
        /// Adds a list of time trials to the database
        /// </summary>
        /// <param name="timeTrials"> list of new time trials </param>
        public void addTimeTrials(ObservableCollection<TimeTrial> timeTrials)
        {
            foreach (TimeTrial TT in timeTrials)
            {
                foreach (TimeTrialsOptionTime TTOT in TT.TTOptionTimes)
                {
                    context.TTOptionTimes.Add(TTOT);
                }
                context.TimeTrials.Add(TT);
            }
            context.SaveChanges();
        }

        public IEnumerable<Modification> getFilteredNewModels(string Sender, string Base, string BoxSize)
        {
            return context.Modifications.Where(item => item.IsOption == false && item.IsNew == true &&
                                                       item.State == 0 &&
                                                       item.Sender.Contains(Sender) &&
                                                       item.ModelBase.Contains(Base) &&
                                                       item.BoxSize.Contains(BoxSize)).ToList();
        }

        public IEnumerable<Modification> getFilteredNewOptions(string Sender, string OptionCode, string BoxSize)
        {
            return context.Modifications.Where(item => item.IsOption == true && item.IsNew == true &&
                                                       item.State == 0 && 
                                                       item.Sender.Contains(Sender) &&
                                                       item.OptionCode.Contains(OptionCode) &&
                                                       item.BoxSize.Contains(BoxSize)).ToList();
        }

        public IEnumerable<Modification> getFilteredModifiedModels(string Sender, string ModelName)
        {
            return context.Modifications.Where(item => item.IsOption == false && item.IsNew == false &&
                                                       item.State == 0 &&
                                                       item.Sender.Contains(Sender) &&
                                                       item.ModelBase.Contains(ModelName)).ToList();
        }

        public IEnumerable<Modification> getFilteredModifiedOptions(string Sender, string OptionCode, string BoxSize)
        {
            return context.Modifications.Where(item => item.IsOption == true && item.IsNew == false &&
                                                       item.State == 0 &&
                                                       item.Sender.Contains(Sender) && 
                                                       item.OptionCode.Contains(OptionCode) &&
                                                       item.BoxSize.Contains(BoxSize)).ToList();
        }

        public IEnumerable<OverrideRequest> getFilteredOverrideRequests(string Sender, string ModelNum)
        {
            return context.OverrideRequests.Where(item => item.State == 0 &&
                                                          item.Sender.Contains(Sender) && 
                                                          item.ModelNum.Contains(ModelNum)).ToList();
        }
    }
}