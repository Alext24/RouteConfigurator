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
        //private WorkHoursDBContext context;
        private RouteConfiguratorDB context;

        public DataAccessService()
        {
            //   context = new WorkHoursDBContext();
            context = new RouteConfiguratorDB();
        }

        public ObservableCollection<Model.Model> getModels()
        {
            ObservableCollection<Model.Model> models = new ObservableCollection<Model.Model>();

            foreach(Model.Model model in context.Models)
            {
                models.Add(model);
            }

            return models;
        }

        public ObservableCollection<Model.Model> getFilteredModels(string modelFilter, string boxSizeFilter)
        {

            ObservableCollection<Model.Model> models = new ObservableCollection<Model.Model>();

            var result = context.Models.Where(model => model.Base.Contains(modelFilter) && 
                                                       model.BoxSize.Contains(boxSizeFilter)).ToList();

            foreach(Model.Model model in result)
            {
                models.Add(model);
            }

            return models;
        }

        public ObservableCollection<Option> getOptions()
        {
            ObservableCollection<Option> options = new ObservableCollection<Option>();

            foreach(Option option in context.Options)
            {
                options.Add(option);
            }

            return options;
        }

        public ObservableCollection<Option> getFilteredOptions(string optionFilter, string optionBoxSizeFilter)
        {

            ObservableCollection<Option> options = new ObservableCollection<Option>();

            var result = context.Options.Where(option => option.OptionCode.Contains(optionFilter) && 
                                                       option.BoxSize.Contains(optionBoxSizeFilter)).ToList();

            foreach(Option option in result)
            {
                options.Add(option);
            }

            return options;
        }

        /// <summary>
        /// Gets a specific model
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public Model.Model getModel(string modelName)
        {
            Model.Model returnModel = null;

            var model = context.Models.Find(modelName);

            if(model == null)
            {
                throw new Exception("Model not found");
            }

            returnModel = model as Model.Model;

            return returnModel;
        }

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

        public ObservableCollection<Option> getModelOptions(List<string> optionsList, string boxSize)
        {
            ObservableCollection<Option> options = new ObservableCollection<Option>();

            var result = context.Options.Where(option => optionsList.Any(optionCode => option.OptionCode.Contains(optionCode)) && 
                                                         option.BoxSize.Contains(boxSize)).ToList();

            foreach(Option option in result)
            {
                options.Add(option);
            }

            return options;
        }


    }
}