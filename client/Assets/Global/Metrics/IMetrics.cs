using Cysharp.Threading.Tasks;
using Shared;

namespace Global.Metrics
{
    public interface IMetrics
    {
        UniTask Send(IMetricContext context);
    }
}