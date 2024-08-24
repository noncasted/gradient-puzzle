namespace Features.GamePlay
{
    public interface IPaintSelection
    {
        void Clear();
        IPaintDock CreateDock();
    }
}