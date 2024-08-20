
namespace GamePlay.Common
{
    public interface IStateDefinition
    {
        int Id { get; }
        
        bool IsTransitable(IStateDefinition definition);
    }
}