using Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Levels
{
    [InlineEditor]
    public class LevelConstructorOptions : EnvAsset
    {
        [SerializeField] private string _inkscapePath = @"S:\apps\inkscape\bin\inkscape.exe";
        [SerializeField] private string[] _inkscapeActions;
        [SerializeField] private Area _areaPrefab;

        [SerializeField] private LevelCreateOptions.Geometry _geometry;
        [SerializeField] private LevelCreateOptions.Optimization _render;
        
        public string InkscapePath => _inkscapePath;
        public string InkscapeActions => "\"" + string.Join(";", _inkscapeActions) + "\"";
        public Area AreaPrefab => _areaPrefab;
        
        public LevelCreateOptions.Geometry Geometry => _geometry;
        public LevelCreateOptions.Optimization Render => _render;
    }
}