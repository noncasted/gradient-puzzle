namespace Features.GamePlay
{
    public interface IPaintDock : IPaintTarget
    {
        float Size { get; }

        void UpdateTransform(int areaSize);
    }
}