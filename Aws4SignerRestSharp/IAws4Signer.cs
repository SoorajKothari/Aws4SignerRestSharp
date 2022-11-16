// See https://aka.ms/new-console-template for more information
using RestSharp;
public interface IAws4Signer
{
    void Sign(RestRequest request);
}
