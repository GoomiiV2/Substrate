using ImGuiNET;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Substrate
{
    public class ImGuiStyleConverter : JsonConverter
    {
        public static string ColorsPrefix = "Colors_";

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ImGuiStyle);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var jsonObj = JObject.Load(reader);

            var colorsProp = jsonObj.Property("Colors");
            if (colorsProp != null)
            {
                foreach (var colorProp in colorsProp)
                {
                    foreach (JProperty color in colorProp) {
                        if (!Enum.TryParse<ImGuiCol>(color.Name, out var id)) continue;
                        
                        var vec4Color = Utils.Vector4FromHex(color.Value.ToString());
                        jsonObj.Add($"{ColorsPrefix}{(int) id}", JObject.FromObject(vec4Color));
                    }
                }
            }

            var style = jsonObj.ToObject<ImGuiStyle>();
            return style;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);
            var colors = new Dictionary<string, JProperty>();

            writer.WriteStartObject();
            foreach (var val in t)
            {
                if (((JProperty)val).Name.StartsWith(ColorsPrefix))
                {
                    var idxStr  = ((JProperty)val).Name.Substring(ColorsPrefix.Length);
                    var idx     = int.Parse(idxStr);
                    var colName = Enum.GetName(typeof(ImGuiCol), idx);
                    colors.Add(colName, ((JProperty)val));
                }
                else
                {
                    serializer.Serialize(writer, val);
                }
            }

            writer.WritePropertyName("Colors");
            writer.WriteStartObject();
            foreach (var color in colors)
            {
                writer.WritePropertyName(color.Key);
                writer.WriteValue(Utils.Vector4ToHex(color.Value.Value.ToObject<Vector4>()));
            }
            writer.WriteEndObject();
            writer.WriteEndObject();
        }
    }
}
