#include "FastSIMD/FastSIMD.h"

#define SIMDPP_ARCH_X86_AVX512BW 1
#define SIMDPP_ARCH_X86_AVX512DQ 1
#define SIMDPP_ARCH_X86_AVX512VL 1

#if FASTSIMD_COMPILE_AVX512 

// To compile AVX512 support enable AVX512 code generation compiler flags for this file
#ifndef __AVX512DQ__ 
#ifdef _MSC_VER
#error To compile AVX512 set C++ code generation to use /arch:AVX512 on FastSIMD_Level_AVX512.cpp, or change "#define FASTSIMD_COMPILE_AVX512" in FastSIMD_Config.h
#else
#error To compile AVX512 add build command "-mavx512f -mavx512dq" on FastSIMD_Level_AVX512.cpp, or change "#define FASTSIMD_COMPILE_AVX512" in FastSIMD_Config.h
#endif
#endif

#include "Internal/AVX512.h"
#define FS_SIMD_CLASS FastSIMD::AVX512
#include "Internal/SourceBuilder.inl"
#endif