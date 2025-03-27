namespace Global.Backend
{
    public class BackendClient : IBackendClient
    {
        public BackendClient(IBackendGetGateway get, IBackendPostGateway post, IBackendMediaGateway media)
        {
            Get = get;
            Post = post;
            Media = media;
        }

        public IBackendGetGateway Get { get; }
        public IBackendPostGateway Post { get; }
        public IBackendMediaGateway Media { get; }
    }
}