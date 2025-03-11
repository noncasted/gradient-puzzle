using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Internal
{
    public class ServiceScopeSceneLoader : ISceneLoader
    {
        public ServiceScopeSceneLoader(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        private readonly List<ILoadedScene> _results = new();
        private readonly ISceneLoader _sceneLoader;

        public IReadOnlyList<ILoadedScene> Results => _results;

        public async UniTask<ILoadedScene> Load(SceneData sceneAsset)
        {
            var result = await _sceneLoader.Load(sceneAsset);

            _results.Add(result);

            return result;
        }
    }
}