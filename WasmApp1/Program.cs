using System;
using System.Runtime.InteropServices;
using WebAssembly;

namespace WasmApp1
{
	using EGLDisplay = IntPtr; //void*
	using EGLint = Int32;
	using EGLBoolean = UInt32;
	using EGLConfig = IntPtr; //void*
	using EGLNativeWindowType = Int32;
	using EGLSurface = IntPtr; //void*
	using EGLContext = IntPtr; //void*

	internal unsafe class Program
    {
		[DllImport("libEGL")]
		private static extern EGLDisplay eglGetDisplay(IntPtr eglnativedisplaytype);

		[DllImport("libEGL")]
		private static extern EGLint eglGetError();

		[DllImport("libEGL")]
		private static extern EGLBoolean eglInitialize(EGLDisplay display, EGLint *major, EGLint *minor);

		[DllImport("libEGL")]
		private static extern EGLBoolean eglGetConfigs(EGLDisplay display, EGLConfig *configs, EGLint config_size, EGLint *num_config);

		[DllImport("libEGL")]
		private static extern EGLBoolean eglChooseConfig(EGLDisplay dpy, EGLint[] attrib_list,
			   EGLConfig *configs, EGLint config_size,
			   EGLint *num_config);

		[DllImport("libEGL")]
		private static extern EGLSurface eglCreateWindowSurface(EGLDisplay dpy, EGLConfig config, EGLNativeWindowType dummyWindow, EGLint[] attrib_list);

		[DllImport("libEGL")]
		private static extern EGLContext eglCreateContext(EGLDisplay dpy, EGLConfig config, EGLContext share_context, EGLint[] attrib_list);

		[DllImport("libEGL")]
		private static extern EGLBoolean eglMakeCurrent(EGLDisplay dpy, EGLSurface draw, EGLSurface read, EGLContext ctx);

		[DllImport("libGLES2")]
		private static extern void glClearColor(float red, float green, float blue, float alpha);

		[DllImport("libGLES2")]
		private static extern void glClear(uint glbitfield);

		private static uint GL_COLOR_BUFFER_BIT = 0x00004000;
		private static IntPtr EGL_DEFAULT_DISPLAY = (IntPtr)0;
		private static EGLint EGL_SUCCESS = 0x3000;
		private static EGLBoolean EGL_TRUE = 1;
		private static EGLBoolean EGL_FALSE = 0;
		private static EGLint EGL_ALPHA_SIZE = 0x3021;
		private static EGLint EGL_BLUE_SIZE	= 0x3022;
		private static EGLint EGL_GREEN_SIZE = 0x3023;
		private static EGLint EGL_RED_SIZE = 0x3024;
		private static EGLint EGL_NONE = 0x3038;
		private static EGLint EGL_CONTEXT_CLIENT_VERSION = 0x3098;

		private static JSObject window;

        private static Action<double> LoopAction = new Action<double>(loop);

        private static int iteration_counter = 0;
        private static int iteration_counter_global = 0;
        private static System.Diagnostics.Stopwatch glClear_watch = new System.Diagnostics.Stopwatch();
        private static double glClear_elapsed_ms = 0;
        private static double glClear_elapsed_ms_global = 0;

        private static double iteration_elapsed_ms = 0;
        private static double iteration_elapsed_ms_global = 0;
        private static double previous_timestamp_ms = 0;

        private static void Main()
        {
            window = (JSObject)Runtime.GetGlobalObject();

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

            if (initEGl())
            {

				onCreate();

                //set_main_loop();
                loop(0);
            }
            else
            {
                System.Console.WriteLine("WebGL Init failed!");
            }

            //on_terminate();
        }

        private static bool initEGl()
        {
			EGLDisplay display = eglGetDisplay(EGL_DEFAULT_DISPLAY);
			if (eglGetError() != EGL_SUCCESS)
			{
				throw new Exception("Error getting display: " + eglGetError());
			}

			EGLint major = 0, minor = 0;
			EGLBoolean ret = eglInitialize(display, &major, &minor);
			if (eglGetError() != EGL_SUCCESS || ret != EGL_TRUE || major * 10000 + minor < 10004)
			{
				var ex = new Exception("Error Initializing EGL: " + eglGetError().ToString("X4") + ", ret: " + ret + " major: " + major + " minor: " + minor);
				throw ex;
			}

			EGLint numConfigs;
			ret = eglGetConfigs(display, null, 0, &numConfigs);
			if (eglGetError() != EGL_SUCCESS || ret != EGL_TRUE)
			{
				throw new Exception("Error getting configs: " + eglGetError());
			}

			EGLint[] attribs = {
				EGL_RED_SIZE, 5,
				EGL_GREEN_SIZE, 6,
				EGL_BLUE_SIZE, 5,
				EGL_NONE
			};
			EGLConfig config;
			ret = eglChooseConfig(display, attribs, &config, 1, &numConfigs);
			if (eglGetError() != EGL_SUCCESS || ret != EGL_TRUE)
			{
				throw new Exception("Error choosing config: " + eglGetError());
			}

			EGLNativeWindowType dummyWindow = 0;
			EGLSurface surface = eglCreateWindowSurface(display, config, dummyWindow, null);
			if (eglGetError() != EGL_SUCCESS || surface.ToInt32() == 0)
			{
				throw new Exception("Error creating window surface: " + eglGetError());
			}

			// The correct attributes, should create a good EGL context
			EGLint[] contextAttribs = {
				EGL_CONTEXT_CLIENT_VERSION, 2,
				EGL_NONE
			};
            EGLContext context = eglCreateContext(display, config, (IntPtr)0, contextAttribs);
			if (eglGetError() != EGL_SUCCESS || context.ToInt32() == 0)
			{
				throw new Exception("Error creating context: " + eglGetError());
			}

			ret = eglMakeCurrent(display, surface, surface, context);
			if (eglGetError() != EGL_SUCCESS || ret != EGL_TRUE)
			{
				throw new Exception("Error making current context: " + eglGetError());
			}

			return true;
		}

		private static void onCreate()
		{
			glClearColor(1, 0, 0, 0.5f);
			glClear(GL_COLOR_BUFFER_BIT);
		}

        private static void loop(double timestampMilliseconds)
        {
            glClear_watch.Restart();
            glClear(GL_COLOR_BUFFER_BIT);
            glClear_watch.Stop();

            iteration_counter++;
            iteration_elapsed_ms += (timestampMilliseconds - previous_timestamp_ms);
            previous_timestamp_ms = timestampMilliseconds;
            glClear_elapsed_ms += glClear_watch.Elapsed.TotalMilliseconds;
            if (iteration_counter >= 6000)
            {
                iteration_counter_global += iteration_counter;
                glClear_elapsed_ms_global += glClear_elapsed_ms;
                iteration_elapsed_ms_global += iteration_elapsed_ms;
                System.Console.WriteLine($"glClear: {glClear_elapsed_ms / iteration_counter} ms, mean: {glClear_elapsed_ms_global / iteration_counter_global} ms");
                System.Console.WriteLine($"FPS: {(iteration_counter / iteration_elapsed_ms)*1000}, mean: {(iteration_counter_global/ iteration_elapsed_ms_global)*1000}");
                iteration_counter = 0;
                glClear_elapsed_ms = 0;
                iteration_elapsed_ms = 0;
            }
            window.Invoke("requestAnimationFrame", LoopAction);
        }
    }
}
