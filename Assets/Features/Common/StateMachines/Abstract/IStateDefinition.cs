
namespace Features.Common.StateMachines.Abstract
{
    public interface IStateDefinition
    {
        int Id { get; }
        
        bool IsTransitable(IStateDefinition definition);
    }
}