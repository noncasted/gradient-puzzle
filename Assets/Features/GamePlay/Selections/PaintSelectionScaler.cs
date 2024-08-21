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
        [SerializeField] private RectTransform _transform;
        [SerializeField] private PaintSelectionScaleDictionary _scale;

        private float _dockSize;

        public float DockSize => _dockSize;
        public int AreaSize => (int)_transform.sizeDelta.x;

        public void Create(IScopeBuilder builder)
        {
            builder.RegisterComponent(this)
                .As<IPaintSelectionScaler>();
        }
        
        public void Scale(int colorsAmount)
        {
            _grid.enabled = true;
            var size = _scale[colorsAmount];
            _grid.cellSize = new Vector2(size, size);
            _dockSize = size;
        }
        
        public void Disable()
        {
            _grid.enabled = false;
        }
    }
    
    [Serializable]
    public class PaintSelectionScaleDictionary : SerializableDictionary<int, float> { }
}