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
using System.Configuration;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.DataProvider
{
    [FilteredDataSet]
    public class ProxyCacheObject : DataSourceBusinessObject, IValidationObject
    {
        public const string DefaultExpireTimeSpanString = "12:00:00";
        public static TimeSpan DefaultExpireTimeSpan = TimeSpan.Parse(DefaultExpireTimeSpanString);
        public const string DefaultData = "{\"data\":\"The data requested is currently unavailable, please check back later\"}";

        public enum DataStatus { Error, Fresh, Expired, Stale };

        public int Id { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.Text, TextType = FilterTextType.Equals)]
        [FilterSelection(Code = "UrlContains", CriterionType = FilterCriterionType.Text, TextType = FilterTextType.Contains)]
        public string Url { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.MultiSelect, TextType = FilterTextType.Equals)]
        public string DatasetId { get; set; }

        public string Data { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }

        [FilterSelection(Code = "ExpirationDate", CriterionType = FilterCriterionType.DateRange)]
        [FilterSelection(Code = "IsExpired", CriterionType = FilterCriterionType.DateRange)]
        public DateTime ExpirationDateTime { get; set; } 
  
        public String ExpirationInterval { get; set; }

        [FilterSelection(CriterionType = FilterCriterionType.Boolean)]
        public bool NeedsRefresh { get; set; }

        public int Failures { get; set; }

        public TimeSpan GetExpirationIntervalTimeSpan() 
        {
            try
            {
                return TimeSpan.ParseExact(ExpirationInterval, "d'.'hh':'mm':'ss", null);
            }
            catch (Exception)
            {
                try
                {
                    ExpirationInterval = ConfigurationManager.AppSettings["ProxyCacheDefaultExpireTimeSpan"];
                    return TimeSpan.ParseExact(ExpirationInterval, "d'.'hh':'mm':'ss", null);
                }
                catch (Exception)
                {
                    ExpirationInterval = DefaultExpireTimeSpanString;
                    return DefaultExpireTimeSpan;
                }
            }
        }

        public void SetExpirationIntervalTimeSpan(TimeSpan expirationIntervalTimeSpan)
        {
            ExpirationInterval = expirationIntervalTimeSpan.ToString();
        }

        public DataStatus GetDataStatus()
        {            
            DataStatus retVal = DataStatus.Fresh;

            if (DateTime.Compare(DateTime.UtcNow, ExpirationDateTime) > 0)
            {
                if (Failures >= DataUpdater.CONSECUTIVE_FAILURE_LIMIT)
                {
                    retVal = DataStatus.Stale;
                }
                else
                {
                    retVal = DataStatus.Expired;
                }
            }

            return retVal;
        }
    }
}
