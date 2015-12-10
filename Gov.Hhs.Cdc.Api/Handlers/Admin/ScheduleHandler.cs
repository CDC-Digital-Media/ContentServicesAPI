// Copyright [2015] [Centers for Disease Control and Prevention] 
// Licensed under the CDC Custom Open Source License 1 (the 'License'); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
// 
//   http://t.cdc.gov/O4O
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an 'AS IS' BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CsBusinessObjects.Admin;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Logging;
using Gov.Hhs.Cdc.Schedule;

namespace Gov.Hhs.Cdc.Api
{
    public static class ScheduleHandler
    {
        private static ScheduleManager ScheduleProvider
        {
            get
            {
                return new ScheduleManager();
            }
        }

        public static void Get(IOutputWriter writer, string id, string action, ICallParser parser)
        {          
            if (parser.ParamDictionary["src"] == null)
            {
                writer.Write(ValidationMessages.CreateError("Schedule", "Invalid Scheduler Request."));
                return;
            }
            
            string source = parser.ParamDictionary["src"];

            switch (source.ToLower())
            {
                case "schedule":
                    IList<SchedulerEntry> entries = null;

                    entries = GetSchedules(parser);
                    var scheduleData = new SerialResponse { results = entries };
                    writer.Write(scheduleData);
                    break;
                case "history":
                    IList<SchedulerRunHistory> history = null;

                    history = GetRunHistory(parser);
                    var historyData = new SerialResponse { results = history };
                    writer.Write(historyData);
                    break;
                case "task":
                    IList<SchedulerTask> tasks = null;

                    tasks = GetTask(parser);
                    var taskData = new SerialResponse { results = tasks };
                    writer.Write(taskData);
                    break;
                case "utility":
                    IList<SchedulerUtility> utility = null;

                    utility = GetUtilities(parser);
                    var utilityData = new SerialResponse { results = utility };
                    writer.Write(utilityData);
                    break;
                default:
                    writer.Write(ValidationMessages.CreateError("Schedule", "Invalid Scheduler Request."));
                    break;
            }
        }

        private static IList<SchedulerRunHistory> GetRunHistory(ICallParser parser)
        {
            IList<SchedulerRunHistory> history = null;

            if (parser.ParamDictionary.ContainsKey("tk"))
            {
                if (parser.ParamDictionary["tk"] == null || parser.ParamDictionary["tk"].Trim() == "")
                {
                    Logger.LogError("Invalid Request - Id parameter missing", "GetRunHistory");
                    throw new Exception("Invalid Schedule History Request - Id parameter missing");
                }
                else
                {
                    Int64 Id = Convert.ToInt64(parser.ParamDictionary["tk"]);
                    history = ScheduleManager.GetRunHistoryById(Id);
                }
            } 
            else if (parser.ParamDictionary.ContainsKey("name"))
            {
                if (parser.ParamDictionary["name"] == null || parser.ParamDictionary["name"].Trim() == "")
                {
                    Logger.LogError("Invalid Request - Name parameter missing", "GetRunHistory");
                    throw new Exception("Invalid Schedule History Request - Name parameter missing");
                }
                else
                {
                    string name = parser.ParamDictionary["name"];
                    history = ScheduleManager.GetRunHistoryByName(name);
                }
            }
            else if (parser.ParamDictionary.ContainsKey("status"))
            {
                if (parser.ParamDictionary["status"] == null || parser.ParamDictionary["status"].Trim() == "")
                {
                    Logger.LogError("Invalid Request - Status parameter missing", "GetRunHistory");
                    throw new Exception("Invalid Schedule History Request - Status parameter missing");
                }
                else
                {
                    bool runstatus = Convert.ToBoolean(parser.ParamDictionary["status"]);
                    history = ScheduleManager.GetRunHistoryByStatus(runstatus);
                }
            }
            else if (parser.ParamDictionary.ContainsKey("utility"))
            {
                if (parser.ParamDictionary["utility"] == null || parser.ParamDictionary["utility"].Trim() == "")
                {
                    Logger.LogError("Invalid Request - Utility parameter missing", "GetRunHistory");
                    throw new Exception("Invalid Schedule History Request - Utility parameter missing");
                }
                else
                {
                    string name = parser.ParamDictionary["utility"];
                    history = ScheduleManager.GetRunHistoryByUtilityName(name);
                }
            }
            else if (parser.ParamDictionary.ContainsKey("begin"))
            {
                if (parser.ParamDictionary["begin"] == null || parser.ParamDictionary["begin"].Trim() == "")
                {
                    Logger.LogError("Invalid Request - Date parameter missing", "GetRunHistory");
                    throw new Exception("Invalid Schedule History Request - Date parameter missing");
                }
                else
                {
                    DateTime st = (DateTime)ParseUtcDate(parser.ParamDictionary["begin"]);
                    DateTime ed = new DateTime();

                    if (parser.ParamDictionary.ContainsKey("end"))
                    {
                        ed = (DateTime)ParseUtcDate(parser.ParamDictionary["end"]);
                        history = ScheduleManager.GetRunHistoryByDateRange(st, ed);
                    }
                    else
                    {
                        history = ScheduleManager.GetRunHistoryByDateRange(st, null);
                    }
                }
            }
            else
            {
                Logger.LogError("Invalid Request - No Parameters", "GetRunHistory");
                throw new Exception("Invalid Schedule History Request - No Parameters");
            }

            return history;
        }

