using Cysharp.Threading.Tasks;
using Global.Backend;
using Internal;
using Shared;

namespace Global.Metrics
{
    public class Metrics : IMetrics
    {
        public Metrics(
            IReadOnlyLifetime scopeLifetime,
            IBackendClient backend)
        {
            _scopeLifetime = scopeLifetime;
            _backend = backend;
        }

        private readonly IReadOnlyLifetime _scopeLifetime;
        private readonly IBackendClient _backend;
        
        public UniTask Send(IMetricContext context)
        {
            var url = _backend.Url + MetricsContexts.EndpointGroup + context.Endpoint;
            return _backend.PostJson(_scopeLifetime, url, context);
        }
    }
}