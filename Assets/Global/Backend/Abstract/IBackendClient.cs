using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Global.Backend
{
    public interface IBackendClient
    {
        UniTask<T> Get<T>(IGetRequest request, CancellationToken cancellation);
        UniTask<string> GetRaw(IGetRequest request, CancellationToken cancellation);
        UniTask<T> Post<T>(IPostRequest request, CancellationToken cancellation);
        UniTask<AudioClip> GetAudio(IGetRequest request, AudioType audioType, CancellationToken cancellation);
        UniTask<Texture2D> GetImage(IGetRequest request, CancellationToken cancellation);
    }
}