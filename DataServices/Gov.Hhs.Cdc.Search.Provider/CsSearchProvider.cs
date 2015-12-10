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
using Gov.Hhs.Cdc.DataSource;
using Gov.Hhs.Cdc.Search.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;
//using Gov.Hhs.Cdc.DataSource.Bll;

namespace Gov.Hhs.Cdc.Search.Provider
{
    public abstract class CsSearchProvider : ISearchProvider
    {
        public string Code { get { return "CsSearchProvider"; } }
        public string Name { get { return "Content Services Search Provider"; } }
        public bool CanPage { get { return true; }}
        public bool CanSort { get { return true; }}
        public bool SupportsEfficientIntermediateRequests { get { return true; }}
        public bool CanIndexByLocation { get { return false; }}
        public bool SupportsPartialResults { get { return false; }}
        public List<string> SupportsApplications { get { return new List<string>{};}}

        //public DataSetResult Get(SearchParameters searchParameters, ResultSetParms resultSetParms)
        //{

        //    //SearchParameters filterParms = new SearchParameters()
        //    //{

        //    //    ApplicationCode = searchParameters.ApplicationCode,
        //    //    DataSetCode = searchParameters.DataSetCode,
        //    //    //FilterCode = searchParameters.FilterCode,
        //    //    //ControlCode = searchParameters.ControlCode,
        //    //    EnvironmentCode = DataSourceConfig.DefaultEnvironment,
        //    //    Paging = searchParameters.Paging,
        //    //    //IsPaged = searchParameters.IsPaged,
        //    //    //PageSize = searchParameters.PageSize,
        //    //    //PageNumber = searchParameters.PageNumber,
        //    //    //ReturnMultiplePages = searchParameters.ReturnMultiplePages,
        //    //    //ReturnAllPages = searchParameters.ReturnAllPages,
        //    //    //LastPageNumber = searchParameters.LastPageNumber,
        //    //    Sorting = searchParameters.Sorting,
        //    //    FilterCriteria = searchParameters.FilterCriteria
        //    //};

        //    ValidationMessages validations = null;
        //    DataSetResult result = GetData(searchParameters, out validations, resultSetParms);

        //    if (validations.Errors().Any())
        //        throw new ApplicationException("DataSet Validation Errors: " + validations.ToString());

        //    return result;        
        //}

        //public DataSetResult GetData(SearchParameters searchParameters, out ValidationMessages validations, ResultSetParms resultSetParms)
        //{
        //    validations = null;
        //    IApplicationSearchProvider dataManagerFactory = Create(searchParameters.ApplicationCode);
        //    return dataManagerFactory.GetData(searchParameters, resultSetParms, new FilterCriteriaMgr());
        //}

        //public static Dictionary<string, IApplicationSearchProvider> ApplicationSearchProviders;
        //public static IApplicationSearchProvider Create(string applicationCode)
        //{
        //    try
        //    {
        //        ApplicationSearchProviders.Add("media", Gov.Hhs.Cdc.DataSource.Media.MediaSearchProvider());
        //        ApplicationSearchProviders.Add("user", Gov.Hhs.Cdc.DataSource.User.UserSearchProvider());
        //        //Load using MEF
        //        //http://stackoverflow.com/questions/1087794/c-sharp-load-a-dll-file-and-access-methods-from-class-within
        //        //Assembly assembly = AssemblyCache.GetAssembly(application.DataSourceManagerDll);
        //        //IDataManager dll = (IDataManager)assembly.CreateInstance(application.DataSourceManagerClass);

        //        string lowerApplicationCode = applicationCode.ToLower();
        //        if( ApplicationSearchProviders.ContainsKey(lowerApplicationCode) )
        //            return ApplicationSearchProviders[lowerApplicationCode];
        //        else
        //            throw new ApplicationException("The DataSourceManagerClass for application (" + applicationCode + ") has not been defined in the DataSourceFactory.Create() method");
        //        //switch (applicationCode.ToLower())
        //        //{
                        
        //        //    case "media":
        //        //        return new Gov.Hhs.Cdc.DataSource.Media.MediaSearchProvider();
        //        //    default:
        //        //        throw new ApplicationException("The DataSourceManagerClass for application (" + applicationCode + ") has not been defined in the DataSourceFactory.Create() method");

        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }

        //}

        //public DataSetResult GetDataLists(SearchParameters searchParameters)
        //{
        //    //SearchParameters filterParms = new SearchParameters()
        //    //{

        //    //    ApplicationCode = searchParameters.ApplicationCode,
        //    //    DataSetCode = searchParameters.DataSetCode,
        //    //    //FilterCode = searchParameters.FilterCode,
        //    //    //ControlCode = searchParameters.ControlCode,
        //    //    EnvironmentCode = DataSourceConfig.DefaultEnvironment,
        //    //    FilterCriteria = searchParameters.FilterCriteria
        //    //};

        //    ValidationMessages validations = null;
        //    DataSetResult result = GetData(searchParameters, out validations, new ResultSetParms());

        //    if (validations.Errors().Any())
        //        throw new ApplicationException("DataSet Validation Errors: " + validations.ToString());

        //    return result;
        //}


        public DataSetResult Get(SearchParameters searchParameters, ResultSetParms resultSetParms)
        {
            throw new NotImplementedException();
        }
    }
}
