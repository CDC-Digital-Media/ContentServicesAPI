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
using System.Linq;
using System.Xml.Linq;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataServices.Bo
{
    public class FilterCriterionDateRange : FilterCriterion
    {
        #region dynamicProperties
        public SearchDate SearchType { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime SingleDate { get; set; }
        public SingleDateType TheSingleDateType { get; set; }
        public RollingDateOperatorType.RollingDateOperator RollingDateType { get; set; }


        public enum SearchDate { DateRange, SingleDate, RollingDate };
        #endregion dynamicProperties

        #region configurationProperties
        public override string ParameterType { get { return "DateRange"; } }
        public bool AllowDateInPast { get; set; }
        public bool AllowDateInFuture { get; set; }
        public List<SingleDateType> TheSingleDateTypeList { get; set; }
        private static List<SingleDateType> StaticSingleDateTypeList { get; set; }
        public List<RollingDateOperatorType> TheRollingDateTypeList { get; set; }
        private static List<RollingDateOperatorType> StaticRollingDateTypeList { get; set; }
        #endregion configurationProperties


        public List<RollingDateOperatorType> RollingDateTypeList()
        {
            return (from t in RollingDateOperatorType.TypeList()
                    where ((AllowDateInFuture || t.InFuture != true) && (AllowDateInPast || t.InPast != true))
                    select t).ToList();
        }

        public FilterCriterionDateRange(DateTime? fromDate, DateTime? toDate)
        {
            Initialize();
            SetAllowedDates(true, true);

            if (fromDate != null && toDate == null)
            {
                SetAsSingleDate(SingleDateType.SingleDateOperator.GtEq, (DateTime)fromDate);
            }
            else if (fromDate == null && toDate != null)
            {
                SetAsSingleDate(SingleDateType.SingleDateOperator.LtEq, (DateTime)toDate);
            }
            else if (fromDate != null && toDate != null)
            {
                IsFiltered = true;
                SearchType = SearchDate.DateRange;
                BeginDate = (DateTime)fromDate;
                EndDate = (DateTime)toDate;
            }

        }
        public FilterCriterionDateRange(bool allowDateInPast, bool allowDateInFuture)
        {
            Initialize();
            SetAllowedDates(allowDateInPast, allowDateInFuture);
        }

        private void SetAllowedDates(bool allowDateInPast, bool allowDateInFuture)
        {
            AllowDateInPast = allowDateInPast;
            AllowDateInFuture = allowDateInFuture;
            TheRollingDateTypeList = RollingDateTypeList();
        }

        private void Initialize()
        {
            if (StaticSingleDateTypeList == null)
            {
                StaticSingleDateTypeList = SingleDateType.TypeList();
            }
            TheSingleDateTypeList = StaticSingleDateTypeList;     

            //Set some defaults
            SearchType = SearchDate.DateRange;
            BeginDate = DateTime.UtcNow.Date;
            EndDate = DateTime.UtcNow.Date;
            SingleDate = DateTime.UtcNow.Date;
            TheSingleDateType = new SingleDateType(SingleDateType.SingleDateOperator.Eq);
            RollingDateType = RollingDateOperatorType.RollingDateOperator.ThisMonth;
            AllowDateInPast = true;
            AllowDateInFuture = true;
        }

        public override string ValueToString()
        {
            return string.Empty;
        }

        #region SerializeDeserializeXml


        protected override void SetCriteriaValues(XElement values)
        {
            SearchType = (SearchDate)Enum.Parse(typeof(SearchDate), values.Attribute("SearchDateType").Value);
            switch (SearchType)
            {
                case SearchDate.DateRange:
                    BeginDate = values.GetAttributeValueAsDateTime("BeginDate");
                    EndDate = values.GetAttributeValueAsDateTime("EndDate");
                    break;
                case SearchDate.SingleDate:
                    SingleDate = values.GetAttributeValueAsDateTime("SingleDate");
                    TheSingleDateType = new SingleDateType(values.Attribute("SingleDateType").Value);
                    break;
                case SearchDate.RollingDate:
                    RollingDateType = (RollingDateOperatorType.RollingDateOperator)Enum.Parse(typeof(RollingDateOperatorType.RollingDateOperator), values.Attribute("RollingDateType").Value);
                    break;
            }
        }

        public override void SetCriteriaValues(FilterCriterion criterion)
        {
            base.SetCriteriaValues(criterion);
            FilterCriterionDateRange dr = (FilterCriterionDateRange)criterion;
            SearchType = dr.SearchType;
            BeginDate = dr.BeginDate;
            EndDate = dr.EndDate;
            SingleDate = dr.SingleDate;
            TheSingleDateType = dr.TheSingleDateType;
            RollingDateType = dr.RollingDateType;
        }
        #endregion

        private void SetAsSingleDate(SingleDateType.SingleDateOperator dateOperator, DateTime date)
        {
            IsFiltered = true;
            SearchType = SearchDate.SingleDate;
            TheSingleDateType = new SingleDateType(dateOperator);
            SingleDate = date;
        }

        public override void SetCriteriaValues(Criterion criterion)
        {
            base.SetCriteriaValues(criterion);
            SearchType = (SearchDate)Enum.Parse(typeof(SearchDate), criterion.DateType);
            switch (SearchType)
            {
                case SearchDate.DateRange:
                    if (string.IsNullOrEmpty(criterion.Date1))
                    {
                        SetAsSingleDate(SingleDateType.SingleDateOperator.LtEq, DateTime.Parse(criterion.Date2));
                    }
                    else if (string.IsNullOrEmpty(criterion.Date2))
                    {
                        SetAsSingleDate(SingleDateType.SingleDateOperator.GtEq, DateTime.Parse(criterion.Date1));
                    }
                    else
                    {
                        BeginDate = DateTime.Parse(criterion.Date1);
                        EndDate = DateTime.Parse(criterion.Date2);
                    }
                    break;

                case SearchDate.SingleDate:
                    TheSingleDateType = new SingleDateType(criterion.DateOperator);
                    SingleDate = DateTime.Parse(criterion.Date1);
                    break;

                case SearchDate.RollingDate:
                    RollingDateType = (RollingDateOperatorType.RollingDateOperator)Enum.Parse(typeof(RollingDateOperatorType.RollingDateOperator), criterion.DateOperator);
                    break;
            }
        }

        private DateRange GetSingleDateRange()
        {
            switch (TheSingleDateType.Type)
            {
                case SingleDateType.SingleDateOperator.Eq:
                    return new DateRange(SingleDate, SingleDate);
                case SingleDateType.SingleDateOperator.Gt:
                    DateTime? nextDay = SingleDate == null ? (DateTime?)null : ((DateTime)SingleDate).AddDays(1);
                    return new DateRange(nextDay, null);
                case SingleDateType.SingleDateOperator.GtEq:
                    return new DateRange(SingleDate, null);
                case SingleDateType.SingleDateOperator.Lt:
                    DateTime? previousDay = SingleDate == null ? (DateTime?)null : ((DateTime)SingleDate).AddDays(-1);
                    return new DateRange(null, previousDay);
                case SingleDateType.SingleDateOperator.LtEq:
                    return new DateRange(null, SingleDate);
            }
            return new DateRange(null, null);

        }

        private DateRange GetRollingDateRange()
        {
            DateTime now = DateTime.UtcNow;
            DateTime firstDayThisMonth = new DateTime(now.Year, now.Month, 1);
            DateTime firstDayThisYear = new DateTime(now.Year, 1, 1);
            DateTime firstDayThisWeek = now.AddDays(-(int)now.DayOfWeek);
            DateTime firstdayThisQuarter = new DateTime(now.Year, 1 + (now.Month - 1) / 3 * 3, 1);

            switch (RollingDateType)
            {
                case RollingDateOperatorType.RollingDateOperator.LastMonth:
                    return new DateRange(firstDayThisMonth.AddMonths(-1), firstDayThisMonth.AddDays(-1));
                case RollingDateOperatorType.RollingDateOperator.LastWeek:
                    return new DateRange(firstDayThisWeek.AddDays(-7), firstDayThisWeek.AddDays(-1));
                case RollingDateOperatorType.RollingDateOperator.LastQuarter:
                    return new DateRange(firstdayThisQuarter.AddMonths(-3), firstdayThisQuarter.AddDays(-1));
                case RollingDateOperatorType.RollingDateOperator.LastYear:
                    return new DateRange(firstDayThisYear.AddYears(-1), firstDayThisYear.AddDays(-1));
                case RollingDateOperatorType.RollingDateOperator.NextMonth:
                    return new DateRange(firstDayThisMonth.AddMonths(1), firstDayThisMonth.AddMonths(2).AddDays(-1));
                case RollingDateOperatorType.RollingDateOperator.NextWeek:
                    return new DateRange(firstDayThisWeek.AddDays(7), firstDayThisWeek.AddDays(13));
                case RollingDateOperatorType.RollingDateOperator.NextQuarter:
                    return new DateRange(firstdayThisQuarter.AddMonths(3), firstdayThisQuarter.AddMonths(3).AddDays(-1));
                case RollingDateOperatorType.RollingDateOperator.NextYear:
                    return new DateRange(firstDayThisYear.AddYears(1), firstDayThisYear.AddYears(2).AddDays(-1));
                case RollingDateOperatorType.RollingDateOperator.ThisMonth:
                    return new DateRange(firstDayThisMonth, firstDayThisMonth.AddMonths(1).AddDays(-1));
                case RollingDateOperatorType.RollingDateOperator.ThisWeek:
                    return new DateRange(firstDayThisWeek, firstDayThisWeek.AddDays(6));
                case RollingDateOperatorType.RollingDateOperator.ThisQuarter:
                    return new DateRange(firstdayThisQuarter, firstdayThisQuarter.AddMonths(3).AddDays(-1));
                case RollingDateOperatorType.RollingDateOperator.ThisYear:
                    return new DateRange(firstDayThisYear, firstDayThisYear.AddYears(1).AddDays(-1));
                case RollingDateOperatorType.RollingDateOperator.Today:
                    return new DateRange(now, now);
                case RollingDateOperatorType.RollingDateOperator.Tomorrow:
                    return new DateRange(now.AddDays(1), now.AddDays(1));
                case RollingDateOperatorType.RollingDateOperator.Yesterday:
                    return new DateRange(now.AddDays(-1), now.AddDays(-1));
                case RollingDateOperatorType.RollingDateOperator.Next30Days:
                    return new DateRange(now, now.AddDays(30 - 1));
                case RollingDateOperatorType.RollingDateOperator.Next60Days:
                    return new DateRange(now, now.AddDays(60 - 1));
                case RollingDateOperatorType.RollingDateOperator.Next90Days:
                    return new DateRange(now, now.AddDays(90 - 1));
            }
            return new DateRange(null, null);

        }

        public DateRange GetDateRange()
        {
            switch (SearchType)
            {
                case FilterCriterionDateRange.SearchDate.DateRange:
                    if (BeginDate != null && EndDate != null)
                    {
                        return new DateRange(BeginDate, EndDate);
                    }
                    break;
                case FilterCriterionDateRange.SearchDate.SingleDate:
                    if (SingleDate != null)
                    {
                        return GetSingleDateRange();
                    }
                    break;
                case FilterCriterionDateRange.SearchDate.RollingDate:
                    return GetRollingDateRange();
            }
            return new DateRange(null, null);

        }

        public override string ToString()
        {
            string results = base.ToString();
            switch (SearchType)
            {
                case SearchDate.DateRange:
                    if (BeginDate != null)
                    {
                        results += "from " + ((DateTime)BeginDate).ToShortDateString();
                    }
                    if (EndDate != null)
                    {
                        results += " to " + ((DateTime)EndDate).ToShortDateString();
                    }
                    break;
                case SearchDate.SingleDate:
                    results += TheSingleDateType.Name + " ";
                    if (SingleDate != null)
                    {
                        results += ((DateTime)SingleDate).ToShortDateString();
                    }
                    break;
                case SearchDate.RollingDate:
                    results += RollingDateType.ToString();
                    break;
            }
            return Code + ": " + results;
        }

        public override void SetValues(string value)
        {
            base.SetValues(value);
        }

    }
}
