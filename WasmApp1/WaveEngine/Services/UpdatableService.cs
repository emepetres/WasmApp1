// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Runtime.Serialization;

namespace WaveEngine.Framework.Services
{
    /// <summary>
    /// Specialized class for a <see cref="Service"/> that needs to be updated constantly.
    /// </summary>
    /// <remarks>
    /// Services are exposed and managed by the "WaveEngine.Framework.Services.WaveServices" static class.
    /// </remarks>
    public abstract class UpdatableService : Service
    {
        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <param name="gameTime">The elapsed game time since the last update.</param>
        public abstract void Update(TimeSpan gameTime);
    }
}
