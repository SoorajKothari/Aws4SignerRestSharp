// See https://aka.ms/new-console-template for more information
using RestSharp;
using Aws4SignerRestSharp;
Console.WriteLine("Hello, World!");
NewCall();
Console.ReadLine();
void NewCall()
{
    var client = new RestClient("https://alibhutto.s3.us-east-1.amazonaws.com");
    var req = new RestRequest("?acl=alibhutto", Method.Get);
    Uri u = new("https://alibhutto.s3.us-east-1.amazonaws.com/?acl=alibhutto");
    Aws4SignContext context = new(u, "us-east-1", "s3", "AKIAWUY7ISKCS6GL5UAE", "E+GX2HhiPJzeQu3vAseZ7gpnmBdq8JHQMaenSq55");
    var nr = new Aws4Signer(context);
    nr.Sign(req);
    var res = client.Execute(req);
}
