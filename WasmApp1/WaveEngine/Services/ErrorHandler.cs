// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;

namespace WaveEngine.Framework.Services
{
    /// <summary>
    /// Error handler service.
    /// </summary>
    public class ErrorHandler : Service
    {
        /// <summary>
        /// Event that is fired when an exception is fired
        /// </summary>
        public event EventHandler<Exception> OnExceptionFired;

        /// <summary>
        /// Gets or sets a value indicating whether the application re-throw the exception after capture it.
        /// </summary>
        public bool RethrowException
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorHandler" /> class.
        /// </summary>
        public ErrorHandler()
        {
            this.RethrowException = true;
        }

        /// <summary>
        /// Register an exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>True if we want to re-throw the exception.</returns>
        internal bool CaptureException(Exception exception)
        {
            if (exception == null)
            {
                return false;
            }

            if (this.OnExceptionFired != null)
            {
                this.OnExceptionFired(this, exception);
            }

            return this.RethrowException;
        }
    }
}
