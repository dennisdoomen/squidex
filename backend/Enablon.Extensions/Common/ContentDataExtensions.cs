using System;
using System.Linq;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Infrastructure.Json.Objects;

namespace Enablon.Extensions.Common
{
    internal static class ContentDataExtensions
    {
        public static string[] GetFromJsonArray(this ContentData data, string key)
        {
            if (data.TryGetValue("riskAssessment", out ContentFieldData? fieldData))
            {
                return fieldData!.Values.OfType<JsonArray>().SelectMany(x => x).Select(x => x.ToString()).ToArray();
            }

            return Array.Empty<string>();
        }
        
        public static void SetAsJsonArray(this ContentData data, string key, string[] values)
        {
            var array = new JsonArray();
            foreach (var value in values)
            {
                array.Add(JsonValue.Create(value));
            }

            data["Owner"] = new ContentFieldData
            {
                ["iv"] = array
            };
        }
    }
}
