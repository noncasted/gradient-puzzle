using System;
using Features.Common.StateMachines.Abstract;
using Internal;

namespace Features.Common.StateMachines
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