// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;

namespace WaveEngine.Framework
{
    /// <summary>
    /// Represents the base class for a interceptor registered in the <see cref="Application.Container"/>.
    /// </summary>
    public abstract class ContainerInterceptor
    {
        /// <summary>
        /// Gets the type or base type to be intercepted.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the container where the interceptor is registered.
        /// </summary>
        public Container Container { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerInterceptor"/> class. It will receive events for
        /// every type registered in the container.
        /// </summary>
        public ContainerInterceptor()
        {
            this.Type = typeof(object);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerInterceptor"/> class. It will receive events for
        /// the specified type or any of its base types registered in the container.
        /// </summary>
        /// <param name="type">The type or base type to be intercepted.</param>
        public ContainerInterceptor(Type type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Called when a type assignable from <see param="type"/> is registered in the container.
        /// </summary>
        /// <param name="type">The registered type.</param>
        public abstract void OnRegistered(Type type);

        /// <summary>
        /// Called when a type assignable from <see param="type"/> is unregistered from the container.
        /// </summary>
        /// <param name="type">The unregistered type.</param>
        public abstract void OnUnregistered(Type type);

        /// <summary>
        /// Called every time an instance of a type assignable from <see param="instance"/> is instanced by the container.
        /// </summary>
        /// <param name="instance">The generated instance.</param>
        public abstract void OnInstanced(object instance);
    }
}
