// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using WaveEngine.Common;
using WaveEngine.Common.Dependencies;

namespace WaveEngine.Framework.Dependencies
{
    /// <summary>
    /// Represents a dependency link between two objects.
    /// If one of the objects is deleted, the link must be removed.
    /// </summary>
    public class DependencyLink : IDependencyLink
    {
        /// <inheritdoc/>
        public DependencyLinkTypes Type => DependencyLinkTypes.AttachableObject;

        /// <summary>
        /// The source of the dependency link.
        /// </summary>
        public readonly DependencyObject Source;

        /// <summary>
        /// The target of the dependency link.
        /// </summary>
        public readonly DependencyObject Target;

        /// <summary>
        /// The property resolver.
        /// </summary>
        public readonly Resolver Resolver;

        /// <summary>
        /// Gets the member name of the dependency link.
        /// </summary>
        public string MemberName => this.Resolver.ValueAccessor.MemberName;

        /// <summary>
        /// Gets a value indicating whether this dependency is required.
        /// </summary>
        public bool IsRequired => this.Resolver.BindAttribute.IsRequired;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyLink"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="resolver">The resolver.</param>
        public DependencyLink(DependencyObject source, DependencyObject target, Resolver resolver)
        {
            this.Source = source;
            this.Target = target;
            this.Resolver = resolver;
        }

        /// <summary>
        /// Registers this instance in the dependency list of source and target..
        /// </summary>
        public void Register()
        {
            this.Source.Dependencies.Value.Add(this);
            this.Target.Dependencies.Value.Add(this);
        }

        /// <summary>
        /// Unregisters this instance in the dependency list of source and target.
        /// </summary>
        /// <param name="caller">The caller.</param>
        public void Unregister(IDependencyObject caller)
        {
            bool isTarget = caller == this.Target;

            if (isTarget)
            {
                this.Source.Dependencies.Value.Remove(this);
            }
            else
            {
                this.Target.Dependencies.Value.Remove(this);
            }

            this.Resolver.Delete(this.Source, this.Target, isTarget);

            // Fire event on source object
            this.Source.FireOnDependencyRemoved(this.MemberName, this.Target);
        }
    }
}
