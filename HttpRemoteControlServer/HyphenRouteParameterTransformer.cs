using System.Text.RegularExpressions;

namespace HttpRemoteControlServer;

public sealed class HyphenRouteParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        if (value is null)
        {
            return null;
        }

        var inputString = value.ToString();
        if (string.IsNullOrEmpty(inputString))
        {
            return null;
        }
        
        return Regex.Replace(inputString, "([a-z])([A-Z])", "$1-$2").ToLower();
    }
}