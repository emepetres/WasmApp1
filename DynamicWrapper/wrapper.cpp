#include <stdio.h>
#include <chrono>
#include <emscripten/emscripten.h>
#include <GL/glfw.h>
#include <GL/glext.h>
//#include <SDL/SDL_opengl.h>

#define WASM_EXPORT __attribute__((visibility("default")))

typedef void (*csharpFunc)(void);

typedef void (*LPGLCLEAR)(GLbitfield);
LPGLCLEAR lpGlClear;

extern "C"
{
	WASM_EXPORT void test_wrapper();
	WASM_EXPORT bool init_gl();
	WASM_EXPORT void on_create();
	WASM_EXPORT void set_main_loop();
	WASM_EXPORT void do_frame();
	WASM_EXPORT void on_terminate();
}

int gl_clear_counter;
long gl_clear_counter_global;
double time_counter;
double time_counter_global;

void do_frame();

WASM_EXPORT void test_wrapper()
{
	printf("Hello, I'm the C++ wrapper!!\n");
}

WASM_EXPORT bool init_gl()
{
	const int width = 480,
						height = 800;

	if (glfwInit() != GL_TRUE)
	{
		printf("glfwInit() failed\n");
		return false;
	}

	if (glfwOpenWindow(width, height, 8, 8, 8, 8, 16, 0, GLFW_WINDOW) != GL_TRUE)
	{
		printf("glfwOpenWindow() failed\n");
		return false;
	}

	gl_clear_counter = 0;
	time_counter = 0;
	gl_clear_counter_global = 0;
	time_counter_global = 0;

	lpGlClear = (LPGLCLEAR)glfwGetProcAddress("glClear");
	//printf("Pointer to glClear from glfw: %p\n", lpGlClear);

	//printf("Pointer to glClear from SDL: %p", SDL_GL_GetProcAddress("glClear"));

	return true;
}

WASM_EXPORT void on_create()
{
	//auto t1 = std::chrono::high_resolution_clock::now();
	glClearColor(1.0f, 0.0f, 0.0f, 0.0f);
	//auto t2 = std::chrono::high_resolution_clock::now();

	//printf("glClearColor: %lld ns\n", std::chrono::duration_cast<std::chrono::nanoseconds>(t2 - t1).count());
}

WASM_EXPORT void set_main_loop()
{
	emscripten_set_main_loop(do_frame, 0, 0);
}

WASM_EXPORT void do_frame()
{
	/* Render here */
	//	auto t1 = std::chrono::high_resolution_clock::now();
	//glClear(GL_COLOR_BUFFER_BIT);
	lpGlClear(GL_COLOR_BUFFER_BIT);
	//	auto t2 = std::chrono::high_resolution_clock::now();

	//gl_clear_counter++;
	//	time_counter += std::chrono::duration<double, std::milli>(t2 - t1).count();

	// if (gl_clear_counter == 6000)
	// {
	// 	time_counter_global += time_counter;
	// 	gl_clear_counter_global += gl_clear_counter;
	// 	printf("glClear: %lf ms, mean: %lf ms\n", time_counter / gl_clear_counter, time_counter_global / gl_clear_counter_global);
	// 	gl_clear_counter = 0;
	// 	time_counter = 0;
	// }

	/* Swap front and back buffers */
	//t1 = std::chrono::high_resolution_clock::now();
	//glfwSwapBuffers();
	//t2 = std::chrono::high_resolution_clock::now();

	//printf("glfwSwapBuffers: %lld ns\n", std::chrono::duration_cast<std::chrono::nanoseconds>(t2 - t1).count());

	/* Poll for and process events */
	//glfwPollEvents();
}

WASM_EXPORT void on_terminate()
{
	glfwTerminate();
}