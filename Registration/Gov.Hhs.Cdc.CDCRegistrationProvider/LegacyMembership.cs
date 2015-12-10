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
using System.Web.Security;
using System.Security.Cryptography;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{

    public class LegacyMembership : SqlMembershipProvider
    {
        public string GetDecryptedPassword(string encryptedPwd)
        {
            try
            {
                byte[] encodedPassword = Convert.FromBase64String(encryptedPwd);
                byte[] bytes = this.DecryptPassword(encodedPassword);
                if (bytes == null)
                    return null;

                return Encoding.Unicode.GetString(bytes, 0x10, bytes.Length - 0x10);
            }
            catch (Exception ex)
            {                
                throw ex;
            }            
        }

        public static string ValidateUserHash(string pass, string salt)
        {
            byte[] bIn = Encoding.Unicode.GetBytes(pass);
            byte[] bSalt = Convert.FromBase64String(salt);
            byte[] bAll = new byte[bSalt.Length + bIn.Length];
            byte[] bRet = null;

            Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
            Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);

            HashAlgorithm s = HashAlgorithm.Create("SHA1");
            bRet = s.ComputeHash(bAll);

            return Convert.ToBase64String(bRet);
        }
    }
}
