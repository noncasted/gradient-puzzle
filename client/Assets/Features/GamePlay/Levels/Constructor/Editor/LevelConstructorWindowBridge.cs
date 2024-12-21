using Internal;
using UnityEditor;

namespace GamePlay.Levels
{
    [InitializeOnLoad]
    public static class LevelConstructorWindowBridge
    {
        static LevelConstructorWindowBridge()
        {
            var lifetime = new Lifetime();
            
            Level.ConstructorRequest.Advise(lifetime, OnRequest);
        }

        private static void OnRequest(Level level)
        {
            var window = EditorWindow.GetWindow<LevelConstructorWindow>("Level Constructor");
            window.Setup(level);
            window.Show();
        }
    }
}