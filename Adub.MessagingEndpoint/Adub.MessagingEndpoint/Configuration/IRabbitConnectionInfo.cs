namespace Adub.MessagingEndpoint.Configuration
{
    public interface IRabbitConnectionInfo
    {
        string Host { get; }
        string Password { get; }
        string Port { get; }
        string User { get; }
        string VHost { get; }

        IRabbitConnectionInfo Initialize(string serviceName);
    }
}