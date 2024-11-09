namespace Global.GameServices
{
    public readonly struct IntModifierOptions
    {
        public IntModifierOptions(
            int id,
            int step,
            IModifierData data)
        {
            Id = id;
            Step = step;
            Data = data;
        }
        
        public int Id { get; }
        public int Step { get; }
        public IModifierData Data { get; }
    }
    
    public readonly struct FloatModifierOptions
    {
        public FloatModifierOptions(
            int id,
            float step,
            IModifierData data)
        {
            Id = id;
            Step = step;
            Data = data;
        }
        
        public int Id { get; }
        public float Step { get; }
        public IModifierData Data { get; }
    }
}