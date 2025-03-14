using System.Collections.Generic;
using GamePlay.Common;
using GamePlay.Levels;
using GamePlay.Selections;

namespace Services
{
    public class GameContext : IGameContext
    {
        private ILevel _level;
        private List<IPaintTarget> _targets;

        public ILevel Level => _level;
        public IReadOnlyList<IPaintTarget> Targets => _targets;

        public void Setup(ILevel level, IReadOnlyList<IPaintDock> docks)
        {
            _level = level;
            
            _targets = new List<IPaintTarget>();
            _targets.AddRange(docks);
            _targets.AddRange(level.Areas);
        }
    }
}