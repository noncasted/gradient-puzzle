using Features.Common.StateMachines.Abstract;
using Internal;
using UnityEngine;

namespace Features.Common.StateMachines
{
    public abstract class StateDefinition : EnvAsset, IStateDefinition
    {
        [SerializeField] private StateDefinition[] _transitions;

        public bool IsTransitable(IStateDefinition definition)
        {
            foreach (var transition in _transitions)
            {
                if (definition.Id == transition.Id)
                    return true;
            }

            return false;
        }
    }
}