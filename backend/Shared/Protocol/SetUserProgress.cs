using System;

namespace Shared
{
    public class SetUserProgress
    {
        public class Request
        {
            public Guid UserId { get; set; }
            public string LevelId { get; set; }
        }
    }
}