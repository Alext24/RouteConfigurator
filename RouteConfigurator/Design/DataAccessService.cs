﻿using RouteConfigurator.Model;
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

        public decimal getTotalOptionsTime(string boxSize, List<string> options)
        {
            decimal totalTime = 0;

            if (options.Count() == 0)
            {
                return 0;
            }

            List<Option> optionsForBoxSize = context.Options.Where(o => o.BoxSize.Equals(boxSize)).ToList();

            foreach(string option in options)
            {
                foreach(Option op in optionsForBoxSize)
                {
                    if (op.OptionCode.Equals(option))
                    {
                        totalTime += op.Time;
                        break;
                    }
                }
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
    }
}