using Internal;

namespace Global.GameServices
{
    public abstract class BaseModifier<T> : IModifier
    {
        public BaseModifier(IModifierData data, int id)
        {
            Data = data;
            Id = id;
            _value = new ViewableProperty<T>();
        }

        private readonly ViewableProperty<T> _value;

        public IModifierData Data { get; }
        public int Id { get; }

        public IViewableProperty<T> Value => _value;
        public T Result => _value.Value;

        public void Set(T value)
        {
            _value.Set(value);
        }

        public abstract void Step();
    }
}