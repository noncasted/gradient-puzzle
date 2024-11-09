using UnityEngine;

namespace Global.GameServices
{
    public abstract class FloatModifierDefinitionBase : ModifierDefinitionBase
    {
        [SerializeField] private float _baseValue;
        [SerializeField] private float _step;

        public override IModifier Create()
        {
            var options = new FloatModifierOptions(Id, _step, Data);
            var instance = CreateInstance(options);
            instance.Set(_baseValue);
            return instance;
        }

        protected abstract BaseFloatModifier CreateInstance(FloatModifierOptions options);
    }
}