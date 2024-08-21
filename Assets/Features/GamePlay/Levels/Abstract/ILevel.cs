using System.Collections.Generic;
using Internal;

namespace Features.GamePlay
{
    public interface ILevel
    {
        IReadOnlyList<IArea> Areas { get; }

        void Setup(IReadOnlyLifetime lifetime);
    }
}