#include <stdio.h>

#define WASM_EXPORT __attribute__((visibility("default")))

extern "C"
{
	WASM_EXPORT void TestWrapper();
}

WASM_EXPORT void TestWrapper()
{
	printf("Hello, I'm the C++ wrapper!!\n");
}
