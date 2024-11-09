using VContainer;

namespace Internal
{
    public static class ServiceLoadResultExtensions
    {
        public static T Get<T>(this IServiceScopeLoadResult loadResult)
        {
            return loadResult.Scope.Container.Resolve<T>();
        }
    }
}