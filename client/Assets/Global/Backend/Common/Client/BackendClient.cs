namespace Global.Backend
{
    public class BackendClient : IBackendClient
    {
        public BackendClient(
            IBackendGetGateway get,
            IBackendPostGateway post,
            IBackendMediaGateway media, 
            BackendOptions options)
        {
            Get = get;
            Post = post;
            Media = media;
            Url = options.Url;
        }

        public string Url { get; }
        public IBackendGetGateway Get { get; }
        public IBackendPostGateway Post { get; }
        public IBackendMediaGateway Media { get; }
    }
}