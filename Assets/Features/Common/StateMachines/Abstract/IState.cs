using Cysharp.Threading.Tasks;

namespace Features.Common.StateMachines.Abstract
{
    public interface IState
    {
        IStateDefinition Definition { get; }

        bool IsAvailable();
        UniTask Enter();
    }
}