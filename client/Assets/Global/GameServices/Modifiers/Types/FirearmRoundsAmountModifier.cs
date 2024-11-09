namespace Global.GameServices
{
    public class FirearmRoundsAmountModifier : IntModifierDefinitionBase
    {
        protected override BaseIntModifier CreateInstance(IntModifierOptions options)
        {
            return new Value(options);
        }

        public class Value : BaseIntModifier
        {
            public Value(IntModifierOptions options) : base(options)
            {
            }
        }
    }
}