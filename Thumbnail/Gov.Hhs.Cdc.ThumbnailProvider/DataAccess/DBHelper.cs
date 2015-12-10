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

using System.Data;
using System.Data.Objects;

using System.Data.SqlClient;
using System.Configuration;

using System.Collections;
using System.Data.Common;

using System.Data.EntityClient;
using System.Data.Metadata.Edm;
using System.IO;
using System.Collections.Specialized;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Gov.Hhs.Cdc.ThumbnailProvider.DAL
{
    class DBHelper
    {
        
        /// <summary>
        /// GetAppConnection() 
        /// </summary>
        /// <returns>Default application connection</returns>   
        public static EntityConnection GetAppConnection(string argName)
        {
            try
            {
                EntityConnectionStringBuilder connBuilder = new EntityConnectionStringBuilder();
                connBuilder.Provider = "System.Data.SqlClient";
                connBuilder.ProviderConnectionString = DBHelper.GetWebConfigConnectionString(argName);
                connBuilder.Metadata = SchemaConstants.CSIntegration_Media;
                return new EntityConnection(connBuilder.ToString());
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static string GetWebConfigConnectionString(string argName)
        {            
            foreach (System.Configuration.ConnectionStringSettings css in ConfigurationManager.ConnectionStrings)
            {
                if (css.Name == argName) { 
                    return css.ConnectionString; }
            }
            return string.Empty;
        }
        
    }
}
