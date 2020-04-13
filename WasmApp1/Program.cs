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
        private static extern void do_frame();

        [DllImport("DynamicWrapper")]
        private static extern void on_terminate();

        private static void Main()
        {
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
                        using (var window = (JSObject)Runtime.GetGlobalObject())
                        {
                            window.Invoke("alert", "Hello, Wasm!");
                        }
                    }));
                body.Invoke("appendChild", button);
            }

            if (init_gl())
            {
                on_create();
                while (true)
                {
                    do_frame();
                }
            }

            on_terminate();
        }
    }
}
