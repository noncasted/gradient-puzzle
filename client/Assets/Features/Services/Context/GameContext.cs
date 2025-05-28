using System.Collections.Generic;
using GamePlay.Common;
using GamePlay.Levels;
using GamePlay.Selections;

namespace Services
{
    public class GameContext : IGameContext
    {
        private ILevel _level;
        private ILevelData _levelData;
        private List<IPaintTarget> _targets;

        public ILevel Level => _level;
        public ILevelData LevelData => _levelData;
        public IReadOnlyList<IPaintTarget> Targets => _targets;

        public void Setup(ILevel level, IReadOnlyList<IPaintDock> docks, ILevelData levelData)
        {
            _level = level;
            _levelData = levelData;

            var areas = new List<IArea>(level.Areas);
            _targets = new List<IPaintTarget>();

            foreach (var area in areas)
            {
                if (area.IsAnchor == false)
                    _targets.Add(area);
            }
        }
    }
}