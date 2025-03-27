using System.Collections.Generic;
using GamePlay.Common;
using Internal;

namespace GamePlay.Paints
{
    public interface IPaintDragStarter
    {
        IPaintTarget Selected { get; }
        
        void Start(IReadOnlyLifetime lifetime, IReadOnlyList<IPaintTarget> targets);
    }
}