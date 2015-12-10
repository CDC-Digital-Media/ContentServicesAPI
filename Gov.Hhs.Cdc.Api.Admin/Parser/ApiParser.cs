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
using System.Web;

using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.Api.Admin
{
    public class ApiParser : ParserBase
    {
        public ApiParser(HttpContext context)
            : base(context)    //Let public facing default to false, isPublicFacing: false)
        {
            // Parse the queryString            
            IsPublicFacing = false;
            Parse();
        }

        public override void Parse()
        {
            // Parse the queryString            
            base.Parse();

            if (ParamDictionary == null)
                ParamDictionary = RestHelper.CreateDictionary(base._queryString);

            if (ParamDictionary.Count > 0)
            {
                foreach (KeyValuePair<string, string> entry in ParamDictionary)
                {
                    switch (entry.Key.ToLower())
                    {
                        case Param.VALUESET_ID:
                            string[] valuesetid = entry.Value.Split(',');
                            Criteria.Add("ValueSet", valuesetid.ToList<string>().Select(a => a.Trim()).ToList<string>());

                            //string lang = GetValueSetLanguage("Language");
                            //Criteria.Add("ValueSetName", valuesetid.ToList<string>().Select(a => a.Trim() + "|" + lang).ToList<string>());
                            break;
                        default:
                            break;
                    }
                }
            }
        }

    }
}
