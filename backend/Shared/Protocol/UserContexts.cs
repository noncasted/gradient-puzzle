using System;
using System.Collections.Generic;

namespace Shared
{
    public class UserContexts
    {
        public class GetProgress
        {
            public const string Endpoint = "getProgress";
            
            public class Request
            {
                public Guid UserId { get; set; }
            }

            public class Response
            {
                public Dictionary<LevelSectionType, int> PassedLevels { get; set; }
            }
        }
        
        public class SetProgress
        {
            public const string Endpoint = "setLevelPassed";
            
            public Guid UserId { get; set; }
            public LevelSectionType Section { get; set; }
            public int Level { get; set; }
        }
    }
}