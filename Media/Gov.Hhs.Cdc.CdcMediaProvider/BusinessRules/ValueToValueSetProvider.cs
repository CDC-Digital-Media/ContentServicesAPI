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
using Gov.Hhs.Cdc.DataServices;
using Gov.Hhs.Cdc.CdcMediaProvider.Dal;
using Gov.Hhs.Cdc.DataSource;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public class ValueToValueSetProvider : MediaMgrBase
    {
        public static ValueToValueSetObject Get(int valueId, string valueLang, int valueSetId, string valueSetLang)
        {
            return Get(valueId, valueLang, valueSetId, valueSetLang, forUpdate: false);
        }

        public static ValueToValueSetObject Get(int valueId, string valueLang, int valueSetId, string valueSetLang, bool forUpdate)
        {
            using (IDataServicesObjectContext media = ObjectContextFactory.Create())
            {
                return ValueToValueSetCtl.Get((MediaObjectContext)media, forUpdate)
                    .Where(m =>
                        m.ValueId == valueId && m.ValueLanguageCode == valueLang &&
                        m.ValueSetId == valueSetId && m.ValueSetLanguageCode == valueSetLang
                    ).FirstOrDefault();
            }
        }
        public static ValidationMessages Save(IList<ValueToValueSetObject> objects)
        {
            return MediaDataManager.Save(new ValueToValueSetUpdateMgr(objects));
        }

        public static ValidationMessages Save(ValueToValueSetObject theObject)
        {
            return MediaDataManager.Save(new ValueToValueSetUpdateMgr(theObject));
        }

        public static ValidationMessages Delete(IList<ValueToValueSetObject> objects)
        {
            return MediaDataManager.Delete(new ValueToValueSetUpdateMgr(objects));
        }

        public static ValidationMessages Delete(ValueToValueSetObject theObject)
        {
            return MediaDataManager.Delete(new ValueToValueSetUpdateMgr(theObject));
        }
    }
}
