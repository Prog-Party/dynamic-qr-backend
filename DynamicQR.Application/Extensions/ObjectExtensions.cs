using Newtonsoft.Json.Linq;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DynamicQR.Application.Extensions;

public static class ObjectUtility
{
    public static Dictionary<string, string> ConvertToDictionary(this object obj)
    {
        try
        {
            var dictionary = new Dictionary<string, string>();
            if (obj == null)
                return dictionary;

            var sourceType = obj.GetType();

            var sourceName = sourceType.Name;
            if (IsAnonymousType(obj))
                sourceName = "Object";

            if (sourceName == "JObject")
                MapJObjectToDictionary(dictionary, obj, sourceName);
            else if (IsValueType(sourceType))
                dictionary[sourceName] = obj.ToString()!;
            else
                MapToDictionaryInternal(dictionary, obj, sourceName);

            return dictionary;
        }
        catch (Exception)
        {
            var serializedObject = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var errorDictionary = new Dictionary<string, string>() { { "Error", "Could not Serialize obj" }, { "Message", "Could not process an object, therefore it is serialized." }, { "Serialized object", serializedObject } };
            return errorDictionary;
        }
    }

    public static string DictionaryToString(Dictionary<string, string> props)
        => string.Join(", ", props.Select(p => $"[ {p.Key}, {p.Value} ]"));

    public static object ToCollections(object o)
    {
        if (o is JObject jo) return jo.ToObject<IDictionary<string, object>>()!.ToDictionary(k => k.Key, v => ToCollections(v.Value));
        if (o is JArray ja) return ja.ToObject<List<object>>()!.Select(ToCollections).ToList();
        return o;
    }

    public static bool IsAnonymousType(object obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        var type = obj.GetType();

        if (type == null)
            throw new ArgumentNullException("type");

        // HACK: The only way to detect anonymous types right now.
        return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
            && type.IsGenericType && type.Name.Contains("AnonymousType")
            && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
            && type.Attributes.HasFlag(TypeAttributes.NotPublic);
    }

    private static void MapJObjectToDictionary(Dictionary<string, string> dictionary, object source, string name, int depth = 1)
    {
        if (depth > 25)
            throw new StackOverflowException("Too many recursions for " + name);

        var sourceTypeName = source.GetType().Name;

        if (sourceTypeName == "JObject")
        {
            var jObject = (JObject)source;
            var properties = jObject.ToObject<Dictionary<string, object>>();
            foreach (var property in properties ?? new())
            {
                var key = name + "." + property.Key;
                object value = property.Value;

                if (value == null)
                    continue;

                var valueType = value.GetType();

                if (IsValueType(valueType))
                {
                    dictionary[key] = value.ToString()!;
                }
                else
                {
                    MapJObjectToDictionary(dictionary, value, key, depth + 1);
                }
            }
        }


        if (sourceTypeName == "JArray")
        {
            var jArray = (JArray)source;
            var i = 0;
            var properties = jArray.ToObject<List<object>>();
            foreach (var property in properties ?? new())
            {
                var arrayKey = name + "[" + i + "]";
                if (IsValueType(property.GetType()))
                    dictionary[arrayKey] = property.ToString()!;
                else
                    MapJObjectToDictionary(dictionary, property, arrayKey, depth + 1);
                i++;
            }
        }
    }

    private static void MapToDictionaryInternal(Dictionary<string, string> dictionary, object source, string name, int depth = 1)
    {
        if (depth > 25)
            throw new StackOverflowException("Too many recursions for " + name);

        var properties = source.GetType().GetProperties();
        foreach (var p in properties)
        {
            if (p.GetCustomAttribute(typeof(Newtonsoft.Json.JsonIgnoreAttribute)) != null)
                continue;

            var key = name + "." + p.Name;
            object? value = p.GetValue(source, null);

            if (value == null)
                continue;

            var valueType = value.GetType();

            if (IsValueType(valueType))
            {
                dictionary[key] = value.ToString()!;
            }
            else if (value is IEnumerable)
            {
                var i = 0;
                foreach (object o in (IEnumerable)value)
                {
                    var arrayKey = key + "[" + i + "]";
                    if (IsValueType(o.GetType()))
                        dictionary[arrayKey] = o.ToString()!;
                    else
                        MapToDictionaryInternal(dictionary, o, arrayKey, depth + 1);
                    i++;
                }
            }
            else
            {
                MapToDictionaryInternal(dictionary, value, key, depth + 1);
            }
        }
    }

    private static bool IsValueType(Type type)
    {
        return type.IsPrimitive ||
                type.IsValueType ||
                type == typeof(string) ||
                type == typeof(DateTime) ||
                type == typeof(DateTimeOffset) ||
                type == typeof(Uri) ||
                type == typeof(Guid);
    }
}
