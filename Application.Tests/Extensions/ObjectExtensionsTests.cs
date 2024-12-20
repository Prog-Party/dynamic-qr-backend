using DynamicQR.Application.Extensions;
using FluentAssertions;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Application.Tests.Extensions;

[ExcludeFromCodeCoverage]
public sealed class ObjectExtensionsTests
{
    [Fact]
    public void Primative_Type_To_Dictionary_Test()
    {
        var str = "Hoi ik ben";
        var result = str.ConvertToDictionary();

        result["String"].Should().Be("Hoi ik ben");
    }

    [Fact]
    public void Simple_Object_To_Dictionary_Test()
    {
        var theObject = new { a = "123", b = 456 };

        var result = theObject.ConvertToDictionary();

        result["Object.a"].Should().Be("123");
        result["Object.b"].Should().Be("456");
    }

    [Fact]
    public void Nested_Object_To_Dictionary_Test()
    {
        var theObject = new
        {
            a = "123",
            b = new
            {
                b1 = "zzz",
                b2 = "yyy"
            }
        };

        var result = theObject.ConvertToDictionary();

        result.Count.Should().Be(3);
        result["Object.a"].Should().Be("123");
        result["Object.b.b1"].Should().Be("zzz");
        result["Object.b.b2"].Should().Be("yyy");
    }

    [Fact]
    public void Object_With_Ignore_Property_To_Dictionary_Test()
    {
        var theObject = new ObjectWithIgnoreProperty { a = "123", b = "456" };

        var result = theObject.ConvertToDictionary();
        result.Count.Should().Be(1);

        result["ObjectWithIgnoreProperty.a"].Should().Be("123");
    }

    [Fact]
    public void Json_To_Dictionary_Test()
    {
        var theObject = JsonConvert.DeserializeObject("{\"live\":\"false\",\"notificationItems\":[{\"NotificationRequestItem\":{\"additionalData\":{\"shopperCountry\":\"NL\",\"paymentMethodVariant\":\"idealtestissuer\"},\"amount\":{\"currency\":\"EUR\",\"value\":2662},\"eventCode\":\"REFUND\",\"eventDate\":\"2020-05-28T13:36:30+00:00\",\"reason\":\"\",\"success\":\"true\"}}]}");

        var result = theObject.ConvertToDictionary();

        result["JObject.live"].Should().Be("false");
        result["JObject.notificationItems[0].NotificationRequestItem.amount.value"].Should().Be("2662");
    }

    [Fact]
    public void Simple_Array_To_Dictionary_Test()
    {
        var theObject = new { a = new List<string> { "123", "456" } };

        var result = theObject.ConvertToDictionary();

        result["Object.a[0]"].Should().Be("123");
        result["Object.a[1]"].Should().Be("456");
    }

    public class ObjectWithIgnoreProperty
    {
        public string a { get; set; }

        [JsonIgnore]
        public string b { get; set; }
    }
}