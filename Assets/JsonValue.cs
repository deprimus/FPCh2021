using System;
using System.Text;

/// <summary>
/// Contains classes used to handle JSON data.
/// </summary>
namespace Helpers.Json
{
    /// <summary>
    /// Represents a JSON value.
    /// </summary>
    public class JsonValue
    {
        public enum ValueType { STRING, NUMBER, BOOL, ARRAY, OBJECT };

        /// <summary>
        /// The type of the value.
        /// </summary>
        public ValueType Type;

        /// <summary>
        /// The value itself.
        /// </summary>
        public Object Value;

        public JsonValue() { }

        public JsonValue(ValueType type, Object value)
        {
            Type = type;
            Value = value;
        }

        /// <summary>
        /// Gets the JSON value as a string.
        /// </summary>
        public string AsString() => (string)Value;

        /// <summary>
        /// Gets the JSON value as a number.
        /// </summary>
        public double AsNumber() => (double)Value;

        /// <summary>
        /// Gets the JSON value as a boolean.
        /// </summary>
        public bool AsBool() => (bool)Value;

        /// <summary>
        /// Gets the JSON value as an array of JSON values.
        /// </summary>
        public JsonValue[] AsArray() => (JsonValue[])Value;

        /// <summary>
        /// Gets the JSON value as a JSON object.
        /// </summary>
        public Json AsObject() => (Json)Value;

        /// <summary>
        /// Converts the JSON value to its string representation.
        /// </summary>
        public override string ToString()
        {
            switch (Type)
            {
                case ValueType.STRING:
                    return "\"" + AsString() + "\"";
                case ValueType.NUMBER:
                    return AsNumber().ToString();
                case ValueType.BOOL:
                    return AsBool() ? "true" : "false";
                case ValueType.ARRAY:
                    StringBuilder builder = new StringBuilder("[");

                    JsonValue[] array = AsArray();

                    if (array.Length > 0)
                        builder.Append(array[0].ToString());

                    for (Int32 i = 1; i < array.Length; ++i)
                    {
                        builder.Append(",");
                        builder.Append(array[i].ToString());
                    }

                    builder.Append("]");

                    return builder.ToString();
                case ValueType.OBJECT:
                    return AsObject().ToString();
                default:
                    return "";
            }
        }
    }
}