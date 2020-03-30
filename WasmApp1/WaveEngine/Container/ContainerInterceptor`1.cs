// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

namespace WaveEngine.Framework
{
    /// <summary>
    /// Represents the base class for a generic type interceptor registered in the <see cref="Application.Container"/>.
    /// </summary>
    /// <typeparam name="T">The generic type of the interceptor.</typeparam>
    public abstract class ContainerInterceptor<T> : ContainerInterceptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerInterceptor{T}"/> class. It will receive events for
        /// the type <typeparamref name="T"/> or any of its base types registered in the container.
        /// </summary>
        public ContainerInterceptor()
            : base(typeof(T))
        {
        }

        /// <inheritdoc />
        public sealed override void OnInstanced(object instance)
        {
            this.OnInstanced((T)instance);
        }

        /// <summary>
        /// Called every time an instance of a type assignable from <typeparamref name="T"/> is instanced by the container.
        /// </summary>
        /// <param name="instance">The generated instance.</param>
        protected abstract void OnInstanced(T instance);
    }
}
