using System.Collections.Generic;
using System.Linq;
using Internal;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public interface IBackground
    {
        void SetGame(IReadOnlyDictionary<Vector2, Color> colors);
        void ToGame();
        void ToSections();
        void ToLevels();
        void ToSettings();
        void ToCompletion();
    }

    [DisallowMultipleComponent]
    public class Background : BaseMeshEffect, ISceneService, IBackground
    {
        [SerializeField] private Color _levelTargetColor;
        [SerializeField] private float _levelColorLerp = 0.5f;

        [SerializeField] private float _switchTime = 2f;

        [SerializeField] private Color[] _sections;
        [SerializeField] private Color[] _levels;

        private readonly Color[] _currentColors = new Color[4];
        private readonly Color[] _targetColors = new Color[4];
        private readonly Color[] _gameColors = new Color[4];

        private float _timer;

        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<IBackground>();
        }

        public void SetGame(IReadOnlyDictionary<Vector2, Color> colors)
        {
            var bottomLeft = new Vector2(-540, -540);
            var topRight = new Vector2(540, 540);
            var topLeft = new Vector2(-540, 540);
            var bottomRight = new Vector2(540, -540);

            _gameColors[0] = GetNearestTo(topLeft);
            _gameColors[1] = GetNearestTo(topRight);
            _gameColors[2] = GetNearestTo(bottomRight);
            _gameColors[3] = GetNearestTo(bottomLeft);

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

        public void ToGame()
        {
            SwitchTarget(_gameColors);
        }

        public void ToSections()
        {
            SwitchTarget(_sections);
        }

        public void ToLevels()
        {
            SwitchTarget(_levels);
        }

        public void ToSettings()
        {
            SwitchTarget(_sections);
        }

        public void ToCompletion()
        {
            SwitchTarget(_sections);
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
            ForceCopy(_sections, _targetColors);
        }

        [Button]
        private void PushLevels()
        {
            ForceCopy(_levels, _targetColors);
        }

        [Button]
        private void CopyTargetToSections()
        {
            ForceCopy(_targetColors, _sections);
        }

        [Button]
        private void CopyTargetToLevels()
        {
            ForceCopy(_targetColors, _levels);
        }

        private void SwitchTarget(Color[] from)
        {
            for (var i = 0; i < from.Length; i++)
                _targetColors[i] = from[i];

            _timer = 0f;
        }

        private void ForceCopy(Color[] from, Color[] to)
        {
            for (var i = 0; i < from.Length; i++)
                to[i] = from[i];

            _timer = 1;
            graphic.SetAllDirty();
        }
    }
}