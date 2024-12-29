using Shapes;
using UnityEngine;

namespace GamePlay.Levels
{
    [DisallowMultipleComponent]
    public class AreaOutlineRenderer : MonoBehaviour
    {
        [SerializeField] private AreaShapeRenderer _shapeRenderer;
        [SerializeField] private AreaRendererOptions _options;
        [SerializeField] private Polyline _line;
        
        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _line.points.Clear();
            
            foreach (var point in _shapeRenderer.Data.Vertices)
                _line.points.Add(new PolylinePoint(point));
        }
    }
}