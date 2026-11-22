using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Bot.Core.Lookup
{
    public static class JsonParseUtil
    {
        public static string? GetObjectStringProp(JObject obj, string propName)
        {
            if (obj.TryGetValue(propName, out var token) && token != null && token.Type == JTokenType.String)
            { 
                return token.Value<string>();
            }
            return null;
        }

        public static bool TryGetObjectBoolProp(JObject obj, string propName, out bool value)
        {
            value = false;
            if (obj.TryGetValue(propName, out var token) && token != null && token.Type == JTokenType.Boolean)
            {
                value = token.Value<bool>();
                return true;
            }
            return false;
        }

        public static JArray? GetObjectArrayProp(JObject obj, string propName)
        {
            if (obj.TryGetValue(propName, out var token))
            {
                return token as JArray;
            }
            return null;
        }

        public static CharacterData? ParseCharacterData(JObject obj, bool isOfficial)
        {
            string? id = GetObjectStringProp(obj, "id");
            string? name = GetObjectStringProp(obj, "name");
            string? ability = GetObjectStringProp(obj, "ability");
            if (name == null || ability == null || id == null)
            {
                return null;
            }

            if (!Enum.TryParse<CharacterTeam>(GetObjectStringProp(obj, "team"), ignoreCase: true, out var team))
            { 
                return null; 
            }

            CharacterData cd = new(id, name, ability, team, isOfficial: isOfficial);

            cd.FlavorText = GetObjectStringProp(obj, "flavor");
            var imageUrlStr = GetObjectStringProp(obj, "image");

            if (imageUrlStr != null)
            {
                cd.ImageUrl = EscapeUri(imageUrlStr);
            }

            return cd;
        }

        public static string EscapeUri(string value)
        {
            const string reserved = ":/?#[]@!$&'()*+,;=";

            StringBuilder result = new();
            int segmentStart = 0;

            for (int i = 0; i < value.Length; i++)
            {
                if (reserved.Contains(value[i]))
                {
                    result.Append(Uri.EscapeDataString(value[segmentStart..i]));
                    result.Append(value[i]);
                    segmentStart = i + 1;
                }
                else if (value[i] == '%' && i + 2 < value.Length &&
                         Uri.IsHexDigit(value[i + 1]) &&
                         Uri.IsHexDigit(value[i + 2]))
                {
                    result.Append(Uri.EscapeDataString(value[segmentStart..i]));
                    result.Append(value.Substring(i, 3));
                    i += 2;
                    segmentStart = i + 1;
                }
            }

            result.Append(Uri.EscapeDataString(value[segmentStart..]));
            return result.ToString();
        }
    }
}