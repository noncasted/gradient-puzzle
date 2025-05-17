using System.Linq;
using Cysharp.Threading.Tasks;
using GamePlay.Levels;
using Internal;
using Services;

namespace GamePlay.Paints
{
    public class PaintTest
    {
        public PaintTest(
            ILevelLoader levelLoader,
            IPaintFactory paintFactory,
            LevelOptions levelOptions)
        {
            _levelLoader = levelLoader;
            _paintFactory = paintFactory;
            _levelOptions = levelOptions;
        }

        private readonly ILevelLoader _levelLoader;
        private readonly IPaintFactory _paintFactory;
        private readonly LevelOptions _levelOptions;

        public async UniTask Run(IReadOnlyLifetime lifetime)
        {
            var levelData = new LevelData(_levelOptions, 0);
            var level = _levelLoader.Load(levelData);

            var area = level.Areas.First();
            var paint = await _paintFactory.Create(lifetime, area.Color);
            paint.Construct(area.Color);

            var testState = paint.GameObject.GetComponentInChildren<PaintTestState>();
            await testState.Run(lifetime);
        }
    }
}