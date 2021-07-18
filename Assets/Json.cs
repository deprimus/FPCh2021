using System;
using System.Text;
using System.Collections.Generic;

namespace Helpers.Json
{
    /// <summary>
    /// Represents a JSON object.
    /// </summary>
    public class Json
    {
        /// <summary>
        /// The JSON object properties, as an array of pairs.
        /// </summary>
        public Dictionary<string, JsonValue> Entries;

        public Json(Dictionary<string, JsonValue> entries)
        {
            Entries = entries;
        }

        /// <summary>
        /// Converts the JSON object to its string representation.
        /// </summary>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("{");

            bool first = true;

            foreach (KeyValuePair<string, JsonValue> entry in Entries)
            {
                if (!first)
                    builder.Append(",");
                else first = false;

                builder.Append("\"");
                builder.Append(entry.Key);
                builder.Append("\":");
                builder.Append(entry.Value.ToString());
            }

            builder.Append("}");

            return builder.ToString();
        }

        /// <summary>
        /// Converts a string to a JSON object.
        /// </summary>
        public static Json Parse(string json)
        {
            Dictionary<string, JsonValue> result = new Dictionary<string, JsonValue>();
            Int32 index = 0;

            ConsumeObject(json, ref index, ref result);
            return new Json(result);
        }

        private static void ConsumeObject(string json, ref Int32 index, ref Dictionary<string, JsonValue> result)
        {
            result = new Dictionary<string, JsonValue>();

            while (index < json.Length && Char.IsWhiteSpace(json[index]))
                ++index;

            if (json[index] != '{')
                throw new ParsingException("Expected '{' before object", json, index);

            ++index;

            while (index < json.Length)
            {
                string key = null;
                JsonValue value = null;

                while (index < json.Length && Char.IsWhiteSpace(json[index]))
                    ++index;

                if (index == json.Length)
                    throw new ParsingException("Unexpected end of input", json, index);

                if (json[index] == '}')
                {
                    ++index;
                    return;
                }

                ConsumeString(json, ref index, ref key);

                if (json[index] != ':')
                    throw new ParsingException("Expected ':' after key", json, index);

                ++index;

                while (index < json.Length && Char.IsWhiteSpace(json[index]))
                    ++index;

                ConsumeValue(json, ref index, ref value);

                result[key] = value;

                while (index < json.Length && Char.IsWhiteSpace(json[index]))
                    ++index;

                if (index == json.Length)
                    throw new ParsingException("Unexpected end of input", json, index);

                if (json[index] == '}')
                {
                    ++index;
                    return;
                }
                else if (json[index] == ',')
                {
                    ++index;
                    continue;
                }
                else throw new ParsingException("Expected '}' or ','", json, index);
            }

            throw new ParsingException("Expected '}'", json, index);
        }

        private static void ConsumeValue(string json, ref Int32 index, ref JsonValue result)
        {
            if (json[index] == '{')
            {
                Dictionary<string, JsonValue> data = null;
                ConsumeObject(json, ref index, ref data);

                result = new JsonValue();
                result.Type = JsonValue.ValueType.OBJECT;
                result.Value = new Json(data);
            }
            else if (json[index] == '[')
            {
                List<JsonValue> data = null;
                ConsumeArray(json, ref index, ref data);

                result = new JsonValue();
                result.Type = JsonValue.ValueType.ARRAY;
                result.Value = data.ToArray();
            }
            else if (json[index] == '"')
            {
                string data = null;
                ConsumeString(json, ref index, ref data);

                result = new JsonValue();
                result.Type = JsonValue.ValueType.STRING;
                result.Value = data;
            }
            else if (Char.IsDigit(json[index]) || json[index] == '.' || json[index] == '-')
            {
                double data = 0;
                ConsumeNumber(json, ref index, ref data);

                result = new JsonValue();
                result.Type = JsonValue.ValueType.NUMBER;
                result.Value = data;
            }
            else if (Char.IsLetter(json[index]))
            {
                bool data = false;
                ConsumeBool(json, ref index, ref data);

                result = new JsonValue();
                result.Type = JsonValue.ValueType.BOOL;
                result.Value = data;
            }
            else throw new ParsingException("Expected object, array, string, or number", json, index);
        }

        private static void ConsumeArray(string json, ref Int32 index, ref List<JsonValue> result)
        {
            result = new List<JsonValue>();

            while (index < json.Length && Char.IsWhiteSpace(json[index]))
                ++index;

            if (json[index] != '[')
                throw new ParsingException("Expected '[' before array", json, index);

            ++index;

            while (index < json.Length)
            {
                if (Char.IsWhiteSpace(json[index]))
                {
                    ++index;
                    continue;
                }

                if (json[index] == ']')
                {
                    ++index;
                    return;
                }

                JsonValue value = null;
                ConsumeValue(json, ref index, ref value);

                result.Add(value);

                while (index < json.Length && Char.IsWhiteSpace(json[index]))
                    ++index;

                if (index == json.Length)
                    throw new ParsingException("Unexpected end of input", json, index);

                if (json[index] == ']')
                {
                    ++index;
                    return;
                }
                else if (json[index] == ',')
                {
                    ++index;
                    continue;
                }
                else throw new ParsingException("Expected ']' or ','", json, index);
            }

            throw new ParsingException("Expected ']'", json, index);
        }

        private static void ConsumeString(string json, ref Int32 index, ref string result)
        {
            while (index < json.Length && Char.IsWhiteSpace(json[index]))
                ++index;

            if (index == json.Length)
                throw new ParsingException("Unexpected end of input", json, index);
            if (json[index] != '"')
                throw new ParsingException("Expected '\"' before start of string", json, index);

            ++index;

            Int32 end = index;

            while ((end < json.Length)
                && (json[end] != '"' || json[end - 1] == '\\'))
                ++end;

            if (end == json.Length)
                throw new ParsingException("Unexpected end of string", json, end);

            result = json.Substring(index, end - index);

            index = end + 1;
        }

        private static void ConsumeNumber(string json, ref Int32 index, ref double result)
        {
            while (index < json.Length && Char.IsWhiteSpace(json[index]))
                ++index;

            if (index == json.Length)
                throw new ParsingException("Unexpected end of input", json, index);

            Int32 end = index;

            while ((end < json.Length)
                && (Char.IsDigit(json[end]) || json[end] == '.' || json[end] == '-'))
                ++end;

            if (end == index)
                throw new ParsingException("Expected number", json, index);

            if (!double.TryParse(json.Substring(index, end - index), out result))
                throw new ParsingException("Expected number", json, index);

            index = end;
        }

        private static void ConsumeBool(string json, ref Int32 index, ref bool result)
        {
            while (index < json.Length && Char.IsWhiteSpace(json[index]))
                ++index;

            if (index == json.Length)
                throw new ParsingException("Unexpected end of input", json, index);

            Int32 end = index;

            while ((end < json.Length)
                && (Char.IsLetter(json[end])))
                ++end;

            if (end == index)
                throw new ParsingException("Expected boolean", json, index);

            if (!bool.TryParse(json.Substring(index, end - index), out result))
                throw new ParsingException("Expected boolean", json, index);

            index = end;
        }
    }

    public class ParsingException : Exception
    {
        public ParsingException(string message, string json, Int32 index)
            : base(String.Format("{0} at index {1} in {2}", message, index, json)) { }
    }
}