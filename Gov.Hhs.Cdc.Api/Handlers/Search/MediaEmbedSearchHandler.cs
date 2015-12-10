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
    public sealed class MediaEmbedSearchHandler : HandlerSearchBase
    {
        private string Id { get; set; }

        public MediaEmbedSearchHandler(ICallParser parser, string id)
            : base(parser)
        {
            Id = id;
        }

        public override SearchParameters BuildSearchParameters()
        {
            if (Parser.Version == 1)
                Parser.ReplaceIntParmInCriteria("MediaId", ServiceUtility.ParsePositiveInt(Id));
            else
                Parser.ReplaceIntParmInCriteria("Id", ServiceUtility.ParsePositiveInt(Id));

            Parser.Criteria.Add(new Criterion("EffectiveStatus", new List<string>() { Param.MediaStatus.Published.ToString(), Param.MediaStatus.Hidden.ToString() }));

            // build searchParamaters
            SearchParam.ApplicationCode = "Media";
            SearchParam.DataSetCode = "Media";
            SearchParam.BasicCriteria = Parser.Criteria;
            SearchParam.Sorting = new Sorting(new List<SortColumn>()                        
            {
                new SortColumn("Id", SortOrderType.Asc)
            });

            if (Parser.Query.PageSize > 0 && (Parser.Query.PageNumber > 0 || Parser.Query.Offset > 0))
                SearchParam.Paging = new Paging(Parser.Query.PageSize, Parser.Query.PageNumber, Parser.Query.Offset);

            return SearchParam;
        }

        public override SerialResponse BuildResponse(SearchParameters searchParam)
        {
            ServiceUtility.GetDataSet(Parser, searchParam, Response);
            Response.results = Syndicate(Response.dataset);
            return Response;
        }

        private string Syndicate(DataSetResult dataset)
        {
            return string.IsNullOrEmpty(Id) ? string.Empty : GetEmbedCode(dataset);
        }

        private string GetEmbedCode(DataSetResult dataset)
        {
            MediaObject cmi = dataset.Records.Cast<MediaObject>().FirstOrDefault();
            return GetCode(cmi);
        }

        public string GetCode(MediaObject cmi)
        {
            string embed_code = string.Empty;

            if (cmi != null)
            {
                bool includeJqRef = true;   //default is true
                string jquerySrc = string.Empty, jsPluginSrc = string.Empty, cssHref = string.Empty;

                if (Parser.ParamDictionary.ContainsKey(Param.INCLUDE_JQUERY))
                {
                    bool.TryParse(Parser.ParamDictionary[Param.INCLUDE_JQUERY], out includeJqRef);
                }

                if (Parser.ParamDictionary.ContainsKey(Param.JQ_SRC))
                {
                    jquerySrc = Parser.ParamDictionary[Param.JQ_SRC];
                }

                if (Parser.ParamDictionary.ContainsKey(Param.JS_PLUGIN_SRC))
                {
                    jsPluginSrc = Parser.ParamDictionary[Param.JS_PLUGIN_SRC];
                }

                if (Parser.ParamDictionary.ContainsKey(Param.CSS_HREF))
                {
                    cssHref = Parser.ParamDictionary[Param.CSS_HREF];
                }

                // embed params
                var embedParm = new EmbedParam()
                {
                    cssHref = cssHref,
                    includeJqRef = includeJqRef,
                    jquerySrc = jquerySrc,
                    jsPluginSrc = jsPluginSrc
                };

                if (cmi.MediaTypeParms.CreateEmbedHtml)
                {
                    MediaPreferenceSet embedCodePrefs = PreferenceTransformation.GetHtmlExtractionCriteria(base.Parser.ParamDictionary, base.Parser.Version);

                    if (string.IsNullOrEmpty(cmi.Embedcode) || embedCodePrefs.MediaPreferences.IsCustom)
                    {
                        UpdateParserWidthHeight(cmi);
                        //MediaPreferenceSet preferences = PreferenceTransformation.GetHtmlExtractionCriteria(base.Parser.ParamDictionary, base.Parser.Version);
                        if (base.Parser.Version == 1)
                        {
                            embed_code = new CodeEmbedder(base.Parser, cmi.Id.ToString()).EmbedHtmlMediaType(embedCodePrefs.MediaPreferences, cmi.MediaTypeParms.Code, cmi.Title, cmi.Description, embedParm);
                        }
                        else
                        {
                            embed_code = new CodeEmbedderPlus(base.Parser, cmi.Id.ToString()).EmbedHtmlMediaType(embedCodePrefs.MediaPreferences, cmi.MediaTypeParms.Code, cmi.Title, cmi.Description);
                        }
                    }
                    else
                    {
                        embed_code = cmi.Embedcode;
                    }
                }
                else if (cmi.MediaTypeParms.IsWidget)
                {
                    if (string.IsNullOrEmpty(cmi.Embedcode))
                    {
                        UpdateParserWidthHeight(cmi);
                        if (base.Parser.Version == 1)
                        {
                            embed_code = new CodeEmbedder(base.Parser, cmi.Id.ToString()).EmbedWidget(cmi.MediaTypeParms.Code, cmi.Title, cmi.Description, embedParm);
                        }
                        else
                        {
                            embed_code = new CodeEmbedderPlus(base.Parser, cmi.Id.ToString()).EmbedWidget(cmi.MediaTypeParms.Code, cmi.Title, cmi.Description);
                        }
                    }
                    else
                    {
                        embed_code = cmi.Embedcode;
                    }
                }
                else if (cmi.MediaTypeParms.IsBadgeButtonVideo || cmi.MediaTypeParms.IsStaticImageMedia)   //|| cmi.MediaTypeParms.IsEcard)
                {
                    if (string.IsNullOrEmpty(cmi.Embedcode))
                    {
                        UpdateParserWidthHeight(cmi);
                        if (base.Parser.Version == 1)
                        {
                            embed_code = new CodeEmbedder(base.Parser, cmi.Id.ToString()).EmbedBadgeButtonVideoImageInfographic(cmi.MediaTypeParms.Code, cmi.Title, cmi.Description, embedParm);
                        }
                        else
                        {
                            embed_code = new CodeEmbedderPlus(base.Parser, cmi.Id.ToString()).EmbedBadgeButtonVideoImageInfographic(cmi.MediaTypeParms.Code, cmi.Title, cmi.Description);
                        }
                    }
                    else
                    {
                        embed_code = cmi.Embedcode;
                    }
                }
                else if (cmi.MediaTypeParms.IsEcard)
                {
                    if (string.IsNullOrEmpty(cmi.Embedcode))
                    {
                        UpdateParserWidthHeight(cmi);
                        if (base.Parser.Version == 1)
                        {
                            embed_code = new CodeEmbedder(base.Parser, cmi.Id.ToString()).EmbedBadgeButtonVideoImageInfographic(cmi.MediaTypeParms.Code, cmi.Title, cmi.Description, embedParm);
                        }
                        else
                        {
                            embed_code = new CodeEmbedderPlus(base.Parser, cmi.Id.ToString()).EmbedEcard(cmi.MediaTypeParms.Code, cmi.Title, cmi.Description);
                        }
                    }
                    else
                    {
                        embed_code = cmi.Embedcode;
                    }
                }
                else if (cmi.MediaTypeParms.IsPdf)
                {
                    if (string.IsNullOrEmpty(cmi.Embedcode))
                    {
                        UpdateParserWidthHeight(cmi);
                        if (base.Parser.Version != 1)
                        {
                            embed_code = new CodeEmbedderPlus(base.Parser, cmi.Id.ToString()).EmbedPdf(cmi.MediaTypeParms.Code, cmi.Title, cmi.Description, cmi.DocumentDataSize);
                        }
                    }
                    else
                    {
                        embed_code = cmi.Embedcode;
                    }
                }
            }

            return embed_code;
        }

        private void UpdateParserWidthHeight(MediaObject cmi)
        {
            base.Parser.Query.Width = cmi.Width ?? 0;
            base.Parser.Query.Height = cmi.Height ?? 0;
        }
    }
}
