using System.Collections.Generic;
using Internal;
using UnityEditor;
using UnityEngine;

namespace GamePlay.Levels
{
    public class AreaColorGrouper
    {
        public static void GroupSelected()
        {
            var selectedObjects = Selection.gameObjects;
            
            if (selectedObjects.Length == 0)
                return;

            Undo.IncrementCurrentGroup();
            if (selectedObjects.Length == 1)
            {
                var group = selectedObjects[0].GetComponent<AreaColorGroup>();
                
                if (group != null)
                {
                    HandleUngroup(group);
                    return;
                }
            }

            HandleGroup(selectedObjects);
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        private static void HandleGroup(IReadOnlyList<GameObject> objects)
        {
            var parent = GetParent();
            var options = AssetsExtensions.Environment.GetAsset<AreaColorGroupOptions>();

            var group = PrefabUtility.InstantiatePrefab(options.GroupPrefab, parent) as AreaColorGroup;
            Undo.RegisterCreatedObjectUndo(group.gameObject, "Create Group");

            foreach (var selectedObject in objects)
                Undo.SetTransformParent(selectedObject.transform, group.transform, "Group Selected Objects");

            return;

            Transform GetParent()
            {
                return objects[0].transform.parent;
            }
        }

        private static void HandleUngroup(AreaColorGroup group)
        {
            var parent = group.transform.parent;

            var areas = group.GetComponentsInChildren<Area>(true);
            
            foreach (var area in areas)
                Undo.SetTransformParent(area.transform, parent, "Ungroup Selected Objects");
            
            Object.DestroyImmediate(group.gameObject);
        }
    }
}