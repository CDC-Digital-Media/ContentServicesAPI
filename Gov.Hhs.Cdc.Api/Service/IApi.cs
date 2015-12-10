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


using System.IO;

using System.ServiceModel;
using System.ServiceModel.Web;

using Gov.Hhs.Cdc.DataServices.Bo;

namespace Gov.Hhs.Cdc.Api
{
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface IApi
    {
        #region "/v1/resources/ --> media|valuesets|validation|"

        [WebInvoke(Method = "GET", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGet(string version, string resource);

        [WebInvoke(Method = "GET", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGetSlash(string version, string resource);

        [WebInvoke(Method = "GET", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGetFormat(string version, string resource, string format);

        [WebInvoke(Method = "GET", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}" + ".{format}/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGetFormatSlash(string version, string resource, string format);

        //---------------------------------------------------------------------------------------------------------------------------------------------

        [WebInvoke(Method = "GET", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGetId(string version, string resource, string id);

        [WebInvoke(Method = "GET", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGetIdSlash(string version, string resource, string id);

        [WebInvoke(Method = "GET", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGetIdFormat(string version, string resource, string id, string format);

        [WebInvoke(Method = "GET", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}" + ".{format}/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGetIdFormatSlash(string version, string resource, string id, string format);

        //---------------------------------------------------------------------------------------------------------------------------------------------

        [WebInvoke(Method = "GET", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}/{action}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGetIdAction(string version, string resource, string id, string action);

        [WebInvoke(Method = "GET", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}/{action}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGetIdActionSlash(string version, string resource, string id, string action);

        [WebInvoke(Method = "GET", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}/{action}" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGetIdActionFormat(string version, string resource, string id, string action, string format);

        [WebInvoke(Method = "GET", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}/{action}" + ".{format}/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodGetIdActionFormatSlash(string version, string resource, string id, string action, string format);

        #endregion

        //---------------------------------------------------------------------------------------------------------------------------------------------

        #region "Object Management --> Standard REST verbs  POST|PUT|DELETE"

        // add
        [WebInvoke(Method = "POST", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPost(string version, string resource, Stream data);

        // add
        [WebInvoke(Method = "POST", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPostSlash(string version, string resource, Stream data);

        // add
        [WebInvoke(Method = "POST", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPostFormat(string version, string resource, string format, Stream data);

        // add
        [WebInvoke(Method = "POST", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}" + ".{format}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPostFormatSlash(string version, string resource, string format, Stream data);

        //---------------------------------------------------------------------------------------------------------------------------------------------

        // add
        [WebInvoke(Method = "POST", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPostId(string version, string resource, string id, Stream data);

        // add
        [WebInvoke(Method = "POST", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPostIdSlash(string version, string resource, string id, Stream data);

        // add
        [WebInvoke(Method = "POST", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPostIdFormat(string version, string resource, string id, string format, Stream data);

        // add
        [WebInvoke(Method = "POST", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}" + ".{format}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPostIdFormatSlash(string version, string resource, string id, string format, Stream data);

        //---------------------------------------------------------------------------------------------------------------------------------------------

        [WebInvoke(Method = "POST", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}/{action}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPostIdAction(string version, string resource, string id, string action, Stream data);

        [WebInvoke(Method = "POST", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}/{action}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPostIdActionSlash(string version, string resource, string id, string action, Stream data);

        [WebInvoke(Method = "POST", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}/{action}" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPostIdActionFormat(string version, string resource, string id, string action, string format, Stream data);

        [WebInvoke(Method = "POST", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}/{action}" + ".{format}/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPostIdActionFormatSlash(string version, string resource, string id, string action, string format, Stream data);

        //---------------------------------------------------------------------------------------------------------------------------------------------

        // edit/update
        [WebInvoke(Method = "PUT", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPut(string version, string resource, Stream data);

        // edit/update
        [WebInvoke(Method = "PUT", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPutSlash(string version, string resource, Stream data);

        // edit/update
        [WebInvoke(Method = "PUT", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPutFormat(string version, string resource, string format, Stream data);

        // edit/update
        [WebInvoke(Method = "PUT", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}" + ".{format}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPutFormatSlash(string version, string resource, string format, Stream data);

        //---------------------------------------------------------------------------------------------------------------------------------------------

        // edit/update
        [WebInvoke(Method = "PUT", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPutId(string version, string resource, string id, Stream data);

        // /edit/update
        [WebInvoke(Method = "PUT", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPutIdSlash(string version, string resource, string id, Stream data);

        // edit/update
        [WebInvoke(Method = "PUT", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPutIdFormat(string version, string resource, string id, string format, Stream data);

        // /edit/update
        [WebInvoke(Method = "PUT", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}" + ".{format}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPutIdFormatSlash(string version, string resource, string id, string format, Stream data);

        //---------------------------------------------------------------------------------------------------------------------------------------------

        [WebInvoke(Method = "PUT", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}/{action}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPutIdAction(string version, string resource, string id, string action, Stream data);

        [WebInvoke(Method = "PUT", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}/{action}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPutIdActionSlash(string version, string resource, string id, string action, Stream data);

        [WebInvoke(Method = "PUT", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}/{action}" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPutIdActionFormat(string version, string resource, string id, string action, string format, Stream data);

        [WebInvoke(Method = "PUT", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}/{action}" + ".{format}/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodPutIdActionFormatSlash(string version, string resource, string id, string action, string format, Stream data);

        //---------------------------------------------------------------------------------------------------------------------------------------------

        // edit/update
        [WebInvoke(Method = "DELETE", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodDelete(string version, string resource, Stream data);

        // edit/update
        [WebInvoke(Method = "DELETE", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodDeleteSlash(string version, string resource, Stream data);

        // edit/update
        [WebInvoke(Method = "DELETE", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodDeleteFormat(string version, string resource, string format, Stream data);

        // edit/update
        [WebInvoke(Method = "DELETE", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}" + ".{format}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodDeleteFormatSlash(string version, string resource, string format, Stream data);

        //---------------------------------------------------------------------------------------------------------------------------------------------

        // delete
        [WebInvoke(Method = "DELETE", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodDeleteId(string version, string resource, string id, Stream data);

        // delete
        [WebInvoke(Method = "DELETE", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodDeleteIdSlash(string version, string resource, string id, Stream data);

        // delete
        [WebInvoke(Method = "DELETE", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodDeleteIdFormat(string version, string resource, string id, string format, Stream data);

        // delete
        [WebInvoke(Method = "DELETE", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}" + ".{format}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodDeleteIdFormatSlash(string version, string resource, string id, string format, Stream data);

        //---------------------------------------------------------------------------------------------------------------------------------------------

        [WebInvoke(Method = "DELETE", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}/{action}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodDeleteIdAction(string version, string resource, string id, string action, Stream data);

        [WebInvoke(Method = "DELETE", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}/{action}" + "/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodDeleteIdActionSlash(string version, string resource, string id, string action, Stream data);

        [WebInvoke(Method = "DELETE", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}/{action}" + ".{format}", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodDeleteIdActionFormat(string version, string resource, string id, string action, string format, Stream data);

        [WebInvoke(Method = "DELETE", UriTemplate = "/v{version}/" + Param.API_ROOT + "/{resource}/{id}/{action}" + ".{format}/", BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        void WebMethodDeleteIdActionFormatSlash(string version, string resource, string id, string action, string format, Stream data);

        #endregion

        //---------------------------------------------------------------------------------------------------------------------------------------------

    }
}
