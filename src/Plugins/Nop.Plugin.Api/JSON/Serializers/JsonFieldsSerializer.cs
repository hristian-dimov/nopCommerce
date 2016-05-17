using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.Validators;

namespace Nop.Plugin.Api.Serializers
{
    public class JsonFieldsSerializer : IJsonFieldsSerializer
    {
        private const string DateTimeIso8601Format = "yyyy-MM-ddTHH:mm:sszzz";

        private readonly IFieldsValidator _fieldsValidator;

        public JsonFieldsSerializer(IFieldsValidator fieldsValidator)
        {
            _fieldsValidator = fieldsValidator;
        }

        public string Serialize(ISerializableObject objectToSerialize, string fields)
        {
            if (objectToSerialize == null)
            {
                throw new ArgumentNullException("objectToSerialize");
            }

            Dictionary<string, bool> fieldsDictionary = null;

            if (!string.IsNullOrEmpty(fields))
            {
                IList<string> fieldsList = GetPropertiesIntoList(fields);

                //TODO: Remove the dictionary as we can simply use a List
                fieldsDictionary = fieldsList.ToDictionary(x => x, y => true);

                string primaryPropertyName = objectToSerialize.GetPrimaryPropertyName();

                fieldsDictionary.Add(primaryPropertyName, true);
            }

            string json = Serialize(objectToSerialize, fieldsDictionary);

            return json;
        }

        private string Serialize(object objectToSerialize, Dictionary<string, bool> fields)
        {
            var serializer = new JsonSerializer
            {
                DateFormatString = DateTimeIso8601Format
            };

            JToken jToken = JToken.FromObject(objectToSerialize, serializer);

            if (fields != null)
            {
                jToken = jToken.RemoveEmptyChildren(fields);
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
