using DynamicQR.Application.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Application.Tests.Utility;

[ExcludeFromCodeCoverage]
internal static class TestUtility
{
    internal static void TestIfObjectsAreEqual(object? obj1, object? obj2)
    {
        var obj1Dictionary = obj1?.ConvertToDictionary() ?? new();
        var obj2Dictionary = obj2?.ConvertToDictionary() ?? new();

        List<string> errors = new();

        // Check if the keys and values are the same
        foreach (var obj1Item in obj1Dictionary)
        {
            var key = obj1Item.Key;
            if (!obj2Dictionary.ContainsKey(key))
            {
                errors.Add($"{key} is not present in obj2");
                continue;
            }

            if (obj1Item.Value != obj2Dictionary[key])
            {
                errors.Add($"The value of {key} ({obj1Item.Value}) is not equal in obj2 ({obj2Dictionary[key]})");
                continue;
            }
        }

        // check if the keys in obj2Dictionary are also present in obj1Dictionary
        foreach (var obj2Item in obj2Dictionary)
        {
            var key = obj2Item.Key;
            if (!obj1Dictionary.ContainsKey(key))
            {
                errors.Add($"{key} is not present in obj1");
                continue;
            }
        }

        Assert.False(errors.Any(), string.Join(Environment.NewLine, errors));
    }
}
