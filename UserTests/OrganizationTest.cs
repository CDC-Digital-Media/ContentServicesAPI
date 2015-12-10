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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gov.Hhs.Cdc.Search.Controller;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.UserProvider;
//using SimMetricsMetricUtilities;

namespace UserTests
{
    [TestClass]
    public class OrganizationTest
    {
        static SearchControllerFactory SearchControllerFactory = new SearchControllerFactory();

        [TestMethod]
        public void TestTypeAhead()
        {
            Criteria filterCriteria = new Criteria();
            filterCriteria.Add("Name", "C");
            SearchParameters searchParameters = CreateNewSearchParameters(filterCriteria);
            DataSetResult organizations = SearchControllerFactory.GetSearchController(searchParameters).Get();
            List<OrganizationItem> orgs = (List<OrganizationItem>)organizations.Records;
            Assert.AreEqual(1, orgs.Count);
            Assert.AreEqual("CDC", orgs[0].Name);
            string a = orgs.ToString();
        }

        [TestMethod]
        public void TestByOrganizationType()
        {
            Criteria filterCriteria = new Criteria();
            filterCriteria.Add("OrgType", "U.S. Federal Government");
            SearchParameters searchParameters = CreateNewSearchParameters(filterCriteria);
            DataSetResult organizations = SearchControllerFactory.GetSearchController(searchParameters).Get();
            List<OrganizationItem> orgs = (List<OrganizationItem>)organizations.Records;
            Assert.AreEqual(4, orgs.Count);
            //Assert.AreEqual("CDC", orgs[0].Name);
            string a = orgs.ToString();
        }

        private SearchParameters CreateNewSearchParameters(Criteria filterCriteria)
        {
            SearchParameters searchParameters = new SearchParameters()
            {
                ApplicationCode = "Media",
                DataSetCode = "Organization",
                FilterCriteria = filterCriteria,
                Sorting = new Sorting(new SortColumn("Name", SortOrderType.Asc))
            };
            return searchParameters;
        }

        //public double Compare1(string value1, string value2)
        //{
        //    return jaroWinkler.GetSimilarity(value1.Replace(" ", "").Replace(".", "").ToLower(), value2.Replace(" ", "").Replace(".", "").ToLower());
        //}

        //public double Compare2(string value1, string value2)
        //{
        //    return levenstein.GetSimilarity(value1.Replace(" ", "").Replace(".", "").ToLower(), value2.Replace(" ", "").Replace(".", "").ToLower());
        //}
        //JaroWinkler jaroWinkler = new JaroWinkler();

        //Levenstein levenstein = new Levenstein();

        //[TestMethod]
        //public void TestSimMetrics()
        //{

        //    List<double> similarities = new List<double>();
        //    similarities.Add(Compare1("Center for Disease Control", "Center for Disease Control"));
        //    similarities.Add(Compare2("Center for Disease Control", "Center for Disease Control"));

        //    similarities.Add(Compare1("Center for Disease Control", "CENTER FOR DISEASE CONTROL"));
        //    similarities.Add(Compare2("Center for Disease Control", "CENTER FOR DISEASE CONTROL"));

        //    similarities.Add(Compare1("Center for Disease Control", "Center  for  Disease  Control   "));
        //    similarities.Add(Compare2("Center for Disease Control", "Center  for  Disease  Control   "));

        //    similarities.Add(Compare1("Center for Disease Control", "Center for Disease Ctl"));
        //    similarities.Add(Compare2("Center for Disease Control", "Center for Disease Ctl"));

        //    similarities.Add(Compare1("Center for Disease Control", "Centers for Disease Control"));
        //    similarities.Add(Compare2("Center for Disease Control", "Centers for Disease Control"));

        //    similarities.Add(Compare1("United States Federal Government", "US Federal Government"));    //10
        //    similarities.Add(Compare2("United States Federal Government", "US Federal Government"));    //11

        //    similarities.Add(Compare1("U.S. Federal Government", "US Federal Government"));
        //    similarities.Add(Compare2("U.S. Federal Government", "US Federal Government"));

        //    similarities.Add(Compare1("U. S. Federal Government", "US Federal Government"));
        //    similarities.Add(Compare2("U. S. Federal Government", "US Federal Government"));

        //    similarities.Add(Compare1("USSR. Federal Government", "US Federal Government"));
        //    similarities.Add(Compare2("USSR. Federal Government", "US Federal Government"));

        //    similarities.Add(Compare1("USA Federal Government", "US Federal Government"));
        //    similarities.Add(Compare2("USA Federal Government", "US Federal Government"));

        //    string a = similarities.ToString();

        //}
    }
}
