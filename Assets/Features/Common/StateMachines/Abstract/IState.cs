namespace Features.Common.StateMachines.Abstract
{
    public interface IState
    {
        IStateDefinition Definition { get; }
    }
}