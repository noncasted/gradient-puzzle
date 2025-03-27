using GamePlay.Common;

namespace GamePlay.Selections
{
    public interface IPaintDock : IPaintTarget
    {
        void UpdateTransform(int areaSize);
    }
}