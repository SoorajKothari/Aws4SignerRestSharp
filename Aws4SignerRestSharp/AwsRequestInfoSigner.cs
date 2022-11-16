using RestSharp;
using System.Text;
namespace Aws4SignerRestSharp;
public class AwsRequestInfoSigner : Aws4SignerBase
{
    private const string Host = nameof(Host);
    private const string DateHeader = "x-amz-date";
    
    public AwsRequestInfoSigner(Aws4SignContext context,StringBuilder builder) : base(context, builder)
    {
    }

    public override void Sign(RestRequest request)
    {
        AddDate(request);
        AddHost(request);
        AddBasicInfo(request);
    }

    private void AddDate(RestRequest request)
    {
        request.AddHeader(DateHeader, Context.AmzDate);
    }

    private void AddHost(RestRequest request)
    {
        request.AddOrUpdateHeader(Host, Context.CompleteUrl.Host);
    }

    private void AddBasicInfo(RestRequest request)
    {
        Builder.Append(request.Method.ToString().ToUpper() + "\n");
        Builder.Append(Context.GetResource() + "\n");
    }
}