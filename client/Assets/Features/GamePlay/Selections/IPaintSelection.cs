namespace GamePlay.Selections
{
    public interface IPaintSelection
    {
        void Clear();
        IPaintDock CreateDock();
    }
}