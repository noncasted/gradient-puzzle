using System.Collections.Generic;
using Internal;

namespace Global.UI
{
    public interface INavigationTarget
    {
        IReadOnlyDictionary<Direction4, INavigationTarget> Targets { get; }
        
        void Inc();
        void Select();
        void Deselect();
        void PerformClick();
    }
}