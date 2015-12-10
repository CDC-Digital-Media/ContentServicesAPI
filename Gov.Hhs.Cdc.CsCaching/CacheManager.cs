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
using System.Web;

namespace Gov.Hhs.Cdc.CsCaching
{
    public static class CacheManager
    {
        public static IList<string> CachedKeys()
        {
            var keys = new List<string>();
            var enumerator = HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key as string);
                Console.WriteLine(enumerator.Key as string);
                //object value = enumerator.Value;
            }
            return keys;
        }

        public static void Cache(string key, object value)
        {
            HttpRuntime.Cache.Insert(key, value);
        }

        public static void Cache(string key, object value, DateTime expiration)
        {
            HttpRuntime.Cache.Insert(key, value, null, expiration, System.Web.Caching.Cache.NoSlidingExpiration);
        }

        public static T CachedValue<T>(string key)
        {
            return (T)HttpRuntime.Cache[key];
        }

        public static void ClearAll()
        {
            var enumerator = HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                HttpRuntime.Cache.Remove(enumerator.Key as string);
            }
        }

        public static void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }
    }
}
