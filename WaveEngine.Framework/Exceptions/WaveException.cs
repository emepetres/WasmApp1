// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;

namespace WaveEngine.Framework.Exceptions
{
    /// <summary>
    /// This class represent a Wave Exception.
    /// </summary>
    public class WaveException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaveException" /> class.
        /// </summary>
        /// <param name="message">The owner.</param>
        /// <param name="innerException">The inner exception.</param>
        internal WaveException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
