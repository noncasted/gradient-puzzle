using Cysharp.Threading.Tasks;

namespace Common.StateMachines
{
    public interface ILocalState
    {
        IStateDefinition Definition { get; }

        bool IsAvailable();
        UniTask Enter();
    }
}