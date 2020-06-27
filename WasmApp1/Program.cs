using System;
using System.Runtime.InteropServices;

namespace WasmApp1
{
  class MonoPInvokeCallbackAttribute : Attribute
  {
    public MonoPInvokeCallbackAttribute(Type t) { }
  }

  class Program
  {
    private const CallingConvention CallConv = CallingConvention.Winapi;

    [UnmanagedFunctionPointer(CallConv)]
    private delegate void doLoop_t();

    [DllImport("libwebgpu")]
    private static extern void emscripten_set_main_loop(doLoop_t func, int fps, int simulate_infinite_loop);

    private static void Main()
    {
      emscripten_set_main_loop(do_loop, 0, 0);
    }

    [MonoPInvokeCallback(typeof(doLoop_t))]
    private static void do_loop()
    {
      Console.WriteLine("loop!");
    }
  }
}
