using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePlay.Levels
{
    [Serializable]
    public class AreaOutlineData
    {
        [SerializeField] private List<Vector2> _vertices;
        [SerializeField] private List<int> _triangles;

        public List<Vector2> Vertices => _vertices;
        public List<int> Triangles => _triangles;

        public AreaOutlineData(List<Vector2> vertices, List<int> triangles)
        {
            _vertices = vertices;
            _triangles = triangles;
        }

        public AreaOutlineData()
        {
            _vertices = new List<Vector2>();
            _triangles = new List<int>();
        }
    }
}