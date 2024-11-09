using System;
using UnityEngine;

namespace Global.GameServices
{
    [Serializable]
    public class ModifierData : IModifierData
    {
        [SerializeField] private string _name;
        [SerializeField] private string _sign;
        [SerializeField] private Color _color = Color.white;

        public string Name => _name;
        public string Sign => _sign;
        public Color Color => _color;
    }
}