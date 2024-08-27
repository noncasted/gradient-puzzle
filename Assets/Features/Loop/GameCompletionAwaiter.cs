using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Features.GamePlay;
using Internal;

namespace Features
{
    public class GameCompletionAwaiter
    {
        public GameCompletionAwaiter(IReadOnlyList<IArea> areas, IReadOnlyLifetime lifetime)
        {
            _areas = areas;
            _lifetime = lifetime;
        }


        private readonly IReadOnlyList<IArea> _areas;
        private readonly IReadOnlyLifetime _lifetime;

        public async UniTask Await()
        {
            var completion = new UniTaskCompletionSource();
            _lifetime.Listen(() => completion.TrySetCanceled());

            foreach (var area in _areas)
                area.IsCompleted.View(_lifetime, CheckCompletion);

            await completion.Task;

            return;

            void CheckCompletion()
            {
                foreach (var area in _areas)
                {
                    if (area.IsCompleted.Value == false)
                        return;
                }

                completion.TrySetResult();
            }
        }
    }
}