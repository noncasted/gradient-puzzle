using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Internal;
using UnityEngine;

namespace Services
{
    public class MaskRenderOptions : EnvAsset, IMaskRenderOptions
    {
        [SerializeField] private RenderMaskData[] _datas;

        public RenderMaskData Get(int index)
        {
            return _datas[index];
        }

        public RenderMaskData GetFromBack(int index)
        {
            return _datas[_datas.Length - index - 1];
        }

        protected override void OnReload()
        {
#if UNITY_EDITOR
            var folderPath = UnityEditor.AssetDatabase.GetAssetPath(this).Replace($"{name}.asset", "") + "Materials";
            var guids = UnityEditor.AssetDatabase.FindAssets("t:Material", new[] { folderPath });

            var areaMaterials = new List<Material>();
            var contentMaterials = new List<Material>();

            foreach (var guid in guids)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var material = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(assetPath);

                if (material == null)
                    continue;

                if (material.name.StartsWith("Mask_Area_"))
                    areaMaterials.Add(material);
                else if (material.name.StartsWith("Mask_Content_"))
                    contentMaterials.Add(material);
            }

            if (areaMaterials.Count != contentMaterials.Count)
                throw new Exception();

            areaMaterials.Sort((a, b) => ExtractIndex(a.name).CompareTo(ExtractIndex(b.name)));
            contentMaterials.Sort((a, b) => ExtractIndex(a.name).CompareTo(ExtractIndex(b.name)));

            _datas = new RenderMaskData[areaMaterials.Count];

            for (var i = 0; i < areaMaterials.Count; i++)
                _datas[i] = new RenderMaskData(areaMaterials[i], contentMaterials[i]);

            for (var i = 0; i < _datas.Length; i++)
                _datas[i].SetIndex(i + 1);

            UnityEditor.EditorUtility.SetDirty(this);

            int ExtractIndex(string materialName)
            {
                var match = Regex.Match(materialName, @"(\d+)$");
                return match.Success ? int.Parse(match.Value) : 0;
            }
#endif
        }
    }
}