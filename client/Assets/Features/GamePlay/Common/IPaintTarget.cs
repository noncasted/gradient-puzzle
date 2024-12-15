using GamePlay.Paints;
using Internal;
using Services;
using UnityEngine;

namespace GamePlay.Common
{
    public interface IPaintTarget
    {
        IViewableProperty<bool> IsTouched { get; }
        Vector2 Position { get; }
        RectTransform Transform { get; }
        RectTransform CenterTransform { get; }
        RenderMaskData MaskData { get; }
        IPaintHandle PaintHandle { get; }
        
        bool IsInside(Vector2 position);
        float GetMinDistanceToBorder(Vector2 position);
    }
    
    public static class PaintTargetExtensions
    {
        public static IPaint GetPaint(this IPaintTarget target)
        {
            return target.PaintHandle.Paint.Value;
        }
    }
}