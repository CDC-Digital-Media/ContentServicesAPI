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
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Gov.Hhs.Cdc.CdcRegistrationProvider
{
    public static class CredentialManager
    {
        public static string GenerateApiKeyTokenSalt()
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[32];
                rng.GetBytes(data);

                //make alphanumeric
                return Regex.Replace(Convert.ToBase64String(data), @"\W|_", "", RegexOptions.Singleline);
            }
        }

        public static Int64 TokenExpirationInSeconds(Int64 secondsToAdd = 0)
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow.AddSeconds(secondsToAdd) - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        //public static DateTime TokenExpirationUtcDateTime(Int64 secondsToAdd = 86400) //86400 = 1 day
        //{
        //    return DateTime.UtcNow.AddSeconds(secondsToAdd);
        //}

        public static bool CompareHash(string password, string salt, string passwordHashFromDatabase, int? passwordFormat)
        {
            if (salt.Length != 24)
            {
                return CreateHash(password, salt) == passwordHashFromDatabase;
            }
            else    //old 2.6 hash       
            {
                if (passwordFormat != 2)
                {
                    return LegacyMembership.ValidateUserHash(password, salt) == passwordHashFromDatabase;
                }
                else
                {
                    return new LegacyMembership().GetDecryptedPassword(passwordHashFromDatabase) == password;
                }
            }                            
        }

        public static string CreateHash(string password, string salt)
        {
            string combined = password + salt;

            return HashString(combined);
        }

        private static string HashString(string toHash)
        {
            using (SHA512CryptoServiceProvider sha = new SHA512CryptoServiceProvider())
            {
                byte[] data = Encoding.UTF8.GetBytes(toHash);
                byte[] hashed = sha.ComputeHash(data);

                return Convert.ToBase64String(hashed);
            }
        }

    }
}
