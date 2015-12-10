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

using System.IO;

using System.ServiceModel;
using System.ServiceModel.Web;
using Gov.Hhs.Cdc.Api;

namespace Gov.Hhs.Cdc.Api.Admin
{
    [ServiceContract]
    public interface IAdmin
    {
        #region "/v1/resources/ --> media|valuesets|validation|"

        [WebInvoke(Method = "GET", UriTemplate = "/v1/" + Param.API_ROOT + "/{service}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGet(string service);

        [WebInvoke(Method = "GET", UriTemplate = "/v1/" + Param.API_ROOT + "/{service}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGetSlash(string service);

        [WebInvoke(Method = "GET", UriTemplate = "/v1/" + Param.API_ROOT + "/{service}" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGetFormat(string service, string format);

        [WebInvoke(Method = "GET", UriTemplate = "/v1/" + Param.API_ROOT + "/{service}" + ".{format}/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGetFormatSlash(string service, string format);

        [WebInvoke(Method = "GET", UriTemplate = "/v1/" + Param.API_ROOT + "/{service}/{levelOne}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGetLevel1(string service, string levelOne);

        [WebInvoke(Method = "GET", UriTemplate = "/v1/" + Param.API_ROOT + "/{service}/{levelOne}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGetLevel1Slash(string service, string levelOne);

        [WebInvoke(Method = "GET", UriTemplate = "/v1/" + Param.API_ROOT + "/{service}/{levelOne}" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGetLevel1Format(string service, string levelOne, string format);

        [WebInvoke(Method = "GET", UriTemplate = "/v1/" + Param.API_ROOT + "/{service}/{levelOne}" + ".{format}/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGetLevel1FormatSlash(string service, string levelOne, string format);

        [WebInvoke(Method = "GET", UriTemplate = "/v1/" + Param.API_ROOT + "/{service}/{levelOne}/{levelTwo}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGetLevel2(string service, string levelOne, string levelTwo);

        [WebInvoke(Method = "GET", UriTemplate = "/v1/" + Param.API_ROOT + "/{service}/{levelOne}/{levelTwo}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodLevel2GetSlash(string service, string levelOne, string levelTwo);

        [WebInvoke(Method = "GET", UriTemplate = "/v1/" + Param.API_ROOT + "/{service}/{levelOne}/{levelTwo}" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodLevel2GetFormat(string service, string levelOne, string levelTwo, string format);

        [WebInvoke(Method = "GET", UriTemplate = "/v1/" + Param.API_ROOT + "/{service}/{levelOne}/{levelTwo}" + ".{format}/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodLevel2GetFormatSlash(string service, string levelOne, string levelTwo, string format);

        #endregion

        #region "Object Management --> Standard REST verbs  POST|PUT|DELETE"
        
        // add
        [WebInvoke(Method = "POST", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultPost(string dataset, Stream data);

        // add
        [WebInvoke(Method = "POST", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultSlashPost(string dataset, Stream data);

        // add
        [WebInvoke(Method = "POST", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultPostFormat(string dataset, Stream data, string format);

        // add
        [WebInvoke(Method = "POST", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}" + ".{format}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultSlashPostFormat(string dataset, Stream data, string format);

        // edit/update
        [WebInvoke(Method = "PUT", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultUpdate(string dataset, string id, Stream data);

        // /edit/update
        [WebInvoke(Method = "PUT", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}/{id}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultSlashPut(string dataset, string id, Stream data);

        // edit/update
        [WebInvoke(Method = "PUT", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}/{id}" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultPutFormat(string dataset, string id, Stream data, string format);

        // /edit/update
        [WebInvoke(Method = "PUT", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}/{id}" + ".{format}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultSlashPutFormat(string dataset, string id, Stream data, string format);

        // delete
        [WebInvoke(Method = "DELETE", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultDelete(string dataset, string id, Stream data);

        // delete
        [WebInvoke(Method = "DELETE", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}/{id}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultSlashDelete(string dataset, string id, Stream data);

        // delete
        [WebInvoke(Method = "DELETE", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}/{id}" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultDeleteFormat(string dataset, string id, Stream data, string format);

        // delete
        [WebInvoke(Method = "DELETE", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}/{id}" + ".{format}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultSlashDeleteFormat(string dataset, string id, Stream data, string format);

        #endregion

        #region "Object Management -> using POST for DELETE & PUT/UPDATE"

        // edit/update
        [WebInvoke(Method = "POST", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}/{id}/update", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultPostUpdate(string dataset, string id, Stream data);

        // /edit/update
        [WebInvoke(Method = "POST", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}/{id}/update" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultSlashPostUpdate(string dataset, string id, Stream data);

        // edit/update
        [WebInvoke(Method = "POST", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}/{id}/update" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultPostUpdateFormat(string dataset, string id, Stream data, string format);

        // /edit/update
        [WebInvoke(Method = "POST", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}/{id}/update" + ".{format}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultSlashPostUpdateFormat(string dataset, string id, Stream data, string format);

        // delete
        [WebInvoke(Method = "POST", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}/{id}/delete", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultPostDelete(string dataset, string id, Stream data);

        // delete
        [WebInvoke(Method = "POST", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}/{id}/delete" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultSlashPostDelete(string dataset, string id, Stream data);

        // delete
        [WebInvoke(Method = "POST", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}/{id}/delete" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultPostDeleteFormat(string dataset, string id, Stream data, string format);

        // delete
        [WebInvoke(Method = "POST", UriTemplate = "/v1/" + Param.API_ROOT + "/{dataset}/{id}/delete" + ".{format}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultSlashPostDeleteFormat(string dataset, string id, Stream data, string format);

        #endregion

        #region "/v1/resources/media/ --> mediaId/refreshtoken"

        [WebInvoke(Method = "PUT", UriTemplate = "/v1/" + Param.API_ROOT + "/{service}/{levelOne}/{levelTwo}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultPutRouteLevelTwo(string service, string levelOne, string levelTwo, Stream data);

        [WebInvoke(Method = "PUT", UriTemplate = "/v1/" + Param.API_ROOT + "/{service}/{levelOne}/{levelTwo}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultPutSlashRouteLevelTwo(string service, string levelOne, string levelTwo, Stream data);

        [WebInvoke(Method = "PUT", UriTemplate = "/v1/" + Param.API_ROOT + "/{service}/{levelOne}/{levelTwo}" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultPutFormatRouteLevelTwo(string service, string levelOne, string levelTwo, string format, Stream data);

        [WebInvoke(Method = "PUT", UriTemplate = "/v1/" + Param.API_ROOT + "/{service}/{levelOne}/{levelTwo}" + ".{format}/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void DefaultPutFormatSlashRouteLevelTwo(string service, string levelOne, string levelTwo, string format, Stream data);

        #endregion
    }
}
