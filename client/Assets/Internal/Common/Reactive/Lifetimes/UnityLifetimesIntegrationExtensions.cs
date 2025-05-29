using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Internal
{
    public static class UnityLifetimesIntegrationExtensions
    {
        public static ILifetimedValue<float> ToLifetimedValue(this Slider source, IReadOnlyLifetime lifetime)
        {
            var lifetimedValue = source.onValueChanged.ToLifetimedValue(source.value, lifetime);

            return lifetimedValue;
        }

        public static ILifetimedValue<T> ToLifetimedValue<T>(
            this UnityEvent<T> source,
            T baseValue,
            IReadOnlyLifetime lifetime)
        {
            var lifetimedValue = new LifetimedValue<T>(baseValue);

            source.AddListener(Listener);
            lifetime.Listen(() => source.RemoveListener(Listener));

            return lifetimedValue;

            void Listener(T value)
            {
                lifetimedValue.Set(value);
            }
        }

        public static void Advise<T>(
            this UnityEvent<T> source,
            T baseValue,
            IReadOnlyLifetime lifetime,
            Action<T> action)
        {
            source.ToLifetimedValue(baseValue, lifetime).Advise(lifetime, action);
        }
        
        public static void Advise(
            this Slider slider,
            IReadOnlyLifetime lifetime,
            Action<float> action)
        {
            slider.onValueChanged.Advise(slider.value, lifetime, action);
        }
    }
}