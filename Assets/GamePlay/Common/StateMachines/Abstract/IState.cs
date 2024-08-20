using Cysharp.Threading.Tasks;

namespace GamePlay.Common
{
    public interface IState
    {
        IStateDefinition Definition { get; }

        bool IsAvailable();
        UniTask Enter();
    }
}