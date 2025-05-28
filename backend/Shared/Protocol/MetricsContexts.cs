using System;

namespace Shared
{
    public interface IMetricContext
    {
        string Endpoint { get; }
    }
    
    public class MetricsContexts
    {
        public const string EndpointGroup = "/metrics";
        
        public class LevelPass : IMetricContext
        {
            public const string Name = "/levelPass";

            public string Endpoint => Name;
            
            public LevelSectionType Section { get; set; }
            public int LevelIndex { get; set; }
            public TimeSpan Time { get; set; }
        }
        
        public class LevelRate : IMetricContext
        {
            public const string Name = "/levelRate";

            public string Endpoint => Name;
            
            public LevelSectionType Section { get; set; }
            public int LevelIndex { get; set; }
            public LevelRating Rate { get; set; }
        }
    }
}