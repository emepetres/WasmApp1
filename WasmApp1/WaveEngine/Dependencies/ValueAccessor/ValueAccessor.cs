// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;

namespace WaveEngine.Framework.Dependencies
{
    /// <summary>
    /// Set Values.
    /// </summary>
    public abstract class ValueAccessor
    {
        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        public abstract string MemberName { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>The value.</returns>
        public abstract object GetValue(DependencyObject instance);

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="newValue">The new value.</param>
        public abstract void SetValue(DependencyObject instance, object newValue);
    }
}
