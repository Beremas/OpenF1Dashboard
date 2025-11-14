using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenF1Dashboard.CustomConverter
{
	public class FlexibleDecimalListConverter : JsonConverter<List<decimal?>>
	{
		public override List<decimal?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var result = new List<decimal?>();

			if (reader.TokenType == JsonTokenType.StartArray)
			{
				// Handle array: [0.056, 1.035, null]
				while (reader.Read())
				{
					if (reader.TokenType == JsonTokenType.EndArray)
						break;

					result.Add(ReadSingleValue(ref reader));
				}
			}
			else
			{
				// Handle single value: 1.035 or null or "N/A"
				result.Add(ReadSingleValue(ref reader));
			}

			return result;
		}

		private decimal? ReadSingleValue(ref Utf8JsonReader reader)
		{
			if (reader.TokenType == JsonTokenType.Number && reader.TryGetDecimal(out var number))
				return number;

			if (reader.TokenType == JsonTokenType.String)
			{
				var str = reader.GetString();
				return decimal.TryParse(str, out var parsed) ? parsed : null;
			}

			if (reader.TokenType == JsonTokenType.Null)
				return null;

			throw new JsonException($"Unexpected token in decimal list: {reader.TokenType}");
		}

		public override void Write(Utf8JsonWriter writer, List<decimal?> value, JsonSerializerOptions options)
		{
			writer.WriteStartArray();
			foreach (var item in value)
			{
				if (item.HasValue)
					writer.WriteNumberValue(item.Value);
				else
					writer.WriteNullValue();
			}
			writer.WriteEndArray();
		}
	}



}
