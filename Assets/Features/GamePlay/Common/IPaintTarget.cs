using Features.Services.RenderOptions;
using Internal;
using UnityEngine;

namespace Features.GamePlay
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
    }
    
    public static class PaintTargetExtensions
    {
        public static IPaint GetPaint(this IPaintTarget target)
        {
            return target.PaintHandle.Paint.Value;
        }
    }
}