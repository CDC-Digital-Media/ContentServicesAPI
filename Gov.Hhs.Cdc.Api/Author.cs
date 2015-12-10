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
using System.Security.Cryptography;
using System.Runtime.Serialization;

namespace Gov.Hhs.Cdc.Api
{
    public sealed class Author
    {
        private HMAC _author;
        private byte[] onePublicKey;

        public Author() 
        {         
        }

        private string[] Keys = { "PublicKey", "PrivateKey" };

        public KeyHolder GenerateKeyPair()
        {
            KeyHolder keyObj = new KeyHolder();
            for (int i = 0; i < Keys.Length; i++)
            {
                // key
                onePublicKey = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
                _author = new HMACSHA256(onePublicKey);

                byte[] toHash = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());

                // Compute the signature
                byte[] hashBytes = _author.ComputeHash(toHash);

                // Convert to Base64
                string pKey = Convert.ToBase64String(hashBytes);
                keyObj.GetType().GetProperty(Keys[i]).SetValue(keyObj, pKey, null);
            }

            return keyObj;
        }
        
        [DataContract]
        public class KeyHolder
        {
            [DataMember(Name = "PublicKey", Order = 1)]
            public string PublicKey { get; set; }

            [DataMember(Name = "PrivateKey", Order = 2)]
            public string PrivateKey { get; set; }
        }
    }

    
}
