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
using System.Web.Script.Serialization;
using Gov.Hhs.Cdc.Api;
using Gov.Hhs.Cdc.Api.Admin;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.CdcMediaValidationProvider;
using Gov.Hhs.Cdc.Connection;
using Gov.Hhs.Cdc.Global;
using Gov.Hhs.Cdc.MediaProvider;
using Gov.Hhs.Cdc.MediaValidatonProvider;
using Gov.Hhs.Cdc.TransactionLogProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaTests
{
    [TestClass]
    public class MediaValidationTests
    {
        private string authorizedUser = "";
        AdminApiServiceFactory adminService = new AdminApiServiceFactory();
        private static CsTransactionLogProvider _transactionLogProvider = null;
        public CsTransactionLogProvider TransactionLogProvider
        {
            get { return _transactionLogProvider = _transactionLogProvider ?? (_transactionLogProvider = new CsTransactionLogProvider()); }
        }

        //private class MockMediaExtractor : IMediaExtractor
        //{
        //    public ExtractionResult ExtractAndValidateHtml(bool isExtraction, MediaTypeValidationItem mediaTypeValidationItem, MediaAddress mediaAddress)
        //    {
        //        return new ExtractionResult()
        //        {
        //            MediaAddress = mediaAddress
        //        };
        //    }
        //}
        public MediaValidationTests()
        {
            ContentServicesDependencyBuilder.BuildAssembliesWithTestEMailProvider();
            CurrentConnection.Name = "ContentServicesDb";
        }

        [TestMethod]
        public void FluAlreadySyndicatedForWeb()
        {
            CsMediaValidationProvider mediaValidationProvider = new CsMediaValidationProvider();
            //{
            //    TheMediaExtractor = new MockMediaExtractor()
            //};

            var media = new MediaObject
            {
                ExtractionClasses = "syndicate",
                SourceUrl = "http://www......[domain]...../flu/index.htm",
                MediaTypeCode = "html"
            };

            var result = mediaValidationProvider.ExtractContent(media);
            Assert.IsTrue(result.MediaAddress.AddressIsAlreadyPersistedWithSameExtractionCriteria);
        }

        [TestMethod]
        public void FluAlreadySyndicatedForMobile()
        {
            CsMediaValidationProvider mediaValidationProvider = new CsMediaValidationProvider();
            //{
            //    TheMediaExtractor = new MockMediaExtractor()
            //};

            var media = new MediaObject
            {
                ExtractionClasses = "mSyndicate",
                SourceUrl = "http://www......[domain]...../flu/index.htm",
                MediaTypeCode = "html"
            };

            var result = mediaValidationProvider.ExtractContent(media);
            Assert.IsTrue(result.MediaAddress.AddressIsAlreadyPersistedWithSameExtractionCriteria);
        }

        [TestMethod]
        public void CanExtractUrlWithMetaTagContainingAmpersand()
        {
            var validator = new CsMediaValidationProvider();
            var media = new MediaObject
            {
                ExtractionClasses = "module",
                MediaTypeCode = "html",
                SourceUrl = "http://www......[domain]...../vhf/ebola/outbreaks/2014-west-africa/case-counts.html"
            };
            var result = validator.ExtractContent(media);
            Assert.AreEqual(0, result.ExtractedDetail.Messages.NumberOfErrors);
        }

        [TestMethod, Ignore]
        public void CanRetrieveMimeTypes()
        {
            //This isn't really used currently (media type / mime type validation)
            var mimes = CsMediaValidationProvider.ValidMimeTypes();
            Assert.IsTrue(mimes.Count > 1, mimes.Count.ToString());
        }

        [TestMethod]
        public void ExistingHtmlSaysItAlreadyExists()
        {
            SerialMediaAdmin media = TestApiUtility.SinglePublishedHtml();

            Console.WriteLine(media.sourceUrl);
            var obj = new MediaObject
            {
                MediaTypeCode = "html",
                SourceUrl = media.sourceUrl,
                ExtractionClasses = media.classNames,
                ExtractionElementIds = media.elementIds,
                ExtractionXpath = media.xPath
            };
            var result = new CsMediaValidationProvider().ExtractContent(obj);
            Assert.IsTrue(result.MediaAddress.AddressIsAlreadyPersistedWithSameExtractionCriteria);
        }

        [TestMethod]
        public void DuplicateHtmlIsDuplicate()
        {
            var media = TestApiUtility.SinglePublishedHtml();
            var obj = new MediaObject
            {
                MediaTypeCode = "html",
                SourceUrl = media.sourceUrl,
                ExtractionClasses = media.classNames, //syndicate, which is correct
                ExtractionElementIds = media.elementIds,
                ExtractionXpath = media.xPath
            };
            var result = new CsMediaValidationProvider().ExtractContent(obj);
            Assert.IsTrue(result.MediaAddress.AddressIsAlreadyPersistedWithSameExtractionCriteria);
            //Assert.IsFalse(result.IsValid, media.sourceUrl);
        }

        [TestMethod]
        public void DuplicateHtmlIsDuplicateViaApiWithClassNames()
        {
            var media = TestApiUtility.SinglePublishedHtml();
            //TODO:  Find one that has classes, not xpath or elementids
            Console.WriteLine(media.elementIds);
            Console.WriteLine(media.xPath);
            Assert.IsFalse(string.IsNullOrEmpty(media.classNames));
            Console.WriteLine(media.sourceUrl);

            var extractionParms = new List<string>()
            {
                @"url=" + media.sourceUrl,
                @"clsids=" + media.classNames
            };

            List<SerialMediaValidation> result;
            var messages = TestApiUtility.ApiGet<SerialMediaValidation>(adminService,
                adminService.CreateTestUrl("validations", "", "", string.Join(@"&", extractionParms)),
                out result);
            var val = result.FirstOrDefault().validation;
            Assert.IsTrue(val.isDuplicate);
            //Assert.AreEqual(media.id, val.existingMediaId.ToString());
            //TODO:  Put back after I clean up duplicates and make tests clean up after themselves,
            //specifically what is adding http://www......[domain]...../tobacco/data_statistics/by_topic/secondhand_smoke/index.htm
            Assert.IsTrue(val.isDuplicate);
        }

        [TestMethod]
        public void DuplicateHtmlIsDuplicateViaApiWithoutClassNames()
        {
            //var media = TestApiUtility.SinglePublishedHtml();
            var criteria = "?mediatype=HTML&status=published";
            var results = TestApiUtility.AdminApiMediaSearch(criteria);
            var media = results.Where(m => m.classNames.Contains("syndicate")).FirstOrDefault();
            Assert.IsNotNull(media);

            //TODO:  Find one that has classes, not xpath or elementids, and the classes must be syndicate/msyndicate
            Console.WriteLine(media.elementIds);
            Console.WriteLine(media.xPath);
            Console.WriteLine(media.classNames);

            Assert.IsFalse(string.IsNullOrEmpty(media.classNames));
            Console.WriteLine(media.sourceUrl);

            var extractionParms = new List<string>()
            {
                @"url=" + media.sourceUrl
            };

            List<SerialMediaValidation> result;
            var messages = TestApiUtility.ApiGet<SerialMediaValidation>(adminService,
                adminService.CreateTestUrl("validations", "", "", string.Join(@"&", extractionParms)),
                out result);
            var val = result.FirstOrDefault().validation;
            Assert.IsTrue(val.isDuplicate);
            Assert.AreEqual(media.id, val.existingMediaId.ToString());
        }

        [TestMethod]
        public void TestValidateResources()
        {
            List<SerialResource> resources = new List<SerialResource>()
            {
                new SerialResource(){resourceType = "ecard", url = @"http://.....[devServer]...../eCard_resources/smoking/ecard4smokefree.swf"},
                new SerialResource(){resourceType = "text", url = @"http://www......[domain]...../pcd/issues/2012/11_0343.htm"},
                new SerialResource(){resourceType = "text", url = @"http://.....[devServer]...../eCard_resources/smoking/ecard4smokefree.swf"},
                new SerialResource(){resourceType = "ecard", url = @"http://Bad server name/eCard_resources/smoking/ecard4smokefree.swf"},
                new SerialResource(){resourceType = "WebPage", url = @"http://www......[domain]...../flu"}

            };

            var url = adminService.CreateTestUrl("validations", "", "", "");
            var response = TestApiUtility.ApiPostWithoutOutput<List<SerialResource>>(adminService,
                url,                resources, authorizedUser);

            var errors = response.Errors().ToList();
            Console.WriteLine(errors);
            Assert.AreEqual(2, errors.Count());
            Assert.AreEqual("Resource[2]", errors[0].Key);
            Assert.AreEqual("Resource[3]", errors[1].Key);
        }

        [TestMethod]
        public void ValidateHtml5PageTitle()
        {
            var parms = new List<string>()
            {
                @"url=http://www......[domain]...../pneumonia/atypical/index.html",
                @"clsids=syndicate"
            };
            List<SerialMediaValidation> results;
            Console.WriteLine(string.Join(@"&", parms));
            ValidationMessages messages = TestApiUtility.ApiGet<SerialMediaValidation>(adminService,
                adminService.CreateTestUrl("validations", "", "", string.Join(@"&", parms)),
                out results);
            if (messages.Errors().Count() > 0)
            {
                Console.WriteLine(messages.Errors().First().Message);
                Assert.Fail(messages.Errors().Count() + " errors.");
            }
            Assert.AreEqual(@"Pneumonia | Home | Atypical | CDC", results[0].title);
        }

        [TestMethod]
        public void ValidateUrlWithSpaces()
        {
            var parms = new List<string>()
            {
                @"url=http://.....[devServer]...../sample/CDC%20-%20Bringing%20an%20Animal%20into%20U.S.%20%20Cats%20-%20Animal%20Importation.htm",
                @"clsids=syndicate"
            };
            List<SerialMediaValidation> results;
            ValidationMessages messages = TestApiUtility.ApiGet<SerialMediaValidation>(adminService,
                adminService.CreateTestUrl("validations", "", "", string.Join(@"&", parms)),
                out results);
            if (messages.Errors().Count() > 0)
            {
                Console.WriteLine(messages.Errors().First().Message);
                Assert.Fail(messages.Errors().Count() + " errors.");
            }
            Assert.IsTrue(results[0].validation.urlAlreadyExists);
        }

        [TestMethod]
        public void ValidateMetaTagContainingAmpersand()
        {
            var url = "http://www......[domain]...../vhf/ebola/outbreaks/2014-west-africa/case-counts.html";
            var result = TestApiUtility.ValidateUrl(url);
            Assert.AreEqual(1, result.ResultObject.Count);
        }

        [TestMethod]
        public void ValidateMetaTagContainingAmpersand2()
        {
            var url = "http://www......[domain]...../vhf/ebola/outbreaks/2014-west-africa/case-counts.html";
            var cssClass = "module";
            var result = TestApiUtility.ValidateUrl(url, cssClass);
            Assert.AreEqual(1, result.ResultObject.Count);
        }

        [TestMethod]
        public void ValidateExistingUrl()
        {
            string validTopicIds = "[25272, 25329, 25651]";

            string json = @"{'mediatype':'eCard','mimetype':'.swf','encoding':'utf-8','title':'ValidateExistingUrl'
        ,'sourceUrl':'http://.....[devServer]...../eCard_resources/cabinet.swf','targetUrl':'http://www......[domain]...../flu'
,'rowVersion':'AAAAAAAMoT4=','width':'800','height':'600','eCard':{'mobileCardName':'Clay&#39;s stupendous mobile ecard'
,'html5Source':'http://.....[devServer]...../eCard_resources/STD01.html','mobileTargetUrl':'http://www......[domain]...../flu'
,'caption':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardText':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardTextOutside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.','cardTextInside':'Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry&#39;s standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.'
,'imageSourceInsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg'
,'imageSourceInsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg'
,'imageSourceOutsideLarge':'http://.....[devServer]...../eCard_resources/cabinet.jpg'
,'imageSourceOutsideSmall':'http://.....[devServer]...../eCard_resources/cabinet.jpg'
,'isMobile':true,'isActive':false},'sourceCode':'Centers for Disease Control and Prevention','language':'English','status':'published'
,'datepublished':'2014-02-11T20:15:00.000Z','topics':" + validTopicIds + ",'owningOrgId':'11','maintainingOrgId':'11'}";
            SerialMediaAdmin mediaObject = new JavaScriptSerializer().Deserialize<SerialMediaAdmin>(json);

            mediaObject.sourceUrl = "http://www......[domain]...../Motorvehiclesafety/Teen_Drivers/teendrivers_factsheet.html";
            mediaObject.title = "Effective Preferences Test 1";
            mediaObject.id = "";
            mediaObject.effectivePreferences = CreatePreferences("syndicate", stripScript: true);

            List<SerialMediaAdmin> savedMedia;
            ValidationMessages messages = TestApiUtility.ApiPost<SerialMediaAdmin>(adminService,
                adminService.CreateTestUrl("media"), mediaObject,
                out savedMedia, authorizedUser);

            if (messages.NumberOfErrors > 0){
                Assert.Fail(messages.Errors().First().Message);
            }

            Assert.IsTrue(savedMedia.Count > 0);

            var extractionParms = new List<string>()
            {
                @"url=http://www......[domain]...../Motorvehiclesafety/Teen_Drivers/teendrivers_factsheet.html",
                @"clsids=syndicate"
            };

            List<SerialMediaValidation> results;
            ValidationMessages validationMessages = TestApiUtility.ApiGet<SerialMediaValidation>(adminService,
                adminService.CreateTestUrl("validations", "", "", string.Join(@"&", extractionParms)),
                out results);

            TestApiUtility.ApiDelete(adminService, adminService.CreateTestUrl("media", savedMedia[0].id), authorizedUser);
        }

        private static List<SerialPreferenceSet> CreatePreferences(string className, bool stripScript)
        {
            return new List<SerialPreferenceSet>()
            {
                new SerialPreferenceSet()
                {
                    type = "WebPage",
                    isDefault = true,
                    htmlPreferences = new SerialHtmlPreferences()
                    {
                        imageAlign = "right",
                        stripScript = stripScript,
                        contentNamespace="ABC",
                        includedElements = new SerialExtractionPath()
                        {
                            classNames = new List<string>(){className}
                        }
                        //excludedElements = new SerialExtractionPath()
                        //{
                        //    elementIds = new List<string>(){"Element1", "Element2"}
                        //}
                    }
                },
                new SerialPreferenceSet()
                {
                    type = "Mobile",
                    isDefault = false,
                    htmlPreferences = new SerialHtmlPreferences()
                    {
                        includedElements = new SerialExtractionPath()
                        {
                            classNames = new List<string>(){"msyndicate"}
                        }
                    }
                }
            };
        }

        [TestMethod]
        public void CanValidateWithXPath()
        {
            var htmlPreferences = new SerialHtmlPreferences()
            {
                includedElements = new SerialExtractionPath()
                {
                    xPath = @"//html:body/descendant::html:img[@src='/flu/weekly/weeklyarchives2014-2015/images/image251.gif']//parent::*"
                }
            };

            var mediaPreferences = new List<SerialPreferenceSet> { new SerialPreferenceSet()
            {
                htmlPreferences = htmlPreferences,
                isDefault = true,
                type = "WebPage"
            }};
            string url = "http://www......[domain]...../flu/index.htm";

            var theMediaValidationProvider = new CsMediaValidationProvider();
            var result = theMediaValidationProvider.ExtractContent(new MediaObject
            {
                SourceUrl = url,
                MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType,
                Preferences = new PreferencesSet
                {
                    PreferencesPersistedForMediaItem = PreferenceTransformation.CreateMediaPreferences(mediaPreferences)
                }
            });

            Assert.AreEqual(1, result.ExtractedDetail.NumberOfElements);

            var result2 = theMediaValidationProvider.ValidateHtmlForUrl(
                new MediaObject
                {
                    SourceUrl = url,
                    MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType,
                    Preferences = new PreferencesSet
                    {
                        PreferencesPersistedForMediaItem = PreferenceTransformation.CreateMediaPreferences(mediaPreferences)
                    }
                });
            Assert.AreEqual(1, result2.ExtractedDetail.NumberOfElements);
        }


        [TestMethod]
        public void TestMediaValidationByMediaId()
        {
            var media = TestApiUtility.SinglePublishedHtml();
            int mediaId = int.Parse(media.mediaId);

            MediaPreferenceSet extractionCriteria = new MediaPreferenceSet()
            {
                MediaPreferences = new HtmlPreferences()
                {
                    IncludedElements = new ExtractionPath()
                    {
                        ClassNames = new List<string>() { "syndicate" }
                    },
                    ExcludedElements = new ExtractionPath()
                    {
                        ClassNames = new List<string>() { "subColumns", "c50l", "subcl" }
                    },
                    NewWindow = true,
                    Iframe = true,
                    ContentNamespace = "CDC",
                    ImageAlign = ""

                }
            };

            List<ExtractionResult> results = new List<ExtractionResult>();
            var theMediaValidationProvider = new CsMediaValidationProvider();
            results.Add(theMediaValidationProvider.ValidateHtml(mediaId, null, null));

            results.Add(theMediaValidationProvider.ValidateHtml(mediaId, null, null));
            results.Add(theMediaValidationProvider.ValidateHtml(mediaId, null, extractionCriteria));

            int warnings = results[0].Messages.NumberOfStandardsWarnings;
            int countOfValidationsWithDifferentWarningCounts = results.Where(r => r.Messages.NumberOfStandardsWarnings != warnings).Count();
            Assert.AreEqual(0, countOfValidationsWithDifferentWarningCounts);
        }

        [TestMethod]
        public void TestMediaValidationByUrl()
        {

            var extractionCriteria = new List<SerialPreferenceSet> { new SerialPreferenceSet()
            {
                htmlPreferences = new SerialHtmlPreferences()
                {
                    includedElements = new SerialExtractionPath()
                    {
                        classNames = new List<string>() { "syndicate" }
                    },
                    excludedElements = new SerialExtractionPath()
                    {
                        classNames = new List<string>() { "subColumns", "c50l", "subcl" }
                    },
                    newWindow = true,
                    contentNamespace = "CDC",

                }}
            };
            string url = "http://www......[domain]...../tobacco/data_statistics/fact_sheets/cessation/quitting/index.htm";


            List<ExtractionResult> results = new List<ExtractionResult>();
            var theMediaValidationProvider = new CsMediaValidationProvider();
            var result = theMediaValidationProvider.ValidateHtmlForUrl(new MediaObject
            {
                SourceUrl = url,
                Preferences = new PreferencesSet { PreferencesPersistedForMediaItem = PreferenceTransformation.CreateMediaPreferences(extractionCriteria) },
                MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType
            });
            results.Add(theMediaValidationProvider.ValidateHtmlForUrl(new MediaObject
            {
                SourceUrl = url,
                Preferences = new PreferencesSet { PreferencesPersistedForMediaItem = PreferenceTransformation.CreateMediaPreferences(extractionCriteria) },
                MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType
            }));

            results.Add(theMediaValidationProvider.ValidateHtmlForUrl(new MediaObject
            {
                SourceUrl = url,
                Preferences = new PreferencesSet { PreferencesPersistedForMediaItem = PreferenceTransformation.CreateMediaPreferences(extractionCriteria) },
                MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType
            }));

            int warnings = result.Messages.NumberOfStandardsWarnings;
            int countOfValidationsWithDifferentWarningCounts = results.Where(r => r.Messages.NumberOfStandardsWarnings != warnings).Count();
            Assert.AreEqual(0, countOfValidationsWithDifferentWarningCounts);
            Assert.IsTrue(results[0].ExtractedDetail.Data.Length > 100);
        }


        [TestMethod]
        public void TestValidateMediaManyTimesForPerf()
        {
            int attempts = 15;

            var extractionCriteria = new List<SerialPreferenceSet> { new SerialPreferenceSet()
            {
                htmlPreferences = new SerialHtmlPreferences()
                {
                    includedElements = new SerialExtractionPath()
                    {
                        classNames = new List<string>() { "syndicate" }
                    },
                    excludedElements = new SerialExtractionPath()
                    {
                        classNames = new List<string>() { "subColumns", "c50l", "subcl" }
                    },
                    newWindow = true,
                    contentNamespace = "CDC",
                    imageAlign = ""
                }}
            };
            string url = "http://www......[domain]...../tobacco/data_statistics/oshdata/index.htm";
            //var api = "https://.....[devReportingApplicationServer2]...../adminapi/v1/resources/resources/0/validate";
            var api = adminService.CreateTestUrl("validations", "", "", "").ToString();

            var data = "[{\"url\":\"" + url + "\",\"resourceType\":\"WebPage\"}]";
            Console.WriteLine(data);
            for (int i = 0; i <= attempts; i++)
            {
                var theMediaValidationProvider = new CsMediaValidationProvider();
                var result = theMediaValidationProvider.ValidateHtmlForUrl(new MediaObject
                {
                    SourceUrl = url,
                    MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType,
                    Preferences = new PreferencesSet { PreferencesPersistedForMediaItem = PreferenceTransformation.CreateMediaPreferences(extractionCriteria) }
                });
                if (result.Messages.NumberOfErrors > 0)
                {
                    Assert.Fail(result.Messages.Errors().First().Message);
                }

                var apiResult = TestApiUtility.CallAPIPost(api, data, authorizedUser);
                Console.WriteLine(apiResult);
                Assert.AreNotEqual(string.Empty, apiResult);
            }
        }

        [TestMethod]
        public void TestValidateMediaForResponseCode()
        {
            string url = "http://www......[domain]...../pcd/issues/2013/12_0119_es.htm";
            //var api = "https://.....[devReportingApplicationServer2]...../adminapi/v1/resources/resources/0/validate";

            //var data = "[{\"url\":\"" + url + "\",\"resourceType\":\"WebPage\"}]";
            var api = adminService.CreateTestUrl("validations", "", "", "url=" + url);

            //var apiResult = TestApiUtility.CallAPIPost(api, data, authorizedUser);

            //Console.WriteLine(apiResult);

            //Assert.AreNotEqual(string.Empty, apiResult);
            var results = new List<SerialMediaValidation>();
            TestApiUtility.ApiGet<SerialMediaValidation>(adminService, api, out results);
            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results.First().validation.isValid);
        }

        [TestMethod]
        public void EbolaIsValidThoughAlreadySyndicated()
        {
            string url = "http://www......[domain]...../vhf/ebola/outbreaks/2014-west-africa/case-counts.html";
            var api = adminService.CreateTestUrl("validations", "", "", "url=" + url);
            var results = new List<SerialMediaValidation>();
            TestApiUtility.ApiGet<SerialMediaValidation>(adminService, api, out results);
            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results.First().validation.isValid);
        }

        [TestMethod]
        public void CanValidateWithXPathViaApi()
        {
            var xpath = @"//html:body/descendant::html:img[@src='/flu/weekly/weeklyarchives2014-2015/images/image251.gif']//parent::*";
            string url = "http://www......[domain]...../flu/index.htm";

            var api = adminService.CreateTestUrl("validations", "", "", "url=" + url + "&xpath=" + xpath);
            var results = new List<SerialMediaValidation>();
            TestApiUtility.ApiGet<SerialMediaValidation>(adminService, api, out results);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(@"<a href=""http://www......[domain]...../flu/weekly/fluactivitysurv.htm"" xmlns=""http://www.w3.org/1999/xhtml""><img alt=""Influenza Virus Isolated"" src=""http://www......[domain]...../flu/weekly/weeklyarchives2014-2015/images/image251.gif"" /></a>", results.First().content);
        }

        [TestMethod]
        public void CanValidateAutismWithClassNames()
        {
            var url = "http://www......[domain]...../features/living-with-autism/index.html";
            var api = adminService.CreateTestUrl("validations", "", "", "url=" + url + "&clsids=syndicate");
            var results = new List<SerialMediaValidation>();
            TestApiUtility.ApiGet<SerialMediaValidation>(adminService, api, out results);
            Assert.AreEqual(1, results.Count);
            Console.WriteLine(results.First().content);
            Assert.IsTrue(results.First().content.StartsWith("<div class=\"syndicate\""));
        }

        [TestMethod]
        public void CanValidateAutismWithoutClassNames()
        {
            var url = "http://www......[domain]...../features/living-with-autism/index.html";
            var api = adminService.CreateTestUrl("validations", "", "", "url=" + url);
            var results = new List<SerialMediaValidation>();
            TestApiUtility.ApiGet<SerialMediaValidation>(adminService, api, out results);
            Assert.AreEqual(1, results.Count);
            Console.WriteLine(results.First().content);
            Assert.IsTrue(results.First().content.StartsWith("<div class=\"syndicate\""));
        }

        [TestMethod]
        public void CessationIsDuplicateWithoutClassNames()
        {
            var url = "http://www......[domain]...../Features/SmokingCessation/";
            var api = adminService.CreateTestUrl("validations", "", "", "url=" + url);
            var results = new List<SerialMediaValidation>();
            TestApiUtility.ApiGet<SerialMediaValidation>(adminService, api, out results);
            Assert.AreEqual(1, results.Count);
            Console.WriteLine(results.First().content);
            Assert.IsTrue(results.First().validation.isDuplicate);
        }

        [TestMethod]
        public void CessationIsDuplicateWithClassNames()
        {
            var url = "http://www......[domain]...../Features/SmokingCessation/";
            var api = adminService.CreateTestUrl("validations", "", "", "url=" + url + "&clsids=syndicate");
            var results = new List<SerialMediaValidation>();
            TestApiUtility.ApiGet<SerialMediaValidation>(adminService, api, out results);
            Assert.AreEqual(1, results.Count);
            Console.WriteLine(results.First().content);
            Assert.IsTrue(results.First().validation.isDuplicate);
        }


        [TestMethod]
        public void CessationIsDuplicate()
        {
            var media = TestApiUtility.AdminApiMediaSearch("?mediatype=html&urlcontains=http://www......[domain]...../Features/SmokingCessation/").First();
            var obj = new MediaObject
            {
                MediaTypeCode = "html",
                SourceUrl = media.sourceUrl,
                ExtractionClasses = media.classNames, //syndicate, mSyndicate
                ExtractionElementIds = media.elementIds,
                ExtractionXpath = media.xPath
            };
            var result = new CsMediaValidationProvider().ExtractContent(obj);
            Assert.IsTrue(result.MediaAddress.AddressIsAlreadyPersistedWithSameExtractionCriteria);
            Assert.AreNotEqual(0, result.MediaAddress.ExistingMediaId);
            //Assert.IsFalse(result.IsValid, media.sourceUrl);
        }

        [TestMethod]
        public void ExistingHtmlWithNoExtractionCriteriaIsDuplicate()
        {
            var mediaIdWithNullEmbedParameters = "287279";
            var media = TestApiUtility.AdminApiMediaSearch(mediaIdWithNullEmbedParameters).First();
            var obj = new MediaObject
            {
                MediaTypeCode = "html",
                SourceUrl = media.sourceUrl
            };
            var result = new CsMediaValidationProvider().ExtractContent(obj);
            Assert.IsTrue(result.MediaAddress.AddressIsAlreadyPersistedWithSameExtractionCriteria);
            Assert.AreNotEqual(0, result.MediaAddress.ExistingMediaId);
        }

        [TestMethod]
        public void TestMediaItemExistsWithVaryingExtractionCriteria()
        {
            var existingCommunityGuideMediaId = 285160;
            var adminService = new AdminApiServiceFactory();
            var parms = new List<string>()
            {
                @"url=http://www.thecommunityguide.org/about/conclusionreport.html",
                @"clsids=syndicate"
            };
            List<SerialMediaValidation> results;
            ValidationMessages messages = TestApiUtility.ApiGet<SerialMediaValidation>(adminService,
                adminService.CreateTestUrl("validations", "", "", string.Join(@"&", parms)),
                out results);
            Assert.IsNotNull(results);
            Assert.IsNotNull(results[0]);
            var val = results[0].validation;
            Assert.IsTrue(val.isDuplicate);
            Assert.AreEqual(existingCommunityGuideMediaId, val.existingMediaId);

            parms = new List<string>()
            {
                @"url=http://www.thecommunityguide.org/about/conclusionreport.html",
                @"clsids=otherSyndicate"
            };
            messages = TestApiUtility.ApiGet<SerialMediaValidation>(adminService,
                adminService.CreateTestUrl("validations", "", "", string.Join(@"&", parms)),
                out results);
            Assert.IsFalse(results[0].validation.isDuplicate);
        }

        [TestMethod]
        public void TestDetermineSourceFromUrl()
        {
            var extractionCriteria = new List<SerialPreferenceSet> { 
                new SerialPreferenceSet{
                htmlPreferences = new SerialHtmlPreferences {
                    includedElements = new SerialExtractionPath { 
                        classNames = new List<string>() { "Syndicate"}
                    },
                    newWindow = true,
                    contentNamespace = "CDC"
                }
            }
            };
            string url = "http://www......[domain]...../tobacco/data_statistics/fact_sheets/cessation/quitting/index.htm";

            var theMediaValidationProvider = new CsMediaValidationProvider();
            var media = new MediaObject
            {
                SourceUrl = url,
                MediaTypeCode = MediaTypeParms.DefaultHtmlMediaType,
                Preferences = new PreferencesSet
                {
                    PreferencesPersistedForMediaItem = PreferenceTransformation.CreateMediaPreferences(extractionCriteria)
                }
            };
            ExtractionResult results = theMediaValidationProvider.ValidateHtmlForUrl(media);

            Assert.AreEqual("CDC Tobacco", results.MediaAddress.SourceCode);
        }

    }
}
