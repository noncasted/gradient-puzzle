namespace Global.Backend
{
    public interface IBackendClient
    {
        string Url { get; }
        
        IBackendGetGateway Get { get; }
        IBackendPostGateway Post { get; }
        IBackendMediaGateway Media { get; }
    }
}