#include <stdio.h>
#include <chrono>
#include <GL/glfw.h>

#define WASM_EXPORT __attribute__((visibility("default")))

extern "C"
{
	WASM_EXPORT void test_wrapper();
	WASM_EXPORT bool init_gl();
	WASM_EXPORT void on_create();
	WASM_EXPORT void do_frame();
	WASM_EXPORT void on_terminate();
}

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

	return true;
}

WASM_EXPORT void on_create()
{
	auto t1 = std::chrono::high_resolution_clock::now();
	glClearColor(1.0f, 0.0f, 0.0f, 0.0f);
	auto t2 = std::chrono::high_resolution_clock::now();

	//printf("glClearColor: %lld ns\n", std::chrono::duration_cast<std::chrono::nanoseconds>(t2 - t1).count());
}

WASM_EXPORT void do_frame()
{
	/* Render here */
	auto t1 = std::chrono::high_resolution_clock::now();
	glClear(GL_COLOR_BUFFER_BIT);
	auto t2 = std::chrono::high_resolution_clock::now();

	//printf("glClear: %lld ns\n", std::chrono::duration_cast<std::chrono::nanoseconds>(t2 - t1).count());

	/* Swap front and back buffers */
	t1 = std::chrono::high_resolution_clock::now();
	glfwSwapBuffers();
	t2 = std::chrono::high_resolution_clock::now();

	//printf("glfwSwapBuffers: %lld ns\n", std::chrono::duration_cast<std::chrono::nanoseconds>(t2 - t1).count());

	/* Poll for and process events */
	//glfwPollEvents();
}

WASM_EXPORT void on_terminate()
{
	glfwTerminate();
}