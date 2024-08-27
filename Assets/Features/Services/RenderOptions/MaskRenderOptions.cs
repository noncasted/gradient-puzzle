using System;
using System.Collections.Generic;
using Internal;
using UnityEngine;
#if UNITY_EDITOR
using System.Text.RegularExpressions;
using UnityEditor;
#endif

namespace Features.Services.RenderOptions
{
    public class MaskRenderOptions : EnvAsset, IMaskRenderOptions
    {
        [SerializeField] private RenderMaskData[] _datas;

        public RenderMaskData Get(int index)
        {
            return _datas[index];
        }

        protected override void OnReload()
        {
#if UNITY_EDITOR
            var folderPath = AssetDatabase.GetAssetPath(this).Replace($"{name}.asset", "") + "Materials";
            var guids = AssetDatabase.FindAssets("t:Material", new[] { folderPath });

            var areaMaterials = new List<Material>();
            var contentMaterials = new List<Material>();

            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

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

            EditorUtility.SetDirty(this);

            int ExtractIndex(string materialName)
            {
                var match = Regex.Match(materialName, @"(\d+)$");
                return match.Success ? int.Parse(match.Value) : 0;
            }
#endif
        }
    }
}