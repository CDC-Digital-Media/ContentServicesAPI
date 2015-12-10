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
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CsCaching;

namespace Gov.Hhs.Cdc.Search.Controller
{

    public class SearchCacheMgr : ISearchCacheMgr
    {
        #region ClassProperties
        public TimeSpan? ExpirationTime { get; set; }
        private enum CacheObjectType { Parm, Result, IntResult }; 
        #endregion


        public SearchCacheMgr()
        {
        }

        public void SetExpirationTimeInSeconds(int seconds)
        {
            ExpirationTime = seconds > 0 ? new TimeSpan(0, 0, seconds) : (TimeSpan?) null;

        }

        public void Insert(ResultSetParameters resultSetParms)
        {
            if (ExpirationTime == null) return;
            var key = GetCacheKey(CacheObjectType.Parm, resultSetParms.ResultSetId);
            CacheManager.Cache(key, resultSetParms);
                //CacheProvider.Insert(GetCacheKey(CacheObjectType.Parm, resultSetParms.ResultSetId), 
                //    resultSetParms, DateTime.Now.Add((TimeSpan) ExpirationTime));
        }

        public void Insert(DataSetResult value, Guid resultSetId)
        {
            if (ExpirationTime == null) return;
            var key = GetCacheKey(CacheObjectType.Result, resultSetId);
            CacheManager.Cache(key, resultSetId);
            //CacheProvider.Insert(GetCacheKey(CacheObjectType.Result, resultSetId),
            //            value, DateTime.Now.Add((TimeSpan)ExpirationTime));
        }

        public void Insert(DataSetResult value, Guid resultSetId, string searchProviderCode)
        {
            if (ExpirationTime == null) return;
            var key = GetCacheKey(CacheObjectType.IntResult, resultSetId, searchProviderCode);
            CacheManager.Cache(key, value);
            //CacheProvider.Insert(GetCacheKey(CacheObjectType.IntResult, resultSetId, searchProviderCode),
            //        value, DateTime.Now.Add((TimeSpan)ExpirationTime));
        }

        //public bool Contains(Guid resultSetId, string searchProviderCode)
        //{
        //    return CacheProvider.Contains(GetCacheKey(CacheObjectType.IntResult, resultSetId, searchProviderCode));
        //}

        public ResultSetParameters GetCachedParameters(Guid resultSetId)
        {
            var key = GetCacheKey(CacheObjectType.Parm, resultSetId);
            return CacheManager.CachedValue<ResultSetParameters>(key);
           // return (ResultSetParameters)CacheProvider.Get(GetCacheKey(CacheObjectType.Parm, resultSetId));
        }

        public DataSetResult GetDataSetResult(Guid resultSetId)
        {
            var key = GetCacheKey(CacheObjectType.Result, resultSetId);
            return CacheManager.CachedValue<DataSetResult>(key);
            //return (DataSetResult)CacheProvider.Get(GetCacheKey(CacheObjectType.Result, resultSetId));
        }

        public DataSetResult GetDataSetResult(Guid resultSetId, string searchProviderCode)
        {
            var key = GetCacheKey(CacheObjectType.IntResult, resultSetId, searchProviderCode);
            return CacheManager.CachedValue<DataSetResult>(key);
            //return (DataSetResult)CacheProvider.Get(GetCacheKey(CacheObjectType.IntResult, resultSetId, searchProviderCode));
        }

        private static string GetCacheKey(CacheObjectType objectType, Guid resultSetId)
        {
            return "SearchCtl|" + objectType.ToString() + "|" + resultSetId;
        }

        private static string GetCacheKey(CacheObjectType objectType, Guid resultSetId, string searchProviderCode)
        {
            return "SearchCtl|" + objectType.ToString() + "|" + resultSetId + "|" + searchProviderCode;
        }
    }
}
