// See https://aka.ms/new-console-template for more information
using RestSharp;
using Aws4SignerRestSharp;

Test();

void Test()
{
    /*
     Just set these fields and will work fine.
     */
    var client = new RestClient("Your_BaseUrl");
    var request = new RestRequest("Your_Resource", Method.Get);
    Uri completeUrl = new("BaseUrl/Resource?QueryString");
    Aws4SignContext context = new(completeUrl, "Region", "Service", "Access_Key", "Secret_Key");
    var signer = new Aws4Signer(context);
    signer.Sign(request);
    var response = client.Execute(request);
}
