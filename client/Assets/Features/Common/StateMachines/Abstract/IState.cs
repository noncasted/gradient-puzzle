namespace Common.StateMachines
{
    public interface IState
    {
        IStateDefinition Definition { get; }
    }
}