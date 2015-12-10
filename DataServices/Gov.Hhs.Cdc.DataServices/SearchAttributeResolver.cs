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
using System.Reflection;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.DataServices
{
    public static class SearchAttributeResolver
    {
        #region InjectedProperties
        private static ISearchProviders _allSearchProviders;
        public static ISearchProviders AllSearchProviders { get { return _allSearchProviders; } }

        public static void Inject(ISearchProviders allSearchProviders)
        {
            _allSearchProviders = allSearchProviders;
        }
        #endregion

        //Keyed by applicationCode
        private static object FilterAttributesLock = new object();

        private static Dictionary<string, CriteriaForApplication> CachedCriteria = new Dictionary<string, CriteriaForApplication>();

        public static List<CriteriaForApplication> GetFilterCriteriaDtos()
        {
            lock (FilterAttributesLock)
            {
                List<CriteriaForApplication> criteriaForApplications = AllSearchProviders.List
                    .Select(a => GetAllCriteriaForApplication(a)).Where(a => a != null).ToList();
                return criteriaForApplications;
            }
        }

        private static CriteriaForApplication GetAllCriteriaForApplication(ISearchProvider searchProvider)
        {
            string searchProviderName = searchProvider.GetType().FullName;

            if (!CachedCriteria.ContainsKey(searchProviderName))
            {
                IEnumerable<Type> types = searchProvider.BusinessObjectAssemblies.Distinct().SelectMany(a => a.GetTypes());
                List<CriteriaDefinitionsForFilter> all = types.Select(t => GetFilterCriteriaDefinitionsForABusinessObject(t)).Where(t => t != null).ToList();

                if (all.Count() < 1)
                {
                    CachedCriteria.Add(searchProviderName, null);
                }
                else
                {
                    CachedCriteria.Add(searchProviderName, new CriteriaForApplication(searchProvider.ApplicationCode, all.ToList()));
                }
            }
            return CachedCriteria[searchProviderName];
        }

        public static CriteriaDefinitionsForFilter GetFilterCriteriaDefinitionsForABusinessObject(Type type)
        {
            List<FilterCriteriaDefinition> filters = (!type.IsDefined(typeof(FilteredDataSet), false)) ?
                    new List<FilterCriteriaDefinition>() :
                    GetFilterDefinitionsForType(type).ToList();
            return new CriteriaDefinitionsForFilter(GetBusinessObjectNameFromType(type), type.Namespace, filters);
        }

        private static string GetBusinessObjectNameFromType(MemberInfo type)
        {
            return type.Name.TrimEnd("Item").TrimEnd("Object");
        }

        private static List<FilterCriteriaDefinition> GetFilterDefinitionsForType(Type type)
        {
            List<FilterCriteriaDefinition> filtersFromClass = type.GetCustomAttributes(typeof(FilterSelection), false)
                .Select(f => GetFilterCriteriaDefinition((FilterSelection)f, GetBusinessObjectNameFromType(type), null)).ToList();

            List<FilterCriteriaDefinition> filtersFromProperties =
                (from p in type.GetProperties()
                 from f in p.GetCustomAttributes(typeof(FilterSelection), false)
                 select GetFilterCriteriaDefinition((FilterSelection)f, GetBusinessObjectNameFromType(type), p)).ToList();
            List<Type> inheritedTypes = filtersFromProperties.Where(p => p.InheritsType != null).Select(p => p.InheritsType).ToList();
            List<FilterCriteriaDefinition> filtersFromInheritedTypes = inheritedTypes.SelectMany(t => GetFilterDefinitionsForType(t)).ToList();
            return filtersFromClass.Concat(filtersFromProperties).Concat(filtersFromInheritedTypes).OrderBy(f => f.Code).ToList();
        }

        private static ListItem.KeyType GetListKeyType(PropertyInfo property)
        {
            if (property == null)
            {
                return ListItem.KeyType.StringKey;
            }

            Type propertyType = property.PropertyType;
            if (propertyType == typeof(string) || propertyType == typeof(String))
            {
                return ListItem.KeyType.StringKey;
            }
            else
            {
                return ListItem.KeyType.IntKey;
            }
        }

        private static bool RequiresListKeyType(FilterCriterionType type)
        {
            return type == FilterCriterionType.DropDownList
                || type == FilterCriterionType.HierMultiSelect
                || type == FilterCriterionType.MultiSelect
                || type == FilterCriterionType.SingleSelect;
        }

        private static FilterCriteriaDefinition GetFilterCriteriaDefinition(FilterSelection selection, string filterCode, PropertyInfo property)
        {
            if (string.IsNullOrEmpty(selection.Code) && property == null)
            {
                throw new ApplicationException("FilterSelection for class " + filterCode + " must contain the 'Code' parameter");
            }
            if (selection.ListKeyType == ListItem.KeyType.Null && property == null && RequiresListKeyType(selection.CriterionType))
            {
                throw new ApplicationException("FilterSelection for class " + filterCode + ", Code " + selection.Code + " must contain the 'ListKeyType' parameter");
            }
            FilterCriteriaDefinition filter = new FilterCriteriaDefinition()
            {
                InheritsType = selection.Inherit == false ? null : property.PropertyType,
                InheritsTypeName = selection.Inherit == false ? null : GetBusinessObjectNameFromType(property.PropertyType),
                FilterCode = filterCode,
                Code = string.IsNullOrEmpty(selection.Code) ? property.Name : selection.Code,

                DisplayOrder = selection.DisplayOrder,
                DisplayName = selection.DisplayName,
                DisplayNote = selection.DisplayNote,

                Type = selection.CriterionType.ToString(),
                TextType = selection.TextType.ToString(),
                ListKeyType = (selection.ListKeyType == ListItem.KeyType.Null ? GetListKeyType(property) : selection.ListKeyType).ToString(),

                DbColumnName = property == null ? null : property.Name,
                IsRequired = selection.IsRequired,

                //ToDo: Support groups
                GroupOrder = selection.DisplayOrder,
                GroupCode = selection.GroupCode,

                AllowDateInFuture = selection.AllowDateInFuture,
                AllowDateInPast = selection.AllowDateInPast,

                IsIncludedByDefault = selection.IsIncludedByDefault
            };
            return filter;
        }

    }
}
