using System.Collections.Generic;
using GamePlay.Common;
using GamePlay.Levels;
using GamePlay.Selections;

namespace Services
{
    public interface IGameContext
    {
        ILevel Level { get; }
        IReadOnlyList<IPaintTarget> Targets { get; }

        void Setup(ILevel level, IReadOnlyList<IPaintDock> docks);
    }
}