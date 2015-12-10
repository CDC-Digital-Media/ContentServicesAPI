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

namespace Gov.Hhs.Cdc.Email
{
    public class EmailGeneral : EmailBase
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strTo"></param>
        //public EmailGeneral(string strTo)
        //    : base(strTo)
        //{
        //}

        public EmailGeneral(string strFrom, string strTo)
            : base(strFrom, strTo)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strTo"></param>
        /// <param name="strSubject"></param>
        /// <param name="strBody"></param>
        //public EmailGeneral(string strTo, string strSubject, string strBody)
        //    : base(strTo)
        //{
        //    base.Subject = strSubject;
        //    base.Body = strBody;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFrom"></param>
        /// <param name="strTo"></param>
        /// <param name="strCC"></param>
        /// <param name="strSubject"></param>
        /// <param name="strBody"></param>
        public EmailGeneral(string strFrom, string strTo, string strCC, string strSubject, string strBody)
            : base(strFrom, strTo)
        {
            base.CC.Add(strCC);
            base.Subject = strSubject;
            base.Body = strBody;
        }

    }
}
