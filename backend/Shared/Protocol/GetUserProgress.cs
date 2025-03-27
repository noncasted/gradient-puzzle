using System;
using System.Collections.Generic;

namespace Shared
{
    public class GetUserProgress
    {
        public class Request
        {
            public Guid UserId { get; set; }
        }

        public class Response
        {
            public IReadOnlyList<string> PassedLevels { get; set; }
        }
    }
}