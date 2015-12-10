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
using System.Linq.Dynamic;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataServices.Bo
{
    public static class DataServicesLinqExtensions
    {

        public static IQueryable<T> Where<T>(this IQueryable<T> query, FilterCriterion criterion, ReflectionQualifiedNames qualifiedNames)
        {
            if (criterion != null && criterion.GetType() == typeof(FilterCriterionDateRange))
            {
                return query.Where((FilterCriterionDateRange)criterion, qualifiedNames);
            }
            if (criterion != null && criterion.GetType() == typeof(FilterCriterionMultiSelect))
            {
                return query.Where((FilterCriterionMultiSelect)criterion, qualifiedNames);
            }
            if (criterion != null && criterion.GetType() == typeof(FilterCriterionSingleSelect))
            {
                return query.Where((FilterCriterionSingleSelect)criterion, qualifiedNames);
            }
            if (criterion != null && criterion.GetType() == typeof(FilterCriterionText))
            {
                return query.Where((FilterCriterionText)criterion, qualifiedNames);
            }
            if (criterion != null && criterion.GetType() == typeof(FilterCriterionBoolean))
            {
                return query.Where((FilterCriterionBoolean)criterion, qualifiedNames);
            }

            return query;
        }


        public static IQueryable<T> Where<T>(this IQueryable<T> query, FilterCriterionDateRange criterionDateRange, ReflectionQualifiedNames qualifiedNames)
        {
            if (criterionDateRange.GetType() == typeof(FilterCriterionDateRange))
            {
                if (criterionDateRange != null && criterionDateRange.IsFiltered)
                {
                    string dbColumnName = qualifiedNames.GetQualifiedName(criterionDateRange.DbColumnName);
                    DateRange dateRange = criterionDateRange.GetDateRange();
                    if (dateRange.HasBeginDate)
                    {
                        query = query.Where(dbColumnName + " >= @0", dateRange.BeginDate);
                    }
                    DateTime theDate = dateRange.BeginningOfDayAfterEndDate();
                    if (dateRange.HasEndDate)
                    {
                        query = query.Where(dbColumnName + " < @0", theDate);
                    }
                }
            }
            return query;
        }

        //http://blog.walteralmeida.com/2010/05/advanced-linq-dynamic-linq-library-add-support-for-contains-extension-.html

        public static IQueryable<T> Where<T>(this IQueryable<T> query, FilterCriterionMultiSelect multipleSelectList, ReflectionQualifiedNames qualifiedNames)
        {

            if (multipleSelectList != null && multipleSelectList.IsFiltered)
            {
                string dbColumnName = qualifiedNames.GetQualifiedName(multipleSelectList.DbColumnName);

                switch (multipleSelectList.KeyType)
                {
                    case ListItem.KeyType.StringKey:
                        List<string> stringKeys = multipleSelectList.GetStringKeys();
                        query = query.Where("@0.Contains(outerIt." + dbColumnName + ") ", stringKeys);
                        break;

                    case ListItem.KeyType.IntKey:
                        List<int> intKeys = multipleSelectList.GetIntKeys();
                        query = query.Where("@0.Contains(outerIt." + dbColumnName + ") ", intKeys);
                        break;
                    default:
                        break;
                }
            }
            return query;

        }

        public static IQueryable<T> Where<T>(this IQueryable<T> query, FilterCriterionSingleSelect criterion, ReflectionQualifiedNames qualifiedNames)
        {


            if (criterion != null && criterion.IsFiltered)
            {
                string dbColumnName = qualifiedNames.GetQualifiedName(criterion.DbColumnName);

                switch (criterion.KeyType)
                {
                    case ListItem.KeyType.StringKey:
                        string stringKey = criterion.GetStringKey();
                        query = query.Where(dbColumnName + " = @0", stringKey);
                        break;

                    case ListItem.KeyType.IntKey:
                        int intKey = criterion.GetIntKey();
                        query = query.Where(dbColumnName + " == @0", intKey);
                        break;
                    default:
                        break;
                }
            }
            return query;

        }

        public static IQueryable<T> Where<T>(this IQueryable<T> query, FilterCriterionText criterion, ReflectionQualifiedNames qualifiedNames)
        {


            if (criterion != null && criterion.IsFiltered)
            {
                string dbColumnName = qualifiedNames.GetQualifiedName(criterion.DbColumnName);

                string stringKey = criterion.GetStringKey();
                switch (criterion.TextType)
                {
                    case FilterTextType.Equals:
                        query = query.Where(dbColumnName + " = @0", stringKey);
                        break;
                    case FilterTextType.Contains:
                        query = query.Where(dbColumnName + ".Contains(@0)", stringKey);
                        break;
                    case FilterTextType.StartsWith:
                        query = query.Where(dbColumnName + ".StartsWith(@0)", stringKey);
                        break;
                    case FilterTextType.EndsWith:
                        query = query.Where(dbColumnName + ".EndsWith(@0)", stringKey);
                        break;
                    default:
                        break;
                }
            }
            return query;

        }

        public static IQueryable<T> Where<T>(this IQueryable<T> query, FilterCriterionBoolean criterion, ReflectionQualifiedNames qualifiedNames)
        {

            if (criterion != null && criterion.IsFiltered)
            {
                string dbColumnName = qualifiedNames.GetQualifiedName(criterion.DbColumnName);
                query = query.Where(dbColumnName + " == @0", criterion.Value);
            }
            return query;

        }


    }
}
