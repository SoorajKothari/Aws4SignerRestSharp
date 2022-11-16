using RestSharp;
using System.Security.Cryptography;
using System.Text;
namespace Aws4SignerRestSharp;
public class AwsRequestHeadersSigner : Aws4SignerBase
{
    private const string EMPTY_STRING_HASH = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
    private const string ContentHeader = "x-amz-content-sha256";
    private const string DateStampFormat = "yyyyMMdd";
    private const string ALGORITHM = "AWS4-HMAC-SHA256";
    private const string Aws4_Request = "aws4_request";
    private const string Credentials = "Credential=";
    private const string Authorization = "Authorization";
    private const string SignedHeaders = "SignedHeaders=";
    private const string Signature = "Signature=";
    private const string Aws4 = "AWS4";
    public AwsRequestHeadersSigner(Aws4SignContext context, StringBuilder builder) : base(context, builder)
    {
    }

    public override void Sign(RestRequest request)
    {
        string signedcontent = SignContent(request);
        string signedHeaders = SignHeaders(request);
        Builder.Append(signedHeaders + "\n");
        Builder.Append(signedcontent);
        AddAuthHeader(signedHeaders, request);
    }

    private void AddAuthHeader(string signedHeaders, RestRequest request)
    {
        string dateStamp = Context.Now.ToString(DateStampFormat);
        var credentialScope = $"{dateStamp}/{Context.Region}/{Context.ServiceName}/{Aws4_Request}";
        var stringToSign = $"{ALGORITHM}\n{Context.AmzDate}\n{credentialScope}\n" + ComputeHash(Encoding.UTF8.GetBytes(Builder.ToString()));
        var signingKey = GetSignatureKey(dateStamp);
        var signature = ToHexString(GetHmacSha256(signingKey, stringToSign));
        var value = $"{ALGORITHM} {Credentials}{Context.AccessKey}/{credentialScope}, {SignedHeaders}{signedHeaders}, {Signature}{signature}";
        request.AddOrUpdateHeader(Authorization, value);
    }

    private string SignContent(RestRequest request)
    {
        var contentHeader = request.Parameters.FirstOrDefault(pm => pm.Type is ParameterType.RequestBody);
        var payloadHash = EMPTY_STRING_HASH;
        if (contentHeader is not null)
        {
            string? content = contentHeader.Value?.ToString();
            byte[] data = Encoding.UTF8.GetBytes(content!);
            payloadHash = ComputeHash(data);
        }
        request.AddOrUpdateHeader(ContentHeader, payloadHash);
        return payloadHash;
    }

    private string SignHeaders(RestRequest request)
    {
        var signedHeadersList = new List<string>();
        GetHeaders(request).ToList().ForEach(Sign);

        void Sign(Parameter header)
        {
            Builder.Append(header.Name.ToLowerInvariant());
            Builder.Append(":");
            Builder.Append(string.Join(",", header.Value));
            Builder.Append("\n");
            signedHeadersList.Add(header.Name.ToLowerInvariant());
        }
        Builder.Append("\n");
        return string.Join(";", signedHeadersList);
    }

    private IEnumerable<Parameter> GetHeaders(RestRequest request)
    {
        return request.Parameters.Where(x => x.Type is ParameterType.HttpHeader)
            .OrderBy(a => a.Name.ToLowerInvariant(), StringComparer.OrdinalIgnoreCase);
    }

    private static byte[] GetHmacSha256(byte[] key, string data)
    {
        using var hashAlgorithm = new HMACSHA256(key);
        return hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(data));
    }

    private byte[] GetSignatureKey(string dateStamp)
    {
        var kSecret = Encoding.UTF8.GetBytes(Aws4 + Context.SecretKey);
        var kDate = GetHmacSha256(kSecret, dateStamp);
        var kRegion = GetHmacSha256(kDate, Context.Region);
        var kService = GetHmacSha256(kRegion, Context.ServiceName);
        return GetHmacSha256(kService, Aws4_Request);
    }
}