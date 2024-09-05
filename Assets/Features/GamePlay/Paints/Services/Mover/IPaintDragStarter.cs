using System.Collections.Generic;
using Internal;

namespace Features.GamePlay
{
    public interface IPaintDragStarter
    {
        IPaintTarget Selected { get; }
        
        void Start(IReadOnlyLifetime lifetime, IReadOnlyList<IPaintTarget> targets);
    }
}