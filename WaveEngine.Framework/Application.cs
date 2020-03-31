// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using WaveEngine.Common;
using WaveEngine.Framework.Exceptions;
using WaveEngine.Framework.Services;

namespace WaveEngine.Framework
{
    public class Application : DisposableObject
    {
        private bool active;

        private readonly Container container;

        private readonly ServiceInterceptor servicesInterceptor;

        //private Clock clockService;

        private ErrorHandler errorHandler;

        public static Application Current
        {
            get; private set;
        }

        public Container Container => this.container;

        public Application()
        {
            this.active = true;
            this.container = new Container();
            this.servicesInterceptor = new ServiceInterceptor();
            this.container.RegisterInterceptor(this.servicesInterceptor);
            Application.Current = this;
        }

        /// <summary>
        /// Initializes the application according to the passed platform.
        /// Such method acts as the bridge between the application and the final hardware.
        /// </summary>
        public virtual void Initialize()
        {
            // Cache services
            //this.clockService = this.container.Resolve<Clock>();
            this.errorHandler = this.container.Resolve<ErrorHandler>();

            this.servicesInterceptor.InitializeServices();
        }

        public void Load(string path)
        {

        }

        public virtual void UpdateFrame(TimeSpan gameTime)
        {
            if (this.active)
            {
                try
                {
                    //this.clockService?.Update(gameTime);

                    // Update services
                    this.servicesInterceptor?.UpdateServices(gameTime);
                }
                catch (Exception ex)
                {
                    if (this.errorHandler.CaptureException(new WaveException("UpdateFrame exception", ex)))
                    {
                        throw;
                    }
                }
            }
        }

        public virtual void OnActivated()
        {
            this.active = true;
            this.servicesInterceptor.OnActivated();
        }

        public virtual void OnDeactivated()
        {
            this.servicesInterceptor.OnDeactivated();
            this.active = false;
        }

        protected override void Destroy()
        {
            this.servicesInterceptor.OnDestroy();
            this.container.Dispose();
        }
    }
}
