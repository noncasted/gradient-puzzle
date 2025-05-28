using Internal;

namespace Global.Metrics
{
    public static class MetricsExtensions
    {
        public static IScopeBuilder AddMetrics(this IScopeBuilder builder)
        {
            builder.Register<Metrics>()
                .WithScopeLifetime()
                .As<IMetrics>();

            return builder;
        }
    }
}