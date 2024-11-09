namespace Global.GameServices
{
    public class FirearmRearmTimeModifier : FloatModifierDefinitionBase
    {
        protected override BaseFloatModifier CreateInstance(FloatModifierOptions options)
        {
            return new Value(options);
        }

        public class Value : BaseFloatModifier
        {
            public Value(FloatModifierOptions options) : base(options)
            {
            }
        }
    }
}