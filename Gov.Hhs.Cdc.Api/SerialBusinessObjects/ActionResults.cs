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
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.Bo;

namespace Gov.Hhs.Cdc.Api
{
    public class ActionResults
    {
        public object Results { get; set; }
        public ValidationMessages ValidationMessages { get; set; }
        public NameValueCollection HeaderValues { get; set; }

        public SerialMeta Meta { get; set; }

        public ActionResults()
        {
        }

        public ActionResults(object results, ValidationMessages validationMessages, NameValueCollection headerValues = null)
        {
            Results = results;
            ValidationMessages = validationMessages ?? new ValidationMessages();
            HeaderValues = headerValues ?? new NameValueCollection();
        }

        public ActionResults(ActionResults actionResults)
            : this(actionResults.Results, actionResults.ValidationMessages, actionResults.HeaderValues)
        {
        }

        public ActionResults(string jsonStream)
        {
            if (jsonStream == string.Empty) throw new ApplicationException("no results from API");

            SerialResponse resultObj = new JavaScriptSerializer().Deserialize<SerialResponse>(jsonStream);
            Results = resultObj.results;
            ValidationMessages = resultObj.meta.GetUnserializedMessages();
        }
    }

    public class ActionResultsWithType<T> : ActionResults
    {
        public T ResultObject { get { return (T)Results; } }
        public ActionResultsWithType(SerialResponseWithType<T> resultObj)
            : base(resultObj.results, resultObj.meta.GetUnserializedMessages())
        {
        }

        public ActionResultsWithType(ActionResults actionResults)
            : base(actionResults)
        {
        }

        public ActionResultsWithType(string jsonStream)
        {
            if (jsonStream == string.Empty) throw new ApplicationException("no results from API");
            var jss = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            jss.RegisterConverters(new JavaScriptConverter[] { new KeyValuePairJsonConverter() });
            SerialResponseWithType<T> resultObj = jss.Deserialize<SerialResponseWithType<T>>(jsonStream);
            Results = resultObj.results;
            Meta = resultObj.meta;
            ValidationMessages = resultObj.meta.GetUnserializedMessages();
        }
    }
}
