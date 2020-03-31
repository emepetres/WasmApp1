// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;

namespace WaveEngine.Framework
{
    /// <summary>
    /// Abstract class that represents an object that could be disposed.
    /// Implements the IDisposable pattern, exposing a Destroy() method.
    /// </summary>
    public abstract class DisposableObject : IDisposable
    {
        private bool disposed;

        /// <summary>
        /// Gets a value indicating whether this object is disposed.
        /// </summary>
        public bool Disposed => this.disposed;

        /// <summary>
        /// Finalizes an instance of the <see cref="DisposableObject"/> class.
        /// </summary>
        ~DisposableObject()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Public implementation of Dispose pattern callable by consumers.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">Dispose native elements.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Destroy();
            }

            this.disposed = true;
        }

        /// <summary>
        /// Destroy all resources of this instance.
        /// </summary>
        protected abstract void Destroy();
    }
}
