using System;
using Internal;

namespace GamePlay.Common
{
    public static class StateMachineExtensions
    {
        public static void View<T>(
            this IViewableProperty<T> property,
            IStateHandle stateHandle,
            Action listener) => property.View(stateHandle.Lifetime, listener);

        public static void View<T>(
            this IViewableProperty<T> property,
            IStateHandle stateHandle,
            Action<T> listener) => property.View(stateHandle.Lifetime, listener);

        public static void View<T>(
            this IViewableProperty<T> property,
            IStateHandle stateHandle,
            Action<IReadOnlyLifetime, T> listener) => property.View(stateHandle.Lifetime, listener);
    }
}