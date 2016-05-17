using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.Serializers
{
    public class JsonFieldsSerializer : IJsonFieldsSerializer
    {
        private const string DateTimeIso8601Format = "yyyy-MM-ddTHH:mm:sszzz";

        public string Serialize(ISerializableObject objectToSerialize, string fields)
        {
            if (objectToSerialize == null)
            {
                throw new ArgumentNullException("objectToSerialize");
            }

            IList<string> fieldsList = null;

            if (!string.IsNullOrEmpty(fields))
            {
                string primaryPropertyName = objectToSerialize.GetPrimaryPropertyName();

                fieldsList = GetPropertiesIntoList(fields);

                // Always add the root manually
                fieldsList.Add(primaryPropertyName);
            }

            string json = Serialize(objectToSerialize, fieldsList);

            return json;
        }

        private string Serialize(object objectToSerialize, IList<string> fields)
        {
            var serializer = new JsonSerializer
            {
                DateFormatString = DateTimeIso8601Format
            };

            JToken jToken = JToken.FromObject(objectToSerialize, serializer);

            if (fields != null)
            {
                jToken = jToken.RemoveEmptyChildrenAndFilterByFields(fields);
            }

            string jTokenResult = jToken.ToString();

            return jTokenResult;
        }

        private IList<string> GetPropertiesIntoList(string fields)
        {
            IList<string> properties = fields.ToLowerInvariant()
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Distinct()
                .ToList();

            return properties;
        }
    }
}