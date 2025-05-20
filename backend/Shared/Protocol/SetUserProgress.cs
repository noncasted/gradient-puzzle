using System;

namespace Shared
{
    public class SetUserProgress
    {
        public class Request
        {
            public Guid UserId { get; set; }
            public int Stage { get; set; }
            public int Level { get; set; }
        }
    }
}