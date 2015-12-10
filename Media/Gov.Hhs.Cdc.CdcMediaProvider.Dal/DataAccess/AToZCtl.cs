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

using Gov.Hhs.Cdc.MediaProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gov.Hhs.Cdc.CdcMediaProvider.Dal
{
    public class AToZCtl
    {
        public static IQueryable<AToZObject> Get(MediaObjectContext media, bool forUpdate = false)
        {
            IQueryable<AToZObject> AToZs = from a in media.MediaDbEntities.AZLists
                                                 select new AToZObject()
                                                 {
                                                     Letter = a.Letter,
                                                     ValueSetName = a.ValueSetName,
                                                     LanguageCode = a.LanguageCode,
                                                     MediaCount = a.MediaCount,
                                                     VocabCount = a.VocabCount,
                                                     DisplayOrdinal = a.SortOrder,
                                                     DbObject = forUpdate ? a : null
                                                 };
            return AToZs;
        }

    }
}
