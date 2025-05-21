using System.Collections.Generic;
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
        RectTransform SelfTransform { get; }
        RectTransform RootCenter { get; }
        RenderMaskData MaskData { get; }
        IPaintHandle PaintHandle { get; }
        IReadOnlyList<Vector2> InnerPoints { get; }
        float Size { get; }
        
        bool IsInside(Vector2 position);
    }
    
    public static class PaintTargetExtensions
    {
        public static IPaint GetPaint(this IPaintTarget target)
        {
            return target.PaintHandle.Paint.Value;
        }

        public static Vector2 GetNearestCenter(this IPaintTarget target, Vector2 rectPosition)
        {
            var minDistance = float.MaxValue;
            var nearestCenter = Vector2.zero;
            
            foreach (var point in target.InnerPoints)
            {
                var distance = Vector2.Distance(point, rectPosition);
                
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestCenter = point;
                }
            }
            
            return nearestCenter;
        }
        
        public static Vector2 GetNearestCenter(this IPaintTarget target, Vector2 rectPosition, Vector2 exclude)
        {
            var minDistance = float.MaxValue;
            var nearestCenter = Vector2.zero;
            
            foreach (var point in target.InnerPoints)
            {
                if (point == exclude)
                    continue;
                
                var distance = Vector2.Distance(point, rectPosition);
                
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestCenter = point;
                }
            }
            
            return nearestCenter;
        }
    }
}