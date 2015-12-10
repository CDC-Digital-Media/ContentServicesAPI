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

using Gov.Hhs.Cdc.Bo;
using Gov.Hhs.Cdc.DataServices.Bo;
using Gov.Hhs.Cdc.MediaProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Gov.Hhs.Cdc.Api
{
    public class MediaTransformationHelper : MediaTransformationBase
    {
        public const int AttributeIdForTopic = 1;
        public const int AttributeIdForAudience = 2;

        public static List<AttributeValueObject> CreateAttributeValues(string language, List<int> topics, List<int> audiences, Dictionary<string, List<SerialValueItem>> attributes)
        {
            var values = new List<AttributeValueObject>();

            values.AddRange(ValuesFromTopics(language, topics));
            values.AddRange(ValuesFromAudiences(language, audiences));

            if (attributes == null)
            {
                return values;
            }
            foreach (var item in attributes)
            {
                foreach (var value in item.Value)
                {
                    string adjustedForCasing = item.Key.ToLower();
                    values.Add(new AttributeValueObject
                    {
                        AttributeName = adjustedForCasing[0].ToString().ToUpper() + adjustedForCasing.Substring(1),
                        ValueKey = new ValueKey { Id = value.id, LanguageCode = language },
                        ValueName = value.name
                    });
                }
            }
            return values;
        }

        public static List<AttributeValueObject> ValuesFromTopics(string language, List<int> topics)
        {
            var values = new List<AttributeValueObject>();
            if (topics == null)
            {
                return values;
            }

            foreach (var topic in topics)
            {
                values.Add(new AttributeValueObject
                {
                    AttributeName = "Topic",
                    AttributeId = AttributeIdForTopic,
                    ValueKey = new ValueKey { Id = topic, LanguageCode = language }
                });
            }
            return values;
        }

        public static List<AttributeValueObject> ValuesFromAudiences(string language, List<int> audiences)
        {
            var values = new List<AttributeValueObject>();
            if (audiences == null)
            {
                return values;
            }

            foreach (var audience in audiences)
            {
                values.Add(new AttributeValueObject
                {
                    AttributeName = "Audience",
                    AttributeId = AttributeIdForAudience,
                    ValueKey = new ValueKey { Id = audience, LanguageCode = language }
                });
            }
            return values;
        }

        public static SerialTags BuildSerialTags(IEnumerable<AttributeValueObject> attributes)
        {
            if (attributes == null)
            {
                return new SerialTags();
            }

            return new SerialTags()
            {
                topic = attributes.Where(a => string.Equals(a.AttributeName, "Topic", StringComparison.OrdinalIgnoreCase))
                    .Select(b => GetAttribute(b)).ToList(),
                audience = attributes.Where(a => string.Equals(a.AttributeName, "Audience", StringComparison.OrdinalIgnoreCase)).ToList()
                    .Select(b => GetAttribute(b)).ToList()
            };
        }

        public static Dictionary<string, List<SerialValueItem>> BuildSerialAttributeDictionary(IEnumerable<AttributeValueObject> attributes)
        {
            var items = new Dictionary<string, List<SerialValueItem>>();
            if (attributes != null)
            {
                var uniqueAttributes = new List<string>();
                foreach (var att in attributes)
                {
                    if (items.ContainsKey(att.AttributeName.ToLower()))
                    {
                        items[att.AttributeName.ToLower()].Add(GetAttribute(att));
                    }
                    else
                    {
                        items.Add(att.AttributeName.ToLower(), new List<SerialValueItem> { GetAttribute(att) });
                    }
                }
            }
            return items;
        }

        public static SerialValueItem GetAttribute(AttributeValueObject attr)
        {
            return new SerialValueItem()
            {
                id = attr.ValueKey.Id,
                attributeName = attr.AttributeName,
                name = attr.ValueName,
                //language = Util.HtmlEncodeOutput(attr.ValueKey.LanguageCode)
            };
        }

        public static List<SerialValueItemTag> BuildSerialHashTags(IEnumerable<AttributeValueObject> attributes)
        {
            if (attributes == null)
            {
                return new List<SerialValueItemTag>();
            }

            return attributes.Select(a => GetAttributeHash(a)).ToList();
        }

        public static SerialValueItemTag GetAttributeHash(AttributeValueObject attr)
        {
            return new SerialValueItemTag()
            {
                id = attr.ValueKey.Id,
                name = attr.ValueName,
                language = attr.ValueKey.LanguageCode,
                type = attr.AttributeName
            };
        }

        public static List<SerialGeoTag> BuildSerialGeoTags(IEnumerable<MediaGeoDataObject> attributes)
        {
            if (attributes == null)
            {
                return new List<SerialGeoTag>();
            }

            return attributes.Select(a => GetMediaGeoLocation(a)).ToList();
        }

        public static SerialGeoTag GetMediaGeoLocation(MediaGeoDataObject item)
        {
            return new SerialGeoTag()
            {
                geoNameId = item.GeoNameId,
                name = item.Name,
                countryCode = item.CountryCode,
                parentId = item.ParentId,
                latitude = item.Latitude,
                longitude = item.Longitude,
                timezone = item.Timezone,
                admin1Code = item.Admin1Code
            };
        }

        public static List<SerialAlternateImage> BuildSerialAlternateImage(IEnumerable<StorageObject> storage, bool forExport)
        {
            var atlImage = new List<SerialAlternateImage>();
            if (storage != null)
            {
                atlImage = storage.Select(a =>
                    new SerialAlternateImage()
                    {
                        id = a.StorageId,
                        name = a.Name,
                        width = a.Width,
                        height = a.Height,
                        type = a.Type,
                        url = AppendApiKeyToUrlQueryString(ServiceUtility.GetStorageUrl(a.StorageId.ToString() + "." + a.FileExtension, forExport))
                    }).ToList();
            }

            return atlImage;
        }

        internal static Dictionary<string, string> BuildExtendedAttributes(IEnumerable<CsBusinessObjects.Media.ExtendedAttribute> attributes)
        {
            if (attributes == null)
            {
                return new Dictionary<string, string>();
            }

            return attributes.ToDictionary(a => a.Name, a => a.Value);
        }

        public static List<SerialEnclosure> BuildSerialEnclosures(IEnumerable<EnclosureObject> enclosureObj)
        {
            var enclosures = new List<SerialEnclosure>();
            if (enclosureObj != null)
            {
                enclosures = enclosureObj.Select(a =>
                    new SerialEnclosure()
                    {
                        id = a.Id,
                        resourceUrl = a.ResourceUrl,
                        contentType = a.ContentType,
                        size = a.Size
                    }).ToList();
            }

            return enclosures;
        }
        
        public static List<SerialAggregate> BuildSerialAggregates(IEnumerable<AggregateObject> obj)
        {
            var items = new List<SerialAggregate>();
            if (obj != null)
            {
                items = obj.Select(a => new SerialAggregate()
                {
                    id = a.Id,
                    queryString = TransformSearchCriteriaToNameValuePair(a.SearchXML)
                }).ToList();
            }

            return items;
        }

        private static string TransformSearchCriteriaToNameValuePair(string searchCriteria)
        {           
            XElement theDocument = XDocument.Parse(searchCriteria).Root;
            var theSearchCriteria = theDocument.FromXElement<CsBusinessObjects.Media.SearchCriteria>();
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(theSearchCriteria.ContentGroup)) 
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }

                sb.Append(string.Format(Param.CONTENT_GROUP + "={0}", theSearchCriteria.ContentGroup));
            }

            if (!string.IsNullOrEmpty(theSearchCriteria.TopicId))
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }

                sb.Append(string.Format(Param.TOPIC_ID_V2 + "={0}", theSearchCriteria.TopicId));
            }

            if (!string.IsNullOrEmpty(theSearchCriteria.RowsPerPage))
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }

                sb.Append(string.Format(Param.PAGE_RECORD_MAX + "={0}", theSearchCriteria.RowsPerPage));
            }
            
            return sb.ToString();
        }
    }
}
