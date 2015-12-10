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

namespace Gov.Hhs.Cdc.MediaProvider
{
    [Serializable]
    public abstract class ExtractionCriteria
    {
        public string ExtractionName { get; set; }
        public bool IsDefault { get; set; }

        public abstract ExtractionCriteria Merge(ExtractionCriteria subordinateCriteria);

        protected static ExtractionPath Merge(ExtractionPath primary, ExtractionPath secondary)
        {
            return primary != null ? primary : secondary;
        }

        protected static bool? Merge(bool? primary, bool? secondary)
        {
            return primary != null ? primary : secondary;
        }

        protected static string Merge(string primary, string secondary)
        {
            return (!string.IsNullOrEmpty(primary)) ? primary : secondary;
        }
    }
}
