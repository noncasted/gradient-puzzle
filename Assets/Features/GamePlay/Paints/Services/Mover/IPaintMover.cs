using System.Collections.Generic;
using Internal;

namespace Features.GamePlay
{
    public interface IPaintMover
    {
        void Start(IReadOnlyLifetime lifetime, IReadOnlyList<IPaintTarget> targets);
    }
}