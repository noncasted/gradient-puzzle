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
        IReadOnlyList<AreaCenter> Centers { get; }
        
        bool IsInside(Vector2 position);
        float GetMinDistanceToBorder(Vector2 position);
    }
    
    public static class PaintTargetExtensions
    {
        public static IPaint GetPaint(this IPaintTarget target)
        {
            return target.PaintHandle.Paint.Value;
        }

        public static AreaCenter GetNearestCenter(this IPaintTarget target, Transform from)
        {
            var minDistance = float.MaxValue;
            AreaCenter nearestCenter = null;
            
            foreach (var center in target.Centers)
            {
                var distance = Vector2.Distance(center.Transform.position, from.position);
                
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestCenter = center;
                }
            }
            
            return nearestCenter;
        }
    }
}