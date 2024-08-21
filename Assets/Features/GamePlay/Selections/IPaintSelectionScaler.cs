namespace Features.GamePlay
{
    public interface IPaintSelectionScaler
    {
        float Size { get; }
        
        void Scale(int colorsAmount);
    }
}