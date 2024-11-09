namespace Global.GameServices
{
    public class BaseIntModifier : BaseModifier<int>
    {
        public BaseIntModifier(IntModifierOptions options) : base(options.Data, options.Id)
        {
            _step = options.Step;
        }
        
        private readonly int _step;

        public override void Step()
        {
            Set(Result + _step);
        }
    }
}