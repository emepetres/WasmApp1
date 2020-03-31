// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Dependencies;

namespace WaveEngine.Framework
{
    /// <summary>
    /// Abstract class that represents an object that can be attached to the system.
    /// It implements the default life-cycle Detached-Deactivated-Activated-Destroyed.
    /// </summary>
    public abstract class AttachableObject : DependencyObject
    {
        /// <summary>
        /// The current state.
        /// </summary>
        private AttachableObjectState state;

        /// <summary>
        /// Indicated if this object has been loaded.
        /// </summary>
        private bool isLoaded;

        /// <summary>
        /// Indicates if this object has been started.
        /// </summary>
        private bool isStarted;

        /// <summary>
        /// Indicates if this object is enabled.
        /// </summary>
        private bool isEnabled;

        /// <summary>
        /// Gets or sets a value indicating whether this entity is enabled.
        /// </summary>
        public virtual bool IsEnabled
        {
            get => this.isEnabled;

            set
            {
                if (this.isEnabled != value)
                {
                    this.isEnabled = value;
                    this.RefreshIsEnabled();
                }
            }
        }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        public AttachableObjectState State => this.state;

        /// <summary>
        /// Gets a value indicating whether this object is loaded.
        /// </summary>
        public virtual bool IsLoaded => this.isLoaded;

        /// <summary>
        /// Gets a value indicating whether this object is activated.
        /// </summary>
        public virtual bool IsAttached => (this.state == AttachableObjectState.Deactivated) || (this.state == AttachableObjectState.Activated);

        /// <summary>
        /// Gets a value indicating whether this object is started.
        /// </summary>
        public virtual bool IsActivated => this.state == AttachableObjectState.Activated;

        /// <summary>
        /// Gets a value indicating whether this object is started.
        /// </summary>
        public virtual bool IsStarted => this.isStarted && this.state == AttachableObjectState.Activated;

        /// <summary>
        /// Gets a value indicating whether this object is destroyed.
        /// </summary>
        public virtual bool IsDestroyed => this.state == AttachableObjectState.Destroyed;

