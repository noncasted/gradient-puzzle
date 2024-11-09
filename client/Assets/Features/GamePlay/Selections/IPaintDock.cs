using GamePlay.Common;

namespace GamePlay.Selections
{
    public interface IPaintDock : IPaintTarget
    {
        float Size { get; }

        void UpdateTransform(int areaSize);
    }
}