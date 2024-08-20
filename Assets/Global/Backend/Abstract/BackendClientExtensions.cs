using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Global.Backend
{
    public static class BackendClientExtensions
    {
        public static UniTask<T> Get<T>(
            this IBackendClient client,
            string uri,
            bool withLogs,
            CancellationToken cancellation,
            params IRequestHeader[] headers)
        {
            var request = new GetRequest(uri, withLogs, headers);

            return client.Get<T>(request, cancellation);
        }

        public static UniTask<string> GetRaw(
            this IBackendClient client,
            string uri,
            bool withLogs,
            CancellationToken cancellation,
            params IRequestHeader[] headers)
        {
            var request = new GetRequest(uri, withLogs, headers);

            return client.GetRaw(request, cancellation);
        }

        public static UniTask<TResponse> Post<TResponse, TBody>(
            this IBackendClient client,
            string uri,
            TBody body,
            bool withLogs,
            CancellationToken cancellation,
            params IRequestHeader[] headers)
        {
            var bodyJson = JsonConvert.SerializeObject(body);
            var request = new PostRequest(uri, bodyJson, withLogs, headers);

            return client.Post<TResponse>(request, cancellation);
        }

        public static UniTask<TResponse> Post<TResponse>(
            this IBackendClient client,
            string uri,
            bool withLogs,
            CancellationToken cancellation,
            params IRequestHeader[] headers)
        {
            var request = new PostRequest(uri, null, withLogs, headers);

            return client.Post<TResponse>(request, cancellation);
        }

        public static UniTask<AudioClip> GetAudio(
            this IBackendClient client,
            string uri,
            bool withLogs,
            CancellationToken cancellation,
            params IRequestHeader[] headers)
        {
            var request = new GetRequest(uri, withLogs, headers);

            return client.GetAudio(request, AudioType.MPEG, cancellation);
        }

        public static UniTask<Texture2D> GetImage(
            this IBackendClient client,
            string uri,
            bool withLogs,
            CancellationToken cancellation,
            params IRequestHeader[] headers)
        {
            var request = new GetRequest(uri, withLogs, headers);

            return client.GetImage(request, cancellation);
        }
    }
}