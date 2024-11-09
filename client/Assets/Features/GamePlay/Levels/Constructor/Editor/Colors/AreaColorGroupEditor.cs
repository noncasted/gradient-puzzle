using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace GamePlay.Levels
{
    [CustomEditor(typeof(AreaColorGroup))]
    public class AreaColorGroupEditor : OdinEditor
    {
        private AreaColorGroup _group;

        protected override void OnEnable()
        {
            base.OnEnable();
            _group = (AreaColorGroup)target;
            _group.EnablePoints();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _group.DisablePoints();
        }

        private void OnSceneGUI()
        {
            if (_group.Points == null || _group.Points.Count == 0)
                _group.ResetPoints();

            foreach (var (point, color) in _group.Points)
            {
                EditorGUI.BeginChangeCheck();
                point.SetColor(color);
                var newPosition = Handles.PositionHandle(point.transform.position, Quaternion.identity);
                
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(point.transform, "Move Transform Handle");
                    point.transform.position = newPosition;
                }
            }

            foreach (var area in _group.Areas)
                area.DrawContour();
            
            _group.UpdateColors();
        }
    }
}