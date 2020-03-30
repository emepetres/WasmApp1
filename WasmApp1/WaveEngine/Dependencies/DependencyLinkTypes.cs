// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;

namespace WaveEngine.Common.Dependencies
{
    /// <summary>
    /// Enum for the type of dependency links.
    /// </summary>
    [Flags]
    public enum DependencyLinkTypes
    {
        /// <summary>
        /// Attachable Objects.
        /// </summary>
        AttachableObject = 0x01,

        /// <summary>
        /// Loadable objects.
        /// </summary>
        Loadable = 0x02,

        /// <summary>
        /// All types.
        /// </summary>
        All = ~0,
    }
}
