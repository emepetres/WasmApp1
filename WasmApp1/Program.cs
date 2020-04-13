using System;
using System.Runtime.InteropServices;
using WebAssembly;

namespace WasmApp1
{
    internal class Program
    {
        [DllImport("DynamicWrapper")]
        private static extern void test_wrapper();

        [DllImport("DynamicWrapper")]
        private static extern bool init_gl();

        [DllImport("DynamicWrapper")]
        private static extern void on_create();

        [DllImport("DynamicWrapper")]
        private static extern void set_main_loop();

        [DllImport("DynamicWrapper")]
        private static extern void do_frame();

        [DllImport("DynamicWrapper")]
        private static extern void on_terminate();

        private static JSObject window;

        private static void Main()
        {
            window = (JSObject)Runtime.GetGlobalObject();

            test_wrapper();

            using (var document = (JSObject)Runtime.GetGlobalObject("document"))
            using (var body = (JSObject)document.GetObjectProperty("body"))
            using (var button = (JSObject)document.Invoke("createElement", "button"))
            {
                button.SetObjectProperty("innerHTML", "Click me!");
                button.SetObjectProperty(
                    "onclick",
                    new Action<JSObject>(_ =>
                    {
                        window.Invoke("alert", "Hello, Wasm!");
                    }));
                body.Invoke("appendChild", button);
            }

            if (init_gl())
            {

                on_create();

                set_main_loop();
                //window.Invoke("requestAnimationFrame", new Action<double>(_ => do_frame()));
            }
            else
            {
                System.Console.WriteLine("WebGL Init failed!");
            }

            //on_terminate();
        }
    }
}
