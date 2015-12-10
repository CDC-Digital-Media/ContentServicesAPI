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

namespace Gov.Hhs.Cdc.CdcMediaProvider
{
    public static class PersistentUrlGenerator
    {
        private static char[] ValidUrlCharacters = new char[] { '0','1','2','3','4','5','6','7','8','9',
            'B','C','D','F','G','H','J','K','L','M','N', 'P','Q','R','S','T','V','W','X','Z'};
        private static int NumberOfDigits = 5;
        private static int MaxValueForRandomNumber = (int)Math.Pow(ValidUrlCharacters.Length, NumberOfDigits) - 1;

        public static string GenerateNewPersistentUrl()
        {
            Random r = new Random();
            int randomNumber = r.Next(MaxValueForRandomNumber);
            string persistentUrl = IntToString(randomNumber, ValidUrlCharacters);
            persistentUrl = persistentUrl.PadLeft(NumberOfDigits, ValidUrlCharacters[0]);

            return persistentUrl;
        }

        private static string IntToString(int value, char[] baseChars)
        {
            string result = string.Empty;
            int targetBase = baseChars.Length;

            do
            {
                result = baseChars[value % targetBase] + result;
                value = value / targetBase;
            }
            while (value > 0);

            return result;
        }

    }
}
