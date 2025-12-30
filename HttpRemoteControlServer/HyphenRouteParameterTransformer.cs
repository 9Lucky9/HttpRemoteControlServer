using System.Text.RegularExpressions;

namespace HttpRemoteControlServer;

public sealed partial class HyphenRouteParameterTransformer : IOutboundParameterTransformer
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
        
        return MyRegex().Replace(inputString, "$1-$2").ToLower();
    }

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex MyRegex();
}