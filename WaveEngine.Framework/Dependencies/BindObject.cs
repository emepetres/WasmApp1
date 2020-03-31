// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using WaveEngine.Common.Attributes;

namespace WaveEngine.Framework.Dependencies
{
    /// <summary>
    /// Custom attribute used to connect two <see cref="Component"/> instances.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public abstract class BindObject : WaveIgnoreAttribute
    {
        /// <summary>
        /// Whether this binding is required.
        /// </summary>
        internal protected bool IsRequired;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindObject" /> class.
        /// </summary>
        public BindObject()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindObject" /> class.
        /// </summary>
        /// <param name="isRequired">Whether this binding is required.</param>
        public BindObject(bool isRequired)
        {
            this.IsRequired = isRequired;
        }

        /// <summary>
        /// Resolve this binding with the specified object.
        /// </summary>
        /// <param name="instance">The object to resolve.</param>
        /// <param name="propertyType">The property type.</param>
        /// <returns>The result object.</returns>
        public abstract object Resolve(DependencyObject instance, Type propertyType);

        /// <summary>
        /// Resolve this binding with the specified object.
        /// </summary>
        /// <param name="instance">The object to resolve.</param>
        /// <param name="propertyType">The property type.</param>
        /// <returns>The collection.</returns>
        public abstract IEnumerable<object> ResolveCollection(DependencyObject instance, Type propertyType);
    }
}
