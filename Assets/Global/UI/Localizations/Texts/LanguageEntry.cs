using Sirenix.OdinInspector;
using UnityEngine;

namespace Global.UI.Texts
{
    [InlineEditor]
    public class LanguageEntry : ScriptableObject
    {
        [SerializeField] [Multiline] private string _text;

        public string Text => _text;
    }
}