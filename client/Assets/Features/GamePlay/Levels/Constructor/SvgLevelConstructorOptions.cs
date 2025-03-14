using GamePlay.Common;
using Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Levels
{
    [InlineEditor]
    public class SvgLevelConstructorOptions : EnvAsset
    {
        [SerializeField] private float _scale = 1f;
        [SerializeField] private float _renderSimplifyAngle = 10f;
        [SerializeField] private float _systemSimplifyAngle = 10f;
        [SerializeField] private float _pointsDensity = 10f;
        [SerializeField] private string _inkscapePath = @"S:\apps\inkscape\bin\inkscape.exe";
        [SerializeField] private string[] _inkscapeActions;
        [SerializeField] private float _centerCheckDistance;
        [SerializeField] private AreaCenter _centerPrefab;
        
        public float Scale => _scale;
        public float RenderSimplifyAngle => _renderSimplifyAngle;
        public float SystemSimplifyAngle => _systemSimplifyAngle;
        public float PointsDensity => _pointsDensity;
        public string InkscapePath => _inkscapePath;
        public string InkscapeActions => "\"" + string.Join(";", _inkscapeActions) + "\"";
        public float CenterCheckDistance => _centerCheckDistance;
        public AreaCenter CenterPrefab => _centerPrefab;
    }
}