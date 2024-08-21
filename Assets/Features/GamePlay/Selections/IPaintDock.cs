using UnityEngine;

namespace Features.GamePlay
{
    public interface IPaintDock : IPaintTarget
    {
        RectTransform Transform { get; }
        float Size { get; }
    }
}