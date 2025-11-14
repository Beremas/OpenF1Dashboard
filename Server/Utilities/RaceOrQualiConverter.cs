using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace OpenF1Dashboard.Server.Utilities
{
    public class RaceOrQualiConverter : JsonConverter<List<double?>>
    {
        public override List<double?>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            var list = new List<double?>();

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        return list;

                    if (reader.TokenType == JsonTokenType.Number)
                    {
                        list.Add(reader.GetDouble());
                    }
                    else if (reader.TokenType == JsonTokenType.String)
                    {
                        var strVal = reader.GetString();
                        if (double.TryParse(strVal, NumberStyles.Any, CultureInfo.InvariantCulture, out var num))
                            list.Add(num);
                        else
                            list.Add(null);
                    }
                    else if (reader.TokenType == JsonTokenType.Null)
                    {
                        list.Add(null);
                    }
                    else
                    {
                        list.Add(null); // fallback
                    }
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                list.Add(reader.GetDouble());
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                var str = reader.GetString();
                if (double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                    list.Add(val);
                else
                    list.Add(null);
            }
            else
            {
                return null;
            }

            return list;
        }

        public override void Write(Utf8JsonWriter writer, List<double?>? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartArray();
            foreach (var v in value)
            {
                if (v.HasValue)
                    writer.WriteNumberValue(v.Value);
                else
                    writer.WriteNullValue();
            }
            writer.WriteEndArray();
        }
    }


}
