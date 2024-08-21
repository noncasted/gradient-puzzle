using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.GamePlay
{
    [DisallowMultipleComponent]
    public class LevelConstructor : MonoBehaviour
    {
        [SerializeField] private Area _prefab;
        [SerializeField] private Gradient _gradient;
        [SerializeField] private List<Sprite> _sprites;
        [SerializeField] private RectTransform _colorsTransform;
        [SerializeField] private RectTransform _selfTransform;

        [SerializeField] private float _endIncrement;

        [Button]
        public void Construct()
        {
            var areas = GetComponentsInChildren<Area>(true);
            var colors = GetComponentsInChildren<LevelConstructorColorPoint>();

            foreach (var color in colors)
                color.Construct(_sprites.First().texture);

            foreach (var area in areas)
                DestroyImmediate(area.gameObject);

            var spawnedAreas = new List<(Area, int)>();

            for (var i = 0; i < _sprites.Count; i++)
            {
                var sprite = _sprites[i];
                var area = Instantiate(_prefab, transform);
                area.Image.sprite = sprite;
                var lowestPixel = GetLowestPixel();
                spawnedAreas.Add((area, lowestPixel));

                int GetLowestPixel()
                {
                    var texture = area.Image.sprite.texture;
                    var pixels = texture.GetPixels32();
                    var textureSize = new Vector2Int(texture.width, texture.height);

                    for (var y = 0; y < textureSize.y; y++)
                    {
                        for (var x = 0; x < textureSize.x; x++)
                        {
                            var index = y * textureSize.x + x;

                            if (pixels[index].a > 0)
                                return y;
                        }
                    }

                    throw new Exception();
                }
            }

            var orderedAreas = spawnedAreas.OrderByDescending(t => t.Item2).Select(t => t.Item1).ToList();

            for (var i = 0; i < orderedAreas.Count; i++)
            {
                var area = orderedAreas[i];
                area.name = $"Area_{i}";
                var center = GetSpriteBounds(area.Image.sprite.texture).center;
                area.Image.color = GetInterpolatedColor(center, colors);
                area.transform.SetSiblingIndex(i);
                
                continue;

                RectInt GetSpriteBounds(Texture2D texture)
                {
                    var pixels = texture.GetPixels32();
                    var textureSize = new Vector2Int(texture.width, texture.height);

                    int minX = textureSize.x, minY = textureSize.y;
                    int maxX = 0, maxY = 0;

                    for (var y = 0; y < textureSize.y; y++)
                    {
                        for (var x = 0; x < textureSize.x; x++)
                        {
                            var index = y * textureSize.x + x;

                            if (pixels[index].a > 0)
                            {
                                // Update bounds
                                if (x < minX) minX = x;
                                if (x > maxX) maxX = x;
                                if (y < minY) minY = y;
                                if (y > maxY) maxY = y;
                            }
                        }
                    }

                    return new RectInt(minX, minY, maxX - minX + 1, maxY - minY + 1);
                }

                Color GetInterpolatedColor(Vector2 targetPosition, IReadOnlyList<LevelConstructorColorPoint> points)
                {
                    var accumulatedColor = Color.black;
                    var totalWeight = 0f;
                    var size = _selfTransform.rect.size;
                    targetPosition = targetPosition / size;

                    foreach (var point in points)
                    {
                        var pointPosition = point.Position / size;
                        var distance = Vector2.Distance(targetPosition, pointPosition);
                        var weight = 1f/ distance;

                        accumulatedColor += point.Color * weight;
                        totalWeight += weight;
                    }

                    var result = accumulatedColor / totalWeight;
                    return result;
                }
            }
            
            _colorsTransform.SetAsLastSibling();
        }
    }
}