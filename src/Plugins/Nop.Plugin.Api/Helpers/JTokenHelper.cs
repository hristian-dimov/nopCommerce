using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Nop.Plugin.Api.Helpers
{
    public static class JTokenHelper
    {
        public static JToken RemoveEmptyChildrenAndFilterByFields(this JToken token, IList<string> fields)
        {
            if (token.Type == JTokenType.Object)
            {
                var copy = new JObject();

                foreach (JProperty prop in token.Children<JProperty>())
                {
                    JToken child = prop.Value;

                    if (child.HasValues)
                    {
                        child = child.RemoveEmptyChildrenAndFilterByFields(fields);
                    }

                    bool shouldAdd = fields.Contains(prop.Name.ToLowerInvariant());

                    /*
                        if(level == 2 || shouldAdd)
                            if(!child.IsEmptyOrDefault())
                    */

                    if (!child.IsEmptyOrDefault() && shouldAdd)
                    {
                        copy.Add(prop.Name, child);
                    }
                }

                return copy;
            }

            if (token.Type == JTokenType.Array)
            {
                var copy = new JArray();

                foreach (JToken item in token.Children())
                {
                    JToken child = item;

                    if (child.HasValues)
                    {
                        child = child.RemoveEmptyChildrenAndFilterByFields(fields);
                    }

                    if (!child.IsEmptyOrDefault())
                    {
                        copy.Add(child);
                    }
                }

                return copy;
            }

            return token;
        }

        private static bool IsEmptyOrDefault(this JToken token)
        {
            return (token.Type == JTokenType.Array && !token.HasValues) ||
                    (token.Type == JTokenType.Object && !token.HasValues);
        }
    }
}
