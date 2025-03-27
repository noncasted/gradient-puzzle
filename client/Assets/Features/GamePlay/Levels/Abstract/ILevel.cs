using System.Collections.Generic;
using Internal;

namespace GamePlay.Levels
{
    public interface ILevel
    {
        IReadOnlyList<IArea> Areas { get; }

        void Setup(IReadOnlyLifetime lifetime);
    }
}