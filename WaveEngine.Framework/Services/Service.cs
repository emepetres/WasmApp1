// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System.Runtime.Serialization;

namespace WaveEngine.Framework.Services
{
    /// <summary>
    /// This class is used as a base for different game services that are available application wide.
    /// </summary>
    /// <remarks>
    /// Services are exposed and managed by the "WaveEngine.Framework.Services.WaveServices" static class.
    /// </remarks>
    public abstract class Service : AttachableObject
    {
        /// <inheritdoc/>
        protected override void OnLoaded()
        {
        }

        /// <inheritdoc/>
        protected override bool OnAttached()
        {
            return true;
        }

        /// <inheritdoc/>
        protected override void OnActivated()
        {
        }

        /// <inheritdoc/>
        protected override void Start()
        {
        }

        /// <inheritdoc/>
        protected override void OnDeactivated()
        {
        }

        /// <inheritdoc/>
        protected override void OnDetach()
        {
        }

        /// <inheritdoc/>
        protected override void OnDestroy()
        {
        }
    }
}
