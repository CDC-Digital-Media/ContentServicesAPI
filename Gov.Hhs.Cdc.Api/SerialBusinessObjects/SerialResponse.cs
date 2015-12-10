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

using System.Web;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    [KnownType(typeof(List<SerialMediaType>))]
    [KnownType(typeof(List<SerialLocationItem>))]
    [KnownType(typeof(List<SerialMediaV1>))]
    [KnownType(typeof(List<SerialMediaV2>))]
    [KnownType(typeof(List<SerialMediaAdmin>))]
    [KnownType(typeof(List<SerialValueSetItem>))]
    [KnownType(typeof(List<SerialValueItem>))]
    [KnownType(typeof(List<SerialValueItemAdmin>))]
    [KnownType(typeof(SerialSyndicationV1))]
    [KnownType(typeof(SerialSyndicationV2))]
    [KnownType(typeof(SerialMediaValidation))]
    [KnownType(typeof(List<SerialVocab>))]
    [KnownType(typeof(List<SerialLanguageV1>))]
    [KnownType(typeof(List<SerialLanguageV2>))]
    [KnownType(typeof(List<SerialSource>))]
    [KnownType(typeof(List<SerialMediaStatus>))]
    [KnownType(typeof(List<SerialUserOrg>))]
    [KnownType(typeof(List<SerialUserOrgTypeItem>))]
    [KnownType(typeof(List<SerialUserOrgItem>))]
    [KnownType(typeof(List<SerialBusinessOrgItem>))]
    [KnownType(typeof(List<SerialBusinessOrgTypeItem>))]
    [KnownType(typeof(SerialSyndicationList))]
    [KnownType(typeof(SerialShortSyndicationList))]
    [KnownType(typeof(List<SerialShortSyndicationList>))]
    [KnownType(typeof(SerialSyndicationListMedia))]
    [KnownType(typeof(SerialLoginResults))]
    [KnownType(typeof(SerialCardView))]
    [KnownType(typeof(SerialAdminUser))]
    [KnownType(typeof(SerialSchedulerEntry))]
    [KnownType(typeof(SerialSchedulerTask))]
    [KnownType(typeof(SerialSchedulerTaskHistory))]
    [KnownType(typeof(SerialSchedulerUtility))]
    [KnownType(typeof(SerialSchedulerRunHistory))]
    [KnownType(typeof(SerialSchedulerType))]
    [KnownType(typeof(SerialSchedulerIntervalType))]
    [KnownType(typeof(List<SerialFeedFormat>))]
    [KnownType(typeof(List<SerialFeedExport>))]
    [DataContract(Name = "response")]
    public sealed class SerialResponse
    {
        [DataMember(Name = "meta", Order = 1, EmitDefaultValue = true)]
        public SerialMeta meta { get; set; }

        [DataMember(Name = "results", Order = 2, EmitDefaultValue = true)]
        public object results { get; set; }

        [XmlIgnore]
        [ScriptIgnore]
        [DataMember(EmitDefaultValue = false)]
        public DataSetResult dataset { get; set; }

        [XmlIgnore]
        [ScriptIgnore]
        [DataMember(EmitDefaultValue = false)]
        public QueryParams param { get; set; }

        [XmlIgnore]
        [ScriptIgnore]
        [DataMember(EmitDefaultValue = false)]
        public IEnumerable<MediaObject> mediaObjects { get; set; }

        public SerialResponse()
        {
            meta = new SerialMeta();
            results = new object();
            param = null;
        }

        public SerialResponse(object newResults)
        {
            meta = new SerialMeta();
            results = newResults;
            dataset = null;
            param = null;
        }

        public SerialResponse(object newResults, QueryParams param)
        {
            meta = new SerialMeta();
            results = newResults;
            dataset = null;
            this.param = param;
        }

        public SerialResponse(SerialMeta newMeta, object newResults)
        {
            meta = newMeta;
            results = newResults;
            dataset = null;
            param = null;
        }
    }


    public sealed class SerialResponseWithType<T>
    {
        [DataMember(Name = "meta", Order = 1, EmitDefaultValue = true)]
        public SerialMeta meta { get; set; }

        [DataMember(Name = "results", Order = 2, EmitDefaultValue = true)]
        public T results { get; set; }

        [XmlIgnore]
        [ScriptIgnore]
        [DataMember(EmitDefaultValue = false)]
        public DataSetResult dataset { get; set; }

        [XmlIgnore]
        [ScriptIgnore]
        [DataMember(EmitDefaultValue = false)]
        public IEnumerable<MediaObject> mediaObjects { get; set; }

    }
}
