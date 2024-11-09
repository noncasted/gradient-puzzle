namespace Global.GameServices
{
    public interface IModifier
    {
        IModifierData Data { get; }
        int Id { get; }

        void Step();
    }
}