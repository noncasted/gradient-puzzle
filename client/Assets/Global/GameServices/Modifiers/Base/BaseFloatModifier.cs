namespace Global.GameServices
{
    public class BaseFloatModifier : BaseModifier<float>
    {
        public BaseFloatModifier(FloatModifierOptions options) : base(options.Data, options.Id)
        {
            _step = options.Step;
        }
        
        private readonly float _step;
        
        public override void Step()
        {
            Set(Result + _step);
        }
    }
}