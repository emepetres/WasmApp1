using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;

namespace App
{
    public class MyApplication : Application
    {
        public MyApplication()
        {
            System.Console.WriteLine("-----> Registering dummy object");
            this.Container.RegisterType<DummyObject>();
            var dummy = this.Container.Resolve<DummyObject>();
            System.Console.WriteLine($"Dummy message: {dummy.getDummyMessage()}");

            //System.Console.WriteLine("-----> Registering clock");
            //this.Container.RegisterType<Clock>();
            //var clock = this.Container.Resolve<Clock>();
            //System.Console.WriteLine($"clock elapse time: {clock.ElapseTime}");


            ////this.Container.RegisterType<Clock>();
            //this.Container.RegisterType<TimerFactory>();
            //this.Container.RegisterType<Random>();
            //this.Container.RegisterType<ErrorHandler>();
            //this.Container.RegisterType<ScreenContextManager>();
            //this.Container.RegisterType<GraphicsPresenter>();
            //this.Container.RegisterType<AssetsDirectory>();
            //this.Container.RegisterType<AssetsService>();
            //this.Container.RegisterType<ForegroundTaskSchedulerService>();

            //System.Console.WriteLine("Registered Types!");

            //ForegroundTaskScheduler.Foreground.Configure(this.Container);

            //System.Console.WriteLine("Foreground configured!");

            //BackgroundTaskScheduler.Background.Configure(this.Container);
        }

        public override void Initialize()
        {
            base.Initialize();

            //// Get ScreenContextManager
            //var screenContextManager = this.Container.Resolve<ScreenContextManager>();
            //var assetsService = this.Container.Resolve<AssetsService>();

            //// Navigate to scene
            //var scene = assetsService.Load<MyScene>(WaveContent.Scenes.MyScene_wescene);
            //ScreenContext screenContext = new ScreenContext(scene);
            //screenContextManager.To(screenContext);
        }
    }

}
