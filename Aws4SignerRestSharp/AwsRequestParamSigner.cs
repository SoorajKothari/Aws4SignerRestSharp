using RestSharp;
using System.Web;
using System.Collections.Specialized;
using System.Text;
namespace Aws4SignerRestSharp;
#nullable enable
public class AwsRequestParamSigner : Aws4SignerBase
{
    public AwsRequestParamSigner(Aws4SignContext context, StringBuilder builder) : base(context, builder)
    {
    }

    public override void Sign(RestRequest request)
    {
        var querystring = HttpUtility.ParseQueryString(Context.CompleteUrl.Query);
        SignQueryParams(querystring);
    }

    private void SignQueryParams(NameValueCollection collection)
    {
        var parsedValues = new SortedDictionary<string, IEnumerable<string>>(StringComparer.Ordinal);
        foreach (var key in collection.AllKeys)
            Process(key);

        void Process(string? key)
        {
            string value = collection[key]!;
            if (key == null)
                parsedValues.Add(Uri.EscapeDataString(value), new[] { $"{Uri.EscapeDataString(value)}=" });
            else
                parsedValues.Add(Uri.EscapeDataString(key), GetValues(key, value));
        }
        Add(parsedValues);
    }

    private void Add(SortedDictionary<string, IEnumerable<string>> parsedValues)
    {
        var queryParams = parsedValues.SelectMany(a => a.Value);
        var canonicalQueryParams = string.Join("&", queryParams);
        Builder.Append(canonicalQueryParams + "\n");
    }

    private static IEnumerable<string> GetValues(string key, string value)
    {
        return value.Split(',').OrderBy(v => v)
            .Select(v => $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(v)}");
    }
}