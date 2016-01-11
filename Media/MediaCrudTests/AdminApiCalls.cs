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
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Api.Admin;
using Newtonsoft.Json;


public static class AdminApiCalls
{
    public static SerialMediaAdmin SingleMedia(string mediaId)
    {
        return SingleMedia(int.Parse(mediaId));
    }

    public static SerialMediaAdmin SingleMedia(int mediaId)
    {
        var adminService = new AdminApiServiceFactory();
        var sampleMedias = TestApiUtility.GetResource<SerialMediaAdmin>(adminService.CreateTestUrl("media", mediaId));

        if (sampleMedias == null)
        {
            throw new InvalidOperationException("Media not found for id " + mediaId.ToString());
        }
        if (sampleMedias.Count != 1)
        {
            throw new InvalidOperationException("Media count found for id " + mediaId.ToString() + " was " + sampleMedias.Count.ToString());
        }
        return sampleMedias[0];
    }

    public static SerialMediaAdmin SingleCollection()
    {
        return Collections()[0];
    }

    public static List<SerialMediaAdmin> Collections()
    {
        var adminService = new AdminApiServiceFactory();
        List<SerialMediaAdmin> collection;
        TestApiUtility.ApiGet<SerialMediaAdmin>(adminService,
            adminService.CreateTestUrl("media?mediatype=collection"), out collection);
        if (collection == null || collection.Count() == 0)
        {
            throw new InvalidOperationException("No collections found.");
        }
        //return collection;
        return collection.Where(c => c.mediaId != "362221").ToList();
        //collection with non-utf8 topic, Agua, conservación del (195)
    }

    public static List<SerialAdminLog> Log()
    {
        var service = new AdminApiServiceFactory();
        List<SerialAdminLog> logs;
        var messages = TestApiUtility.ApiGet<SerialAdminLog>(service, service.CreateTestUrl("logs"), out logs);
        return logs;
    }


    internal static object UpdateAdminMedia(SerialMediaAdmin media, string adminUser)
    {
        var url = string.Format("{0}://{1}/adminapi/v1/resources/media/{2}", TestUrl.Protocol, TestUrl.AdminApiServer, media.id);
        Console.WriteLine(url);
        var ser = JsonConvert.SerializeObject(media);
        var results = TestApiUtility.CallAPIPut(url, ser, adminUser);
        return new ActionResultsWithType<List<SerialMediaAdmin>>(results);
    }
}

