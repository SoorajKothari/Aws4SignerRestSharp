// See https://aka.ms/new-console-template for more information
public class Aws4SignContext
{
    private const string DateFormat = "yyyyMMddTHHmmssZ";
    public Aws4SignContext(Uri completeUrl, string region, string serviceName, string accessKey, string secretKey)
    {
        CompleteUrl = completeUrl;
        Region = region ?? "us-east-1";
        ServiceName = serviceName;
        AccessKey = accessKey;
        SecretKey = secretKey;
        Now = DateTimeOffset.UtcNow;
        AmzDate = Now.ToString(DateFormat);
    }
    public Uri CompleteUrl { get; }
    public string Region {  get; }
    public string ServiceName { get; }
    public string AccessKey { get; }
    public string SecretKey { get; }
    public DateTimeOffset Now { get; }
    public string AmzDate { get; }
    public string Resource => string.Join("/", CompleteUrl.AbsolutePath.Split('/').Select(Uri.EscapeDataString));
}
