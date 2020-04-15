#!/bin/bash
set -e

emcc wrapper.cpp -std=c++17 -s LEGALIZE_JS_FFI=0 -r -o ./eglwrapper.bc -s WASM=1 -s DISABLE_EXCEPTION_CATCHING=0
