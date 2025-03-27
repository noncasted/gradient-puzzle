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
            LevelConfiguration levelConfiguration)
        {
            _levelLoader = levelLoader;
            _paintFactory = paintFactory;
            _levelConfiguration = levelConfiguration;
        }

        private readonly ILevelLoader _levelLoader;
        private readonly IPaintFactory _paintFactory;
        private readonly LevelConfiguration _levelConfiguration;

        public async UniTask Run(IReadOnlyLifetime lifetime)
        {
            var level = _levelLoader.Load(_levelConfiguration);

            var area = level.Areas.First();
            var paint = await _paintFactory.Create(lifetime, area.Color);
            paint.Construct(area.Color);

            var testState = paint.GameObject.GetComponentInChildren<PaintTestState>();
            await testState.Run(lifetime);
        }
    }
}