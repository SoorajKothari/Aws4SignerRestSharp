// See https://aka.ms/new-console-template for more information
using RestSharp;
using System.Security.Cryptography;
using System.Text;
public abstract class Aws4SignerBase : IAws4Signer
{
    protected Aws4SignContext Context { get; }
    protected StringBuilder Builder { get; }
    protected Aws4SignerBase(Aws4SignContext context,StringBuilder builder)
    {
        Context = context;
        Builder = builder;
    }
    public abstract void Sign(RestRequest request);
    protected string ComputeHash(byte[] bytesToHash)
    {
        using var sha256 = SHA256.Create();
        var result = sha256.ComputeHash(bytesToHash);
        return ToHexString(result);
    }
    protected string ToHexString(IReadOnlyCollection<byte> items)
    {
        var str = new StringBuilder(items.Count * 2);
        foreach (var b in items)
            str.AppendFormat("{0:x2}", b);
        return str.ToString();
    }
}