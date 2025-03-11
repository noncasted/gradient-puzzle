using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Internal
{
    public class SceneLoader : ISceneLoader
    {
        public async UniTask<ILoadedScene> Load(SceneData sceneAsset)
        {
            var scene = await Addressables.LoadSceneAsync(sceneAsset.Value, LoadSceneMode.Additive).ToUniTask();
            SceneManager.SetActiveScene(scene.Scene);
            return new LoadedScene(scene);
        }
    }
}