namespace FraudexPy.Api.Configurations;

public class MinioSettings
{
    public const string SectionName = "MinioSettings";
    public string Endpoint { get; set; } = "localhost:9000";
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = "fraudex-documents";
    public bool UseSSL { get; set; } = false;
}
