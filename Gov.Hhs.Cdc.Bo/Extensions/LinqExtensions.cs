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
using System.Text;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace Gov.Hhs.Cdc.Bo
{
    public static class LinqExtensions
    {
        public static IQueryable<T> PageBy<T>(this IQueryable<T> query, Paging pageInfo)
        {
            if (pageInfo == null || !pageInfo.IsPaged || pageInfo.ReturnAllPages)
                return query;
            else
                return query
                    .Skip(pageInfo.RecordsToSkip)
                    .Take(pageInfo.NumberOfRecordsToSelect);
        }

        /// <summary>Orders the sequence by specific column and direction.</summary>
        /// <param name="query">The query.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="ascending">if set to true [ascending].</param>
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string sortColumn, string direction)
        {
            string methodName = string.Format("OrderBy{0}",
                direction.ToLower() == "asc" ? "" : "descending");

            ParameterExpression parameter = Expression.Parameter(query.ElementType, "p");

            MemberExpression memberAccess = null;
            foreach (var property in sortColumn.Split('.'))
                memberAccess = MemberExpression.Property
                   (memberAccess ?? (parameter as Expression), property);

            LambdaExpression orderByLambda = Expression.Lambda(memberAccess, parameter);

            MethodCallExpression result = Expression.Call(
                      typeof(Queryable),
                      methodName,
                      new[] { query.ElementType, memberAccess.Type },
                      query.Expression,
                      Expression.Quote(orderByLambda));

            return query.Provider.CreateQuery<T>(result);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, Sorting sorting, ReflectionQualifiedNames qualifiedNames)
        {
            if (sorting == null || !sorting.IsSorted || sorting.SortColumns == null)
                return query;

            for(int i = 0; i < sorting.SortColumns.Count; ++i)
            {
                SortColumn sortColumn = sorting.SortColumns[i];
            //foreach (SortColumn sortColumn in sorting.SortColumns)
            //{
                //OrderByType orderByType = i == 0 ? OrderByType.OrderBy : OrderByType.ThenBy;
                if( i == 0)
                    query = query.OrderBy<T>(qualifiedNames.GetQualifiedName(sortColumn.Column), sortColumn.SortOrder.ToString());
                else
                    query = query.ThenBy<T>(qualifiedNames.GetQualifiedName(sortColumn.Column), sortColumn.SortOrder.ToString());
                //query = query.OrderByWithType<T>(qualifiedNames.GetQualifiedName(sortColumn.Column), orderByType, sortColumn.SortOrder.ToString());
            }
            return query;
            //else
            //    return query.OrderBy<T>(sortInfo.SortColumn, sortInfo.SortOrder);
        }
    }
}
