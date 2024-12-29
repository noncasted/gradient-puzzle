using Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Levels
{
    [InlineEditor]
    public class SvgLevelConstructorOptions : EnvAsset
    {
        [SerializeField] private float _pointsDensity = 10f;
        [SerializeField] private string _inkscapePath = @"S:\apps\inkscape\bin\inkscape.exe";
        [SerializeField] private string[] _inkscapeActions;

        public float PointsDensity => _pointsDensity;
        public string InkscapePath => _inkscapePath;
        public string InkscapeActions => "\"" + string.Join(";", _inkscapeActions) + "\"";
    }
}