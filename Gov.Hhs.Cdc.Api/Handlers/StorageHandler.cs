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

using System.Collections.Generic;
using System.Linq;
using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;

namespace Gov.Hhs.Cdc.Api
{
    public class StorageHandler : HandlerSearchBase, ISearchHandler
    {
        public StorageHandler(ICallParser parser)
            : base(parser)
        {
        }

        public override SearchParameters BuildSearchParameters()
        {
            List<SortColumn> columnList = Parser.SortColumns;

            // build searchParamaters
            SearchParam.ApplicationCode = "Media";
            SearchParam.DataSetCode = "Link";
            SearchParam.Sorting = new Sorting(new List<SortColumn>()                        
            {
                new SortColumn("StorageId", SortOrderType.Asc)
            });

            if (Parser.Query.PageSize > 0 && (Parser.Query.PageNumber > 0 || Parser.Query.Offset > 0))
            {
                SearchParam.Paging = new Paging(Parser.Query.PageSize, Parser.Query.PageNumber, Parser.Query.Offset);
            }

            if (columnList.Count > 0)
            {
                SearchParam.Sorting = new Sorting(columnList);
            }

            SearchParam.BasicCriteria = Parser.Criteria;

            return SearchParam;
        }

        public override SerialResponse BuildResponse(SearchParameters searchParam)
        {
            ServiceUtility.GetDataSet(Parser, searchParam, Response);
            Response.results = Response.dataset.Records.Cast<StorageObject>().ToList();

            return Response;
        }

        public void GetContent(IOutputWriter writer)
        {
            byte[] content = new byte[0];

            SerialResponse response = new SerialResponse();
            SearchParameters searchParameters = BuildSearchParameters();
            response = BuildResponse(searchParameters);

            var storage = (List<StorageObject>)response.results;

            if (storage != null && storage.Count == 1)
            {
                content = storage[0].ByteStream;
                Parser.Query.Format = ServiceUtility.GetFormatType(storage[0].FileExtension);
            }
            else
            {
                Parser.Query.Format = FormatType.png;
            }

            if (content == null)
            {
                content = new byte[0];
            }

            response.results = content;

            writer.Write(content);
        }

        public static ActionResults Create(SerialStorageAdmin obj, string apiKey)
        {
            ValidationMessages msgs = new ValidationMessages();

            var resultMediaList = new List<StorageObject>();
            if (RegistrationHandler.RegistrationProvider.GetApiClientByAppKey(apiKey, false) != null)
            {
                StorageObject theObject = new StorageAdminTransformation().CreateStorageObject(obj);

                StorageObject updatedObject;
                msgs.Add(MediaProvider.SaveStorage(theObject, out updatedObject));

                if (updatedObject != null)
                {
                    resultMediaList.Add(updatedObject);
                }
            }
            else
            {
                msgs.AddError("Links", "Invalid Request");
            }

            return new ActionResults(resultMediaList, msgs);
        }

        public static ValidationMessages Delete(int storageId, string apiKey)
        {
            ValidationMessages msgs = new ValidationMessages();

            var resultMediaList = new List<StorageObject>();
            if (RegistrationHandler.RegistrationProvider.GetApiClientByAppKey(apiKey) != null)
            {
                StorageObject theObject = new StorageAdminTransformation().CreateStorageObject(new SerialStorageAdmin() { storageId = storageId });

                msgs.Add(MediaProvider.DeleteStorage(theObject));
            }
            else
            {
                msgs.AddError("Media", "Invalid Request");
            }

            return msgs;
        }
    }
}
