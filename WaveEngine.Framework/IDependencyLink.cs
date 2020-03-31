// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using WaveEngine.Common.Dependencies;

namespace WaveEngine.Common
{
    /// <summary>
    /// Interface that represents a dependency link between two objects.
    /// </summary>
    public interface IDependencyLink
    {
        /// <summary>
        /// Gets the dependency link type.
        /// </summary>
        DependencyLinkTypes Type { get; }

        /// <summary>
        /// Registers this instance in the dependency list of source and target..
        /// </summary>
        void Register();

        /// <summary>
        /// Unregisters this instance in the dependency list of source and target.
        /// </summary>
        /// <param name="caller">The caller.</param>
        void Unregister(IDependencyObject caller);
    }
}
