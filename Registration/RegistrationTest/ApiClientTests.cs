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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.RegistrationProvider;
using Gov.Hhs.Cdc.Bo;

namespace RegistrationTest
{
    [TestClass]
    public class ApiClientTests
    {
        private string Test_Api_Key = "cbaqU9JEfJVXY2CBBwVqiTZ4CPk77SPEcoEDpLBe5M";
        private string Service_User_Guid = "f13dde63-3db5-411d-8b3b-e21954a03a72";
        private string ApiType = "Public";
        private string Name = "CreatedFromTest";

        private SerialApiClientCredential apiClientCredentials;
        private SerialApiClient apiClientObj;

        public ApiClientTests()
        {
            DAL.Initialize();
        }

        [TestMethod]
        public void AddApiClient()
        {
            apiClientObj = CreateSerialApiClientIn();

            //add ApiClient
            ActionResults actionResults = RegistrationHandler.SaveApiClient(apiClientObj, Test_Api_Key);
            Assert.IsFalse(actionResults.ValidationMessages.Errors().Any());

            apiClientCredentials = (SerialApiClientCredential)actionResults.Results;

            //update ApiClient token
            //string oldToken = apiClientCredentials.token;

            //actionResults = RegistrationHandler.UpdateApiClientToken(apiClientCredentials);
            //Assert.IsFalse(actionResults.ValidationMessages.Errors().Any());
            //Assert.IsFalse(oldToken == apiClientCredentials.token);

            //update ApiClient apikey
            string old_apikey = apiClientCredentials.apiKey;

            actionResults = RegistrationHandler.UpdateApiClientApiKey(apiClientCredentials);
            Assert.IsFalse(actionResults.ValidationMessages.Errors().Any());
            Assert.IsFalse(old_apikey == apiClientCredentials.apiKey);

            //delete ApiClient
            actionResults = RegistrationHandler.DeleteApiClient(apiClientObj, Test_Api_Key);
            Assert.IsFalse(actionResults.ValidationMessages.Errors().Any());
        }

        //[TestMethod]
        //public void ValidateCredentialsTokenExchange()
        //{
        //    //validate ApiClient
        //    string oldToken = apiClientCredentials.token;

        //    ActionResults actionResults = RegistrationHandler.ValidateApiClientCredentials(apiClientCredentials);
        //    Assert.IsFalse(actionResults.ValidationMessages.Errors().Any());
        //    Assert.IsFalse(oldToken == apiClientCredentials.token);
        //}

        //[TestMethod]
        //public void DeleteApiClient()
        //{
        //    //delete ApiClient
        //    ActionResults actionResults = RegistrationHandler.DeleteApiClient(apiClientObj, Test_Api_Key);
        //    Assert.IsFalse(actionResults.ValidationMessages.Errors().Any());
        //}

        private SerialApiClient CreateSerialApiClientIn()
        {
            return new SerialApiClient()
            {
                apiClientGuid = "",
                serverUserGuid = Service_User_Guid,
                type = ApiType,
                name = Name,
                active = true
            };
        }

    }
}
