using UnityEngine;

namespace Global.GameServices
{
    public abstract class IntModifierDefinitionBase : ModifierDefinitionBase
    {
        [SerializeField] private int _baseValue;
        [SerializeField] private int _step;
        
        public override IModifier Create()
        {
            var options = new IntModifierOptions(Id, _step, Data);
            var instance = CreateInstance(options);
            instance.Set(_baseValue);
            return instance;
        }

        protected abstract BaseIntModifier CreateInstance(IntModifierOptions options);
    }
}