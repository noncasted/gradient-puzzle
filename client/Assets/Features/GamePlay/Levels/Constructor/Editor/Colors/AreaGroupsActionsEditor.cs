using UnityEditor;
using UnityEngine;

namespace GamePlay.Levels
{
    [InitializeOnLoad]
    public class AreaGroupsActionsEditor
    {
        static AreaGroupsActionsEditor()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnPress;
        }

        private static void OnPress(int _, Rect _1)
        {
            var current = Event.current;

            if (current.type == EventType.KeyDown && current.control && current.keyCode == KeyCode.G)
            {
                AreaColorGrouper.GroupSelected();
                current.Use();
            }
        }
    }
}