        private static IList<SchedulerEntry> GetSchedules(ICallParser parser)
        {
            IList<SchedulerEntry> entries = null;

            if (parser.ParamDictionary.ContainsKey("tk"))
            {
                if (parser.ParamDictionary["tk"] == null || parser.ParamDictionary["tk"].Trim() == "")
                {
                    Logger.LogError("Invalid Request - Id parameter missing", "GetSchedules");
                    throw new Exception("Invalid Schedule Request - Id parameter missing");
                }
                else
                {
                    Int32 Id = Convert.ToInt32(parser.ParamDictionary["tk"]);
                    entries = ScheduleManager.GetScheduleEntryById(Id);
                }
            }
            else if (parser.ParamDictionary.ContainsKey("type"))
            {
                if (parser.ParamDictionary["type"] == null || parser.ParamDictionary["type"].Trim() == "")
                {
                    Logger.LogError("Invalid Request - type parameter missing", "GetSchedules");
                    throw new Exception("Invalid Schedule Request - Type parameter missing");
                }
                else
                {
                    string stype = parser.ParamDictionary["type"];
                    entries = ScheduleManager.GetScheduleEntryByType(stype);
                }
            }
            else
            {
                Logger.LogError("Invalid Request - No Parameters", "GetSchedules");
                throw new Exception("Invalid Schedule Request - No Parameters");
            }

            return entries;
        }

        private static IList<SchedulerUtility> GetUtilities(ICallParser parser)
        {
            IList<SchedulerUtility> utilities = null;

            if (parser.ParamDictionary.ContainsKey("tk"))
            {
                if (parser.ParamDictionary["tk"] == null || parser.ParamDictionary["tk"].Trim() == "")
                {
                    Logger.LogError("Invalid Request - Id parameter missing", "GetUtilities");
                    throw new Exception("Invalid Schedule Utility Request - Id parameter missing");
                }
                else
                {
                    Int32 Id = Convert.ToInt32(parser.ParamDictionary["tk"]);
                    utilities = ScheduleManager.GetUtilityById(Id);
                }
            }
            else if (parser.ParamDictionary.ContainsKey("name"))
            {
                if (parser.ParamDictionary["name"] == null || parser.ParamDictionary["name"].Trim() == "")
                {
                    Logger.LogError("Invalid Request - Name parameter missing", "GetUtilities");
                    throw new Exception("Invalid Schedule Utility Request - Name parameter missing");
                }
                else
                {
                    string stype = parser.ParamDictionary["name"];
                    utilities = ScheduleManager.GetUtilityByName(stype);
                }
            }
            else
            {
                Logger.LogError("Invalid Request - No Parameters", "GetUtilities");
                throw new Exception("Invalid Schedule Utility Request - No Parameters");
            }

            return utilities;
        }

        private static IList<SchedulerTask> GetTask(ICallParser parser)
        {
            IList<SchedulerTask> tasks = null;

            if (parser.ParamDictionary.ContainsKey("tk"))
            {
                if (parser.ParamDictionary["tk"] == null || parser.ParamDictionary["tk"].Trim() == "")
                {
                    Logger.LogError("Invalid Request - Id parameter missing", "GetTask");
                    throw new Exception("Invalid Schedule Task Request - Id parameter missing");
                }
                else
                {
                    Int32 Id = Convert.ToInt32(parser.ParamDictionary["tk"]);
                    tasks = ScheduleManager.GetTaskScheduleById(Id);
                }
            }
            else if (parser.ParamDictionary.ContainsKey("name"))
            {
                if (parser.ParamDictionary["name"] == null || parser.ParamDictionary["name"].Trim() == "")
                {
                    Logger.LogError("Invalid Request - Name parameter missing", "GetTask");
                    throw new Exception("Invalid Schedule Task Request - Name parameter missing");
                }
                else
                {
                    string stype = parser.ParamDictionary["name"];
                    tasks = ScheduleManager.GetTaskScheduleByName(stype);
                }
            }
            else if (parser.ParamDictionary.ContainsKey("utility"))
            {
                if (parser.ParamDictionary["utility"] == null || parser.ParamDictionary["utility"].Trim() == "")
                {
                    Logger.LogError("Invalid Request - Utility Id parameter missing", "GetTask");
                    throw new Exception("Invalid Schedule Task Request - Utility Id parameter missing");
                }
                else
                {
                    Int64 Id = Convert.ToInt64(parser.ParamDictionary["utility"]);
                    tasks = ScheduleManager.GetTaskScheduleByUtilityId(Id);
                }
            }
            else
            {
                Logger.LogError("Invalid Request - No Parameters", "GetTask");
                throw new Exception("Invalid Schedule Task Request - No Parameters");
            }

            return tasks;
        }


        //could be a global method
        private static DateTime? ParseUtcDate(string strDate)
        {
            DateTimeOffset result;
            return DateTimeOffset.TryParse(strDate, out result) ? result.UtcDateTime : (DateTime?)null;
        }
    }
}
