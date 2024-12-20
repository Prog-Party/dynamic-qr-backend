using Microsoft.Azure.Functions.Worker;
using System.Reflection;

namespace DynamicQR.Api.Extensions;

internal static class FunctionContextExtensions
{
    internal static IEnumerable<Attribute> GetFunctionAttributes(this FunctionContext context)
    {
        // Get the function name from the context
        var functionName = context.FunctionDefinition.Name;

        // Find the method associated with this function
        var method = Assembly.GetExecutingAssembly()
                             .GetTypes()
                             .SelectMany(t => t.GetMethods())
                             .FirstOrDefault(m => m.GetCustomAttribute<FunctionAttribute>()?.Name == functionName);

        if (method != null)
        {
            // Return all attributes applied to the method
            return method.GetCustomAttributes();
        }

        return Enumerable.Empty<Attribute>();
    }
}
