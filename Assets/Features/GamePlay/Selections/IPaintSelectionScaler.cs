namespace Features.GamePlay
{
    public interface IPaintSelectionScaler
    {
        float DockSize { get; }
        int AreaSize { get; }
        
        void Scale(int colorsAmount);
        void Disable();
    }
}