using RestSharp;
using System.Web;
using System.Collections.Specialized;
using System.Text;
namespace Aws4SignerRestSharp
{
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
            var values = new SortedDictionary<string, IEnumerable<string>>(StringComparer.Ordinal);
            foreach (var key in collection.AllKeys)
            {
                if (key == null)
                    values.Add(Uri.EscapeDataString(collection[key]), new[] { $"{Uri.EscapeDataString(collection[key])}=" });
                else
                {
                    var queryValues = collection[key].Split(',').OrderBy(v => v)
                        .Select(v => $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(v)}");
                    values.Add(Uri.EscapeDataString(key), queryValues);
                }
            }
            var queryParams = values.SelectMany(a => a.Value);
            var canonicalQueryParams = string.Join("&", queryParams);
            Builder.Append(canonicalQueryParams + "\n");
        }

    }
}