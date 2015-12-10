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
using System.Data.Objects;
using System.Linq;
using System.Transactions;
using System.Xml;
using System.Xml.Linq;
using Gov.Hhs.Cdc.CsBusinessObjects.Admin;

namespace Gov.Hhs.Cdc.Schedule
{
    public static class ScheduleCtl
    {
        public static IQueryable<SchedulerRunHistory> GetScheduleRunHistory(ScheduleObjectContext db)
        {
            var tasks = GetTaskSchedule(db);
            var entries = GetScheduleEntry(db);
            var history = GetTaskHistory(db);
            var utility = GetUtility(db);


            var query = from h in history
                        join t in tasks on h.ScheduleId equals t.ScheduleId
                        join e in entries on h.ScheduleId equals e.ScheduleId
                        join u in utility on t.UtilityId equals u.UtilityId
                        select new SchedulerRunHistory
                        {
                            historyId = h.HistoryId,
                            scheduleId = h.ScheduleId,
                            utilityId = h.UtilityId,
                            scheduleName = t.Name,
                            description = t.Description,
                            configurationData = t.ConfigurationData,
                            runStatus = (h.WasSuccessful == null) ? false : (bool)h.WasSuccessful,
                            resultText = h.ResultText,
                            scheduleType = e.ScheduleType,
                            scheduleIntervalType = e.ScheduleIntervalType,
                            scheduleInterval = (e.ScheduleInterval == null) ? 0 : (Int32)e.ScheduleInterval,
                            utilityName = u.Name,
                            startDateTime = h.StartDateTime,
                            endDateTime = (h.EndDateTime == null) ? new DateTime() : (DateTime)h.EndDateTime
                        };

            return query;
        }

        private static IQueryable<ScheduleEntry> GetScheduleEntry(ScheduleObjectContext db)
        {
            return db.ScheduleDbEntities.ScheduleEntries;
        }

        private static IQueryable<TaskSchedule> GetTaskSchedule(ScheduleObjectContext db)
        {
            return db.ScheduleDbEntities.TaskSchedules;
        }

        private static IQueryable<TaskHistory> GetTaskHistory(ScheduleObjectContext db)
        {
            return db.ScheduleDbEntities.TaskHistories;
        }

        private static IQueryable<Utility> GetUtility(ScheduleObjectContext db)
        {
            return db.ScheduleDbEntities.Utilities;
        }

        public static IQueryable<SchedulerType> GetScheduleTypes(ScheduleObjectContext db)
        {
            return db.ScheduleDbEntities.ScheduleTypes.Select(
                r => new SchedulerType
                {
                    scheduleType = r.ScheduleType1,
                    scheduleTypeName = r.ScheduleTypeName
                }
            );
        }

        public static IQueryable<SchedulerIntervalType> GetScheduleIntervalTypes(ScheduleObjectContext db)
        {
            return db.ScheduleDbEntities.ScheduleIntervalTypes.Select(
                r => new SchedulerIntervalType
                {
                    scheduleIntervalType = r.ScheduleIntervalType1,
                    scheduleIntervalName = r.ScheduleIntervalName
                }
            );
        }

        public static IQueryable<SchedulerTaskHistory> GetTaskHistories(ScheduleObjectContext db)
        {
            return db.ScheduleDbEntities.TaskHistories.Select(
                r => new SchedulerTaskHistory
                {
                    historyId = r.HistoryId,
                    scheduleId = r.ScheduleId,
                    utilityId = r.UtilityId,
                    runStatus = (r.WasSuccessful == null) ? false : (bool)r.WasSuccessful,
                    resultText = r.ResultText,
                    startDateTime = r.StartDateTime,
                    endDateTime = (r.EndDateTime == null) ? new DateTime() : (DateTime)r.EndDateTime
                }
            );
        }

        public static IQueryable<SchedulerUtility> GetUtilities(ScheduleObjectContext db)
        {
            return db.ScheduleDbEntities.Utilities.Select(
                r => new SchedulerUtility
                {
                    utilityId = r.UtilityId,
                    name = r.Name,
                    description = r.Description,
                    assemblyLocation = r.AssemblyLocation,
                    createDateTime = r.CreatedDateTime,
                    modifiedDateTime = r.ModifiedDateTime 
                }
            );
        }

        public static IQueryable<SchedulerTask> GetTaskSchedules(ScheduleObjectContext db)
        {
            return db.ScheduleDbEntities.TaskSchedules.Select(
                r => new SchedulerTask
                {
                    scheduleId = r.ScheduleId,
                    utilityId = r.UtilityId,
                    name = r.Name,
                    description = r.Description,
                    configurationData = r.ConfigurationData,
                    parameter = r.Parameter,
                    active = r.Active,
                    createDateTime = r.CreatedDateTime,
                    modifiedDateTime = r.ModifiedDateTime
                }
            );
        }

        public static IQueryable<SchedulerEntry> GetScheduleEntries(ScheduleObjectContext db)
        {
            return db.ScheduleDbEntities.ScheduleEntries.Select(
                r => new SchedulerEntry
                {
                    scheduleId = r.ScheduleId,
                    startDateTime = r.StartDateTime,
                    endDateTime = (r.EndDateTime == null) ? new DateTime() : (DateTime)r.EndDateTime,
                    scheduleType = r.ScheduleType,
                    lastDayOfMonth = (r.LastDayOfMonth == null) ? false : (bool)r.LastDayOfMonth,
                    scheduleIntervalType = r.ScheduleIntervalType,
                    scheduleInterval = (r.ScheduleInterval == null) ? 0 : (Int32)r.ScheduleInterval,
                    createDateTime = r.CreatedDateTime,
                    modifiedDateTime = r.ModifiedDateTime
                }
            );
        }
    }
}
