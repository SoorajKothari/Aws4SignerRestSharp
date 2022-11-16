using RestSharp;
using System.Text;
namespace Aws4SignerRestSharp;
public class Aws4Signer : List<IAws4Signer>
{
    public Aws4Signer(Aws4SignContext signInfo)
    {
        var builder = new StringBuilder();
        Add(new AwsRequestInfoSigner(signInfo, builder));
        Add(new AwsRequestParamSigner(signInfo, builder));
        Add(new AwsRequestHeadersSigner(signInfo, builder));
    }
    public void Sign(RestRequest request) => ForEach(signer => signer.Sign(request));
}