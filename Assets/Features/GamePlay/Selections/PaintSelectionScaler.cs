using System;
using Internal;
using UnityEngine;
using UnityEngine.UI;

namespace Features.GamePlay
{
    [DisallowMultipleComponent]
    public class PaintSelectionScaler : MonoBehaviour, IPaintSelectionScaler, ISceneService
    {
        [SerializeField] private GridLayoutGroup _grid;

        [SerializeField] private PaintSelectionScaleDictionary _scale;

        private float _size;

        public float Size => _size;

        public void Scale(int colorsAmount)
        {
            var size = _scale[colorsAmount];
            _grid.cellSize = new Vector2(size, size);
            _size = size;
        }

        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<IPaintSelectionScaler>();
        }
    }
    
    [Serializable]
    public class PaintSelectionScaleDictionary : SerializableDictionary<int, float> { }
}