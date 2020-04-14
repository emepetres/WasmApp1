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

        private static JSObject window;

        private static Action<double> LoopAction = new Action<double>(loop);

        private static int iteration_counter = 0;
        private static int iteration_counter_global = 0;
        private static System.Diagnostics.Stopwatch do_frame_watch = new System.Diagnostics.Stopwatch();
        private static double do_frame_elapsed_ms = 0;
        private static double do_frame_elapsed_ms_global = 0;

        private static double iteration_elapsed_ms = 0;
        private static double iteration_elapsed_ms_global = 0;
        private static double previous_timestamp_ms = 0;

        private static void Main()
        {
            window = (JSObject)Runtime.GetGlobalObject();

            test_wrapper();

            //using (var document = (JSObject)Runtime.GetGlobalObject("document"))
            //using (var body = (JSObject)document.GetObjectProperty("body"))
            //using (var button = (JSObject)document.Invoke("createElement", "button"))
            //{
            //    button.SetObjectProperty("innerHTML", "Click me!");
            //    button.SetObjectProperty(
            //        "onclick",
            //        new Action<JSObject>(_ =>
            //        {
            //            window.Invoke("alert", "Hello, Wasm!");
            //        }));
            //    body.Invoke("appendChild", button);
            //}

            if (init_gl())
            {

                on_create();

                //set_main_loop();
                loop(0);
            }
            else
            {
                System.Console.WriteLine("WebGL Init failed!");
            }

            //on_terminate();
        }

        private static void loop(double timestampMilliseconds)
        {
            do_frame_watch.Restart();
            do_frame();
            do_frame_watch.Stop();

            iteration_counter++;
            iteration_elapsed_ms += (timestampMilliseconds - previous_timestamp_ms);
            previous_timestamp_ms = timestampMilliseconds;
            do_frame_elapsed_ms += do_frame_watch.Elapsed.TotalMilliseconds;
            if (iteration_counter >= 6000)
            {
                iteration_counter_global += iteration_counter;
                do_frame_elapsed_ms_global += do_frame_elapsed_ms;
                iteration_elapsed_ms_global += iteration_elapsed_ms;
                System.Console.WriteLine($"glClear: {do_frame_elapsed_ms / iteration_counter} ms, mean: {do_frame_elapsed_ms_global / iteration_counter_global} ms");
                System.Console.WriteLine($"FPS: {(iteration_counter / iteration_elapsed_ms)*1000}, mean: {(iteration_counter_global/ iteration_elapsed_ms_global)*1000}");
                iteration_counter = 0;
                do_frame_elapsed_ms = 0;
                iteration_elapsed_ms = 0;
            }
            window.Invoke("requestAnimationFrame", LoopAction);
        }
    }
}
