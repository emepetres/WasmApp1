// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Common;
using WaveEngine.Common.Dependencies;
using WaveEngine.Framework.Dependencies;

namespace WaveEngine.Framework
{
    /// <summary>
    /// Abstract class that represents an object that has dependencies defined.
    /// </summary>
    public abstract class DependencyObject : IdentifiableObject, IDependencyObject
    {
        /// <summary>
        /// Dependency list.
        /// </summary>
        internal Lazy<List<IDependencyLink>> dependencies;

        /// <inheritdoc/>
        public Lazy<List<IDependencyLink>> Dependencies => this.dependencies;

        /// <summary>
        /// Delegate that handle when a dependency is removed.
        /// </summary>
        /// <param name="memberName">The member name of the dependency.</param>
        /// <param name="removedDependency">The object in which the dependency has removed.</param>
        public delegate void DependencyRemovedHandle(string memberName, DependencyObject removedDependency);

        /// <summary>
        /// Event fired when a dependency is removed.
        /// </summary>
        public event DependencyRemovedHandle OnDependencyRemoved;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyObject"/> class.
        /// </summary>
        public DependencyObject()
        {
            this.dependencies = new Lazy<List<IDependencyLink>>();
        }

        /// <summary>
        /// Resolve the dependencies of this instance.
        /// </summary>
        /// <returns>If all dependencies has been satisfied.</returns>
        internal bool ResolveDependencies()
        {
            return DependencyResolver.ResolveDependencies(this);
        }

        /// <summary>
        /// Delete all dependencies.
        /// </summary>
        internal void DeleteDependencies(DependencyLinkTypes types)
        {
            var dependencies = this.Dependencies.Value;
            for (int i = dependencies.Count - 1; i >= 0; i--)
            {
                var dependencyLink = dependencies[i];
                if (types.HasFlag(dependencyLink.Type))
                {
                    dependencyLink.Unregister(this);
                }
            }
        }

        internal void FireOnDependencyRemoved(string memberName, DependencyObject dependency)
        {
            this.OnDependencyRemoved?.Invoke(memberName, dependency);
        }

        /// <summary>
        /// A required dependency of this object has been broken.
        /// </summary>
        internal protected abstract void DependencyBroken();
    }
}
