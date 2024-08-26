using System;
using UnityEngine;

namespace Features.Services.RenderOptions
{
    [Serializable]
    public class RenderMaskData
    {
        public RenderMaskData(Material area, Material content)
        {
            _area = area;
            _content = content;
        }
        
        private static readonly int Stencil = Shader.PropertyToID("_Stencil");

        [SerializeField] private Material _area;
        [SerializeField] private Material _content;

        public Material Area => _area;
        public Material Content => _content;

        public void SetIndex(int index)
        {
            _area.SetInt(Stencil, index);
            _content.SetInt(Stencil, index);
        }
    }
}