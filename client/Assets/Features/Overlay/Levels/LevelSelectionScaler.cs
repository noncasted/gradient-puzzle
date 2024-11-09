using UnityEngine;
using UnityEngine.UI;

namespace Overlay
{
    [DisallowMultipleComponent]
    public class LevelSelectionScaler : MonoBehaviour
    {
        [SerializeField] private RectTransform _transform;
        [SerializeField] private GridLayoutGroup _grid;

        public void Rescale(int amount)
        {
            var rows = amount / _grid.constraintCount;
            var rowHeight = _grid.cellSize.y + _grid.spacing.y;

            _transform.sizeDelta = new Vector2(_transform.sizeDelta.x, rows * rowHeight);
        }
    }
}