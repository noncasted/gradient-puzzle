using UnityEngine;

namespace Global.GameServices
{
    public interface IModifierData
    {
        string Name { get; }
        string Sign { get; }
        Color Color { get; }
    }
}