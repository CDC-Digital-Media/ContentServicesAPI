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

using Gov.Hhs.Cdc.CsBusinessObjects.Admin;
using Gov.Hhs.Cdc.DataServices.Bo;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System;

namespace Gov.Hhs.Cdc.Schedule
{
    public class ScheduleManager
    {
        private static ScheduleObjectContextFactory ObjectContextFactory { get; set; }

        static ScheduleManager()
        {
            ObjectContextFactory = new ScheduleObjectContextFactory();
        }

        #region RunHistory
        public static IList<SchedulerRunHistory> GetRunHistory()
        {
            using (var objectContext = ObjectContextFactory.Create() as ScheduleObjectContext)
            {
                return ScheduleCtl.GetScheduleRunHistory(objectContext).ToList();
            }
        }

        public static IList<SchedulerRunHistory> GetRunHistoryByStatus(bool runstatus)
        {
            var history = GetRunHistory();
            return history.Where(h => h.runStatus == runstatus).ToList();
        }

        public static IList<SchedulerRunHistory> GetRunHistoryById(Int64 Id)
        {
            var history = GetRunHistory();
            return history.Where(h => h.historyId == Id).ToList();
        }

        public static IList<SchedulerRunHistory> GetRunHistoryByName(string name)
        {
            var history = GetRunHistory();
            return history.Where(h => h.scheduleName == name).ToList();
        }

        public static IList<SchedulerRunHistory> GetRunHistoryByUtilityName(string name)
        {
            var history = GetRunHistory();
            return history.Where(h => h.utilityName == name).ToList();
        }

        public static IList<SchedulerRunHistory> GetRunHistoryByDateRange(DateTime begin, DateTime? end)
        {
            var history = GetRunHistory();
            if (end == null)
            {
                return history.Where(h => h.startDateTime > begin).ToList();
            }
            else
            {
                return history.Where(h => h.startDateTime > begin & h.startDateTime < end).ToList();
            }
        } 
        #endregion

        #region ScheduleEntry
        public static IList<SchedulerEntry> GetScheduleEntry()
        {
            using (var objectContext = ObjectContextFactory.Create() as ScheduleObjectContext)
            {
                return ScheduleCtl.GetScheduleEntries(objectContext).ToList();
            }
        }

        public static IList<SchedulerEntry> GetScheduleEntryById(Int32 Id)
        {
            var schedule = GetScheduleEntry();
            return schedule.Where(h => h.scheduleId == Id).ToList();
        }

        public static IList<SchedulerEntry> GetScheduleEntryByType(string typename)
        {
            var schedule = GetScheduleEntry();
            return schedule.Where(h => h.scheduleType == typename).ToList();
        } 
        #endregion

        #region Utility
        public static IList<SchedulerUtility> GetUtility()
        {
            using (var objectContext = ObjectContextFactory.Create() as ScheduleObjectContext)
            {
                return ScheduleCtl.GetUtilities(objectContext).ToList();
            }
        }

        public static IList<SchedulerUtility> GetUtilityById(Int32 Id)
        {
            var utility = GetUtility();
            return utility.Where(h => h.utilityId == Id).ToList();
        }

        public static IList<SchedulerUtility> GetUtilityByName(string name)
        {
            var utility = GetUtility();
            return utility.Where(h => h.name == name).ToList();
        } 
        #endregion

        #region ScheduleTask
        public static IList<SchedulerTask> GetTaskSchedule()
        {
            using (var objectContext = ObjectContextFactory.Create() as ScheduleObjectContext)
            {
                return ScheduleCtl.GetTaskSchedules(objectContext).ToList();
            }
        }

        public static IList<SchedulerTask> GetTaskScheduleById(Int32 Id)
        {
            var task = GetTaskSchedule();
            return task.Where(h => h.scheduleId == Id).ToList();
        }

        public static IList<SchedulerTask> GetTaskScheduleByName(string name)
        {
            var task = GetTaskSchedule();
            return task.Where(h => h.name == name).ToList();
        }

        public static IList<SchedulerTask> GetTaskScheduleByUtilityId(Int64 Id)
        {
            var task = GetTaskSchedule();
            return task.Where(h => h.utilityId == Id).ToList();
        }  
        #endregion

        public static IList<SchedulerType> GetScheduleTypes()
        {
            using (var objectContext = ObjectContextFactory.Create() as ScheduleObjectContext)
            {
                return ScheduleCtl.GetScheduleTypes(objectContext).ToList();
            }
        }

        public static IList<SchedulerIntervalType> GetScheduleIntervalTypes()
        {
            using (var objectContext = ObjectContextFactory.Create() as ScheduleObjectContext)
            {
                return ScheduleCtl.GetScheduleIntervalTypes(objectContext).ToList();
            }
        }
    }
}
