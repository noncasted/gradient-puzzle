using UnityEditor;

namespace GamePlay.Levels
{
    [InitializeOnLoad]
    public class SelectedAreaContourDrawer
    {
        static SelectedAreaContourDrawer()
        {
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            if (Selection.gameObjects == null || Selection.gameObjects.Length == 0)
                return;

            foreach (var gameObject in Selection.gameObjects)
            {
                if (gameObject.TryGetComponent(out Area area) == false)
                    continue;
                
                area.DrawContour();
            }
        }
    }
}