        /// <summary>
        /// Gets a value indicating whether this object should be activated.
        /// </summary>
        internal protected virtual bool ShouldBeActivated => this.isEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachableObject"/> class.
        /// </summary>
        public AttachableObject()
            : this(AttachableObjectState.Detached, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachableObject"/> class.
        /// </summary>
        /// <param name="initialState">The initial state.</param>
        /// <param name="isLoaded">IsLoaded value.</param>
        /// <param name="isStarted">IsStarted value.</param>
        public AttachableObject(AttachableObjectState initialState, bool isLoaded, bool isStarted)
        {
            this.isLoaded = isLoaded;
            this.isStarted = isStarted;
            this.state = initialState;
            this.isEnabled = true;
        }

        /// <summary>
        /// Invoked when the object is loaded.
        /// </summary>
        protected abstract void OnLoaded();

        /// <summary>
        /// Invoked when the object is attached to the system.
        /// </summary>
        /// <returns>True if all is OK.</returns>
        protected abstract bool OnAttached();

        /// <summary>
        /// Invoked when the object is activated once is attached.
        /// </summary>
        protected abstract void OnActivated();

        /// <summary>
        /// Invoked to start the object.
        /// </summary>
        protected abstract void Start();

        /// <summary>
        /// Invoked when the object is deactivated.
        /// </summary>
        protected abstract void OnDeactivated();

        /// <summary>
        /// Invoked when the object is detached.
        /// </summary>
        protected abstract void OnDetach();

        /// <summary>
        /// Invoked when the object is going to be disposed.
        /// </summary>
        protected abstract void OnDestroy();

        /// <summary>
        /// Load this object.
        /// </summary>
        internal virtual void Load()
        {
            if (this.state != AttachableObjectState.Detached)
            {
                throw new InvalidOperationException($"The attachable object must be detached to be loaded: {this.GetType()} State: {this.state}");
            }

            if (!this.isLoaded)
            {
                this.OnLoaded();
                this.isLoaded = true;
            }
        }

        /// <summary>
        /// Attach the object.
        /// </summary>
        /// <returns>True if all is OK.</returns>
        internal virtual bool Attach()
        {
            if (this.state != AttachableObjectState.Detached)
            {
                throw new InvalidOperationException($"The attachable object must be detached to be loaded: {this.GetType()} State: {this.state}");
            }

            if (this.ResolveDependencies())
            {
                if (this.OnAttached())
                {
                    this.state = AttachableObjectState.Deactivated;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Activate this object.
        /// </summary>
        internal virtual void Activate()
        {
            if (this.state != AttachableObjectState.Deactivated)
            {
                throw new InvalidOperationException($"The attachable object must be deactivated to be activated: {this.GetType()} State: {this.state}");
            }

            if (this.ShouldBeActivated)
            {
                this.OnActivated();
                this.state = AttachableObjectState.Activated;
            }
        }

        /// <summary>
        /// Start this object.
        /// </summary>
        internal virtual void BaseStart()
        {
            if (this.state != AttachableObjectState.Activated)
            {
                throw new InvalidOperationException($"The attachable object must be activated to be started: {this.GetType()} State: {this.state}");
            }

            if (!this.isStarted)
            {
                this.Start();
                this.isStarted = true;
            }
        }

        /// <summary>
        /// Deactivate this object.
        /// </summary>
        internal virtual void Deactivate()
        {
            if (this.state != AttachableObjectState.Activated)
            {
                throw new InvalidOperationException($"The attachable object must be activated to be deactivated: {this.GetType()} State: {this.state}");
            }

            this.OnDeactivated();
            this.state = AttachableObjectState.Deactivated;
        }

        /// <summary>
        /// Detach this object.
        /// </summary>
        internal virtual void Detach()
        {
            if (this.state != AttachableObjectState.Deactivated)
            {
                throw new InvalidOperationException($"The attachable object must be previously attached to be detached: {this.GetType()} State: {this.state}");
            }

            this.OnDetach();
            this.DeleteDependencies(DependencyLinkTypes.AttachableObject);
            this.state = AttachableObjectState.Detached;
            this.isStarted = false;
        }

        /// <summary>
        /// Destroy this object.
        /// </summary>
        public virtual void Destroy()
        {
            if (this.state != AttachableObjectState.Detached)
            {
                throw new InvalidOperationException($"The attachable object must be previously detached to be destroyed: {this.GetType()} State: {this.state}");
            }

            if (this.isLoaded)
            {
                this.OnDestroy();
            }

            this.DeleteDependencies(DependencyLinkTypes.All);
            this.state = AttachableObjectState.Destroyed;
        }

        /// <inheritdoc/>
        internal protected override void DependencyBroken()
        {
            this.ForceState(AttachableObjectState.Detached);
        }

        /// <summary>
        /// Force the object to go a specific state.
        /// It navigate for all states to arrive to the desired state.
        /// </summary>
        /// <remarks>
        /// We recommend to use this method only for testing purposes.
        /// Use instead Activate(), Detach()... methods manually.
        /// </remarks>
        /// <param name="newState">The new state.</param>
        /// <param name="isStarted">Check if the object need to be started.</param>
        /// <returns>If the final state is the desired.</returns>
        internal bool ForceState(AttachableObjectState newState, bool isStarted = false)
        {
            switch (this.state)
            {
                case AttachableObjectState.Detached:
                    switch (newState)
                    {
                        case AttachableObjectState.Deactivated:
                            this.Attach();
                            break;
                        case AttachableObjectState.Activated:
                            if (this.Attach())
                            {
                                this.Activate();
                                if (isStarted)
                                {
                                    this.BaseStart();
                                }
                            }

                            break;
                        case AttachableObjectState.Destroyed:
                            this.Destroy();
                            break;
                    }

                    break;
                case AttachableObjectState.Deactivated:
                    switch (newState)
                    {
                        case AttachableObjectState.Detached:
                            this.Detach();
                            break;
                        case AttachableObjectState.Activated:
                            this.Activate();
                            if (isStarted)
                            {
                                this.BaseStart();
                            }

                            break;
                        case AttachableObjectState.Destroyed:
                            this.Detach();
                            this.Destroy();
                            break;
                    }

                    break;
                case AttachableObjectState.Activated:
                    switch (newState)
                    {
                        case AttachableObjectState.Activated:
                            if (isStarted && !this.isStarted)
                            {
                                this.BaseStart();
                            }

                            break;
                        case AttachableObjectState.Detached:
                            this.Deactivate();
                            this.Detach();
                            break;
                        case AttachableObjectState.Deactivated:
                            this.Deactivate();
                            break;
                        case AttachableObjectState.Destroyed:
                            this.Deactivate();
                            this.Detach();
                            this.Destroy();
                            break;
                    }

                    break;
                case AttachableObjectState.Destroyed:
                    // Do nothing... nobody scape from Destroyed state
                    break;
            }

            return this.state == newState;
        }

        internal virtual void RefreshIsEnabled()
        {
            if (this.IsAttached)
            {
                if (this.isEnabled)
                {
                    if (!this.IsActivated)
                    {
                        this.Activate();
                    }
                }
                else
                {
                    if (this.IsActivated)
                    {
                        this.Deactivate();
                    }
                }
            }
        }
    }
}
