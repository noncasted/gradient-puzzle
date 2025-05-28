using System;

namespace Shared
{
    public interface IMetricContext
    {
        string Endpoint { get; }
    }
    
    public class MetricsContexts
    {
        public const string Endpoint = "/metrics";
        
        public class Level : IMetricContext
        {
            public const string Name = "/level";

            public string Endpoint => Name;
            
            public LevelSectionType Section { get; set; }
            public int LevelIndex { get; set; }
            public TimeSpan Time { get; set; }
        }
    }
}