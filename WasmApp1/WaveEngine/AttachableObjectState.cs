// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

namespace WaveEngine.Framework
{
    /// <summary>
    /// States enumeration of attachable objects.
    /// </summary>
    public enum AttachableObjectState : byte
    {
        /// <summary>
        /// The object is not attached
        /// </summary>
        Detached = 0,

        /// <summary>
        /// The object is not activated
        /// </summary>
        Deactivated,

        /// <summary>
        /// The object is activated
        /// </summary>
        Activated,

        /// <summary>
        /// The object is destroyed
        /// </summary>
        Destroyed,
    }
}
