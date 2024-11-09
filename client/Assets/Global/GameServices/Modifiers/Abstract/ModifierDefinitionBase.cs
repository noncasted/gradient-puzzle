using Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Global.GameServices
{
    [InlineEditor]
    public abstract class ModifierDefinitionBase : EnvAsset
    {
        [SerializeField] private ModifierData _data;

        public IModifierData Data => _data;
        
        public abstract IModifier Create();
    }
}