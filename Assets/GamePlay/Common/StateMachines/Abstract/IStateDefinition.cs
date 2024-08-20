namespace Common.StateMachines
{
    public interface IStateDefinition
    {
        int Id { get; }
        
        bool IsTransitable(IStateDefinition definition);
    }
}