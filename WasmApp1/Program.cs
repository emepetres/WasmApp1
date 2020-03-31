using System;
using WebAssembly;
using App;

namespace WasmApp1
{
    public class Program
    {
        //private static Container container;

        public static void Main()
        {
            // Create app
            var application = new MyApplication();

            using (var document = (JSObject)Runtime.GetGlobalObject("document"))
            using (var body = (JSObject)document.GetObjectProperty("body"))
            using (var button = (JSObject)document.Invoke("createElement", "button"))
            {
                button.SetObjectProperty("innerHTML", "Click me!");
                button.SetObjectProperty(
                    "onclick", 
                    new Action<JSObject>(_ => 
                    {
                        using (var window = (JSObject)Runtime.GetGlobalObject())
                        {
                            var message = application.Container.Resolve<DummyObject>().getDummyMessage();
                            window.Invoke("alert", message);
                        }
                    }));
                body.Invoke("appendChild", button);
            }
        }

        //private static void InitApp()
        //{
        //    container = new Container();

        //    container.RegisterType<DummyObject>();
        //}
    }
}
