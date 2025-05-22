using System.Collections.Generic;
using System.Linq;
using GamePlay.Levels;
using Internal;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public interface IBackground
    {
        void ToGame(IReadOnlyDictionary<Vector2, Color> colors);
        void ToSections();
        void ToLevels();
    }

    [DisallowMultipleComponent]
    public class Background : BaseMeshEffect, ISceneService, IBackground
    {
        [SerializeField] private Color _levelTargetColor;
        [SerializeField] private float _levelColorLerp = 0.5f;
        
        [SerializeField] private float _switchTime = 2f;
        
        [SerializeField] private Color[] _sections;
        [SerializeField] private Color[] _levels;

        [SerializeField] private Color[] _currentColors;
        [SerializeField] private Color[] _targetColors;

        private float _timer;

        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<IBackground>();
        }

        public void ToGame(IReadOnlyDictionary<Vector2, Color> colors)
        {
            var (bottomLeft, topRight) = colors.Keys.ToList().GetBounds();
            var topLeft = new Vector2(bottomLeft.x, topRight.y);
            var bottomRight = new Vector2(topRight.x, bottomLeft.y);

            _targetColors[0] = GetNearestTo(topLeft);
            _targetColors[1] = GetNearestTo(topRight);
            _targetColors[2] = GetNearestTo(bottomRight);
            _targetColors[3] = GetNearestTo(bottomLeft);

            _timer = 0f;

            Color GetNearestTo(Vector2 position)
            {
                var nearest = colors.Keys
                    .OrderBy(k => Vector2.Distance(k, position))
                    .First();

                var color = colors[nearest];
                color = Color.Lerp(color, _levelTargetColor, _levelColorLerp);
                return color;
            }
        }

        public void ToSections()
        {
            _targetColors[0] = _sections[0];
            _targetColors[1] = _sections[1];
            _targetColors[2] = _sections[2];
            _targetColors[3] = _sections[3];

            _timer = 0f;
        }

        public void ToLevels()
        {
            _targetColors[0] = _levels[0];
            _targetColors[1] = _levels[1];
            _targetColors[2] = _levels[2];
            _targetColors[3] = _levels[3];

            _timer = 0f;
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            var rect = graphic.rectTransform.rect;
            var localPositionMatrix =
                BackgroundUtils.LocalPositionMatrix(rect, Vector2.right);

            var vertex = default(UIVertex);

            for (var i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);
                var normalizedPosition = localPositionMatrix * vertex.position;

                vertex.color *= BackgroundUtils.Bilerp(
                    _currentColors[3],
                    _currentColors[2],
                    _currentColors[0],
                    _currentColors[1],
                    normalizedPosition);

                vh.SetUIVertex(vertex, i);
            }
        }

        private void Update()
        {
            if (_timer < 1f)
                graphic.SetAllDirty();

            _timer += Time.deltaTime;
            var progress = Mathf.Clamp01(_timer / _switchTime);

            _currentColors[0] = Color.Lerp(_currentColors[0], _targetColors[0], progress);
            _currentColors[1] = Color.Lerp(_currentColors[1], _targetColors[1], progress);
            _currentColors[2] = Color.Lerp(_currentColors[2], _targetColors[2], progress);
            _currentColors[3] = Color.Lerp(_currentColors[3], _targetColors[3], progress);
        }

        [Button]
        private void PushSections()
        {
            Copy(_sections, _targetColors);
        }

        [Button]
        private void PushLevels()
        {
            Copy(_levels, _targetColors);
        }

        [Button]
        private void CopyTargetToSections()
        {
            Copy(_targetColors, _sections);
        }

        [Button]
        private void CopyTargetToLevels()
        {
            Copy(_targetColors, _levels);
        }

        private void Copy(Color[] from, Color[] to)
        {
            for (var i = 0; i < from.Length; i++)
                to[i] = from[i];
            
            _timer = 1;
            graphic.SetAllDirty();
        }
    }
}