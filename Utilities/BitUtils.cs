/*
    MIT License

    Copyright (C) 2021 Martynas Skirmantas https://github.com/Reaper1121/SharpToolbox

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using System;
using System.Runtime.CompilerServices;

namespace Reaper1121.SharpToolbox.Utilities {

    /// <summary>
    /// Provides various bit operations
    /// </summary>
    [SkipLocalsInit]
    public static class BitUtils {

        /// <summary>
        /// Reads bits from specified range
        /// </summary>
        /// <param name="Arg_Value">The value to read from</param>
        /// <param name="Arg_StartBit">Starting bit index</param>
        /// <param name="Arg_BitCount">The amount of bits to read from starting bit</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadBits(byte Arg_Value, int Arg_StartBit, uint Arg_BitCount) {
            if (((ulong) Arg_StartBit + Arg_BitCount) <= 8) {
                return (byte) ReadBitsUnsafe(Arg_Value, Arg_StartBit, Arg_BitCount);
            } else { throw new ArgumentOutOfRangeException(null, Arg_StartBit + Arg_BitCount, $"{nameof(Arg_StartBit)} + {nameof(Arg_BitCount)} is outside value bit range."); }
        }

        /// <summary>
        /// Reads bits from specified range
        /// </summary>
        /// <param name="Arg_Value">The value to read from</param>
        /// <param name="Arg_StartBit">Starting bit index</param>
        /// <param name="Arg_BitCount">The amount of bits to read from starting bit</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadBits(ushort Arg_Value, int Arg_StartBit, uint Arg_BitCount) {
            if (((ulong) Arg_StartBit + Arg_BitCount) <= 16) {
                return (ushort) ReadBitsUnsafe(Arg_Value, Arg_StartBit, Arg_BitCount);
            } else { throw new ArgumentOutOfRangeException(null, Arg_StartBit + Arg_BitCount, $"{nameof(Arg_StartBit)} + {nameof(Arg_BitCount)} is outside value bit range."); }
        }

        /// <summary>
        /// Reads bits from specified range
        /// </summary>
        /// <param name="Arg_Value">The value to read from</param>
        /// <param name="Arg_StartBit">Starting bit index</param>
        /// <param name="Arg_BitCount">The amount of bits to read from starting bit</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadBits(uint Arg_Value, int Arg_StartBit, uint Arg_BitCount) {
            if (((ulong) Arg_StartBit + Arg_BitCount) <= 32) {
                return ReadBitsUnsafe(Arg_Value, Arg_StartBit, Arg_BitCount);
            } else { throw new ArgumentOutOfRangeException(null, Arg_StartBit + Arg_BitCount, $"{nameof(Arg_StartBit)} + {nameof(Arg_BitCount)} is outside value bit range."); }
        }

        /// <summary>
        /// Reads bits from specified range
        /// </summary>
        /// <param name="Arg_Value">The value to read from</param>
        /// <param name="Arg_StartBit">Starting bit index</param>
        /// <param name="Arg_BitCount">The amount of bits to read from starting bit</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadBits(ulong Arg_Value, int Arg_StartBit, uint Arg_BitCount) {
            if (((ulong) Arg_StartBit + Arg_BitCount) <= 64) {
                return ReadBitsUnsafe(Arg_Value, Arg_StartBit, Arg_BitCount);
            } else { throw new ArgumentOutOfRangeException(null, Arg_StartBit + Arg_BitCount, $"{nameof(Arg_StartBit)} + {nameof(Arg_BitCount)} is outside value bit range."); }
        }

        /// <summary>
        /// Reads bits from specified range without parameter validation
        /// </summary>
        /// <param name="Arg_Value">The value to read from</param>
        /// <param name="Arg_StartBit">Starting bit index</param>
        /// <param name="Arg_BitCount">The amount of bits to read from starting bit</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadBitsUnsafe(uint Arg_Value, int Arg_StartBit, uint Arg_BitCount) {
            Arg_BitCount = 32 - Arg_BitCount;
            Arg_Value >>= Arg_StartBit;
            Arg_Value <<= (int) Arg_BitCount;
            Arg_Value >>= (int) Arg_BitCount;
            return Arg_Value;
        }

        /// <summary>
        /// Reads bits from specified range without parameter validation
        /// </summary>
        /// <param name="Arg_Value">The value to read from</param>
        /// <param name="Arg_StartBit">Starting bit index</param>
        /// <param name="Arg_BitCount">The amount of bits to read from starting bit</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadBitsUnsafe(ulong Arg_Value, int Arg_StartBit, uint Arg_BitCount) {
            Arg_BitCount = 64 - Arg_BitCount;
            Arg_Value >>= Arg_StartBit;
            Arg_Value <<= (int) Arg_BitCount;
            Arg_Value >>= (int) Arg_BitCount;
            return Arg_Value;
        }

        /// <summary>
        /// Writes bits to specified range
        /// </summary>
        /// <param name="Arg_Value">The value to write to</param>
        /// <param name="Arg_StartBit">Starting bit index</param>
        /// <param name="Arg_BitCount">The amount of bits to read from starting bit</param>
        /// <param name="Arg_Bits">The bits to write of the specified range</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte WriteBits(byte Arg_Value, byte Arg_Bits, int Arg_StartBit, uint Arg_BitCount) {
            if (((ulong) Arg_StartBit + Arg_BitCount) <= 8) {
                return (byte) WriteBitsUnsafe(Arg_Value, Arg_Bits, Arg_StartBit, Arg_BitCount);
            } else { throw new ArgumentOutOfRangeException(null, Arg_StartBit + Arg_BitCount, $"{nameof(Arg_StartBit)} + {nameof(Arg_BitCount)} is outside value bit range."); }
        }

        /// <summary>
        /// Writes bits to specified range
        /// </summary>
        /// <param name="Arg_Value">The value to write to</param>
        /// <param name="Arg_StartBit">Starting bit index</param>
        /// <param name="Arg_BitCount">The amount of bits to read from starting bit</param>
        /// <param name="Arg_Bits">The bits to write of the specified range</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort WriteBits(ushort Arg_Value, ushort Arg_Bits, int Arg_StartBit, uint Arg_BitCount) {
            if (((ulong) Arg_StartBit + Arg_BitCount) <= 16) {
                return (ushort) WriteBitsUnsafe(Arg_Value, Arg_Bits, Arg_StartBit, Arg_BitCount);
            } else { throw new ArgumentOutOfRangeException(null, Arg_StartBit + Arg_BitCount, $"{nameof(Arg_StartBit)} + {nameof(Arg_BitCount)} is outside value bit range."); }
        }

        /// <summary>
        /// Writes bits to specified range
        /// </summary>
        /// <param name="Arg_Value">The value to write to</param>
        /// <param name="Arg_StartBit">Starting bit index</param>
        /// <param name="Arg_BitCount">The amount of bits to read from starting bit</param>
        /// <param name="Arg_Bits">The bits to write of the specified range</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint WriteBits(uint Arg_Value, uint Arg_Bits, int Arg_StartBit, uint Arg_BitCount) {
            if (((ulong) Arg_StartBit + Arg_BitCount) <= 32) {
                return WriteBitsUnsafe(Arg_Value, Arg_Bits, Arg_StartBit, Arg_BitCount);
            } else { throw new ArgumentOutOfRangeException(null, Arg_StartBit + Arg_BitCount, $"{nameof(Arg_StartBit)} + {nameof(Arg_BitCount)} is outside value bit range."); }
        }

        /// <summary>
        /// Writes bits to specified range
        /// </summary>
        /// <param name="Arg_Value">The value to write to</param>
        /// <param name="Arg_StartBit">Starting bit index</param>
        /// <param name="Arg_BitCount">The amount of bits to read from starting bit</param>
        /// <param name="Arg_Bits">The bits to write of the specified range</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong WriteBits(ulong Arg_Value, ulong Arg_Bits, int Arg_StartBit, uint Arg_BitCount) {
            if (((ulong) Arg_StartBit + Arg_BitCount) <= 64) {
                return WriteBitsUnsafe(Arg_Value, Arg_Bits, Arg_StartBit, Arg_BitCount);
            } else { throw new ArgumentOutOfRangeException(null, Arg_StartBit + Arg_BitCount, $"{nameof(Arg_StartBit)} + {nameof(Arg_BitCount)} is outside value bit range."); }
        }

        /// <summary>
        /// Writes bits to specified range without parameter validation
        /// </summary>
        /// <param name="Arg_Value">The value to write to</param>
        /// <param name="Arg_StartBit">Starting bit index</param>
        /// <param name="Arg_BitCount">The amount of bits to read from starting bit</param>
        /// <param name="Arg_Bits">The bits to write of the specified range</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint WriteBitsUnsafe(uint Arg_Value, uint Arg_Bits, int Arg_StartBit, uint Arg_BitCount) {
            Arg_BitCount = 32 - Arg_BitCount;
            Arg_Bits >>= Arg_StartBit;
            Arg_Bits <<= (int) Arg_BitCount;
            Arg_Bits >>= (int) Arg_BitCount - Arg_StartBit;
            return Arg_Value |= Arg_Bits;
        }

        /// <summary>
        /// Writes bits to specified range without parameter validation
        /// </summary>
        /// <param name="Arg_Value">The value to write to</param>
        /// <param name="Arg_StartBit">Starting bit index</param>
        /// <param name="Arg_BitCount">The amount of bits to read from starting bit</param>
        /// <param name="Arg_Bits">The bits to write of the specified range</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong WriteBitsUnsafe(ulong Arg_Value, ulong Arg_Bits, int Arg_StartBit, uint Arg_BitCount) {
            Arg_BitCount = 64 - Arg_BitCount;
            Arg_Bits >>= Arg_StartBit;
            Arg_Bits <<= (int) Arg_BitCount;
            Arg_Bits >>= (int) Arg_BitCount - Arg_StartBit;
            return Arg_Value |= Arg_Bits;
        }

        /// <summary>
        /// Converts a value into binary string
        /// </summary>
        /// <param name="Arg_Value">Value to convert to a binary string</param>
        /// <returns>Binary string</returns>
        public static string ToString(byte Arg_Value) {
            char[] Func_BitString = new char[8];
            for (int Loop_Index = 0; Loop_Index < 8; ++Loop_Index) {
                Func_BitString[Loop_Index] = IsSet(Arg_Value, Loop_Index) ? '1' : '0';
            }
            return new string(Func_BitString);
        }

        /// <summary>
        /// Converts a value into binary string
        /// </summary>
        /// <param name="Arg_Value">Value to convert to a binary string</param>
        /// <returns>Binary string</returns>
        public static string ToString(ushort Arg_Value) {
            char[] Func_BitString = new char[16];
            for (int Loop_Index = 0; Loop_Index < 16; ++Loop_Index) {
                Func_BitString[Loop_Index] = IsSet(Arg_Value, Loop_Index) ? '1' : '0';
            }
            return new string(Func_BitString);
        }

        /// <summary>
        /// Converts a value into binary string
        /// </summary>
        /// <param name="Arg_Value">Value to convert to a binary string</param>
        /// <returns>Binary string</returns>
        public static string ToString(uint Arg_Value) {
            char[] Func_BitString = new char[32];
            for (int Loop_Index = 0; Loop_Index < 32; ++Loop_Index) {
                Func_BitString[Loop_Index] = IsSet(Arg_Value, Loop_Index) ? '1' : '0';
            }
            return new string(Func_BitString);
        }

        /// <summary>
        /// Converts a value into binary string
        /// </summary>
        /// <param name="Arg_Value">Value to convert to a binary string</param>
        /// <returns>Binary string</returns>
        public static string ToString(ulong Arg_Value) {
            char[] Func_BitString = new char[64];
            for (int Loop_Index = 0; Loop_Index < 64; ++Loop_Index) {
                Func_BitString[Loop_Index] = IsSet(Arg_Value, Loop_Index) ? '1' : '0';
            }
            return new string(Func_BitString);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSet(byte Arg_Value, int Arg_BitIndex) => IsSet((uint) Arg_Value, Arg_BitIndex);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSet(ushort Arg_Value, int Arg_BitIndex) => IsSet((uint) Arg_Value, Arg_BitIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSet(uint Arg_Value, int Arg_BitIndex) => ((Arg_Value >> Arg_BitIndex) & 1U) == 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSet(ulong Arg_Value, int Arg_BitIndex) => ((Arg_Value >> Arg_BitIndex) & 1UL) == 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set(ref byte Arg_Value, int Arg_BitIndex) => Arg_Value = (byte) (Arg_Value | (1U << Arg_BitIndex));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Set(byte Arg_Value, int Arg_BitIndex) => (byte) (Arg_Value | (1U << Arg_BitIndex));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set(ref ushort Arg_Value, int Arg_BitIndex) => Arg_Value = (ushort) (Arg_Value | (1U << Arg_BitIndex));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Set(ushort Arg_Value, int Arg_BitIndex) => (ushort) (Arg_Value | (1U << Arg_BitIndex));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set(ref uint Arg_Value, int Arg_BitIndex) => Arg_Value |= 1U << Arg_BitIndex;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Set(uint Arg_Value, int Arg_BitIndex) => Arg_Value |= 1U << Arg_BitIndex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set(ref ulong Arg_Value, int Arg_BitIndex) => Arg_Value |= 1UL << Arg_BitIndex;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Set(ulong Arg_Value, int Arg_BitIndex) => Arg_Value |= 1UL << Arg_BitIndex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Toggle(ref byte Arg_Value, int Arg_BitIndex) => Arg_Value = (byte) (Arg_Value ^ (1U << Arg_BitIndex));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Toggle(byte Arg_Value, int Arg_BitIndex) => (byte) (Arg_Value ^ (1U << Arg_BitIndex));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Toggle(ref ushort Arg_Value, int Arg_BitIndex) => Arg_Value = (ushort) (Arg_Value ^ (1U << Arg_BitIndex));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Toggle(ushort Arg_Value, int Arg_BitIndex) => (ushort) (Arg_Value ^ (1U << Arg_BitIndex));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Toggle(ref uint Arg_Value, int Arg_BitIndex) => Arg_Value ^= 1U << Arg_BitIndex;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Toggle(uint Arg_Value, int Arg_BitIndex) => Arg_Value ^= 1U << Arg_BitIndex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Toggle(ref ulong Arg_Value, int Arg_BitIndex) => Arg_Value ^= 1UL << Arg_BitIndex;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Toggle(ulong Arg_Value, int Arg_BitIndex) => Arg_Value ^= 1UL << Arg_BitIndex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reset(ref byte Arg_Value, int Arg_BitIndex) => Arg_Value = (byte) (Arg_Value & ~(1U << Arg_BitIndex));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Reset(byte Arg_Value, int Arg_BitIndex) => (byte) (Arg_Value & ~(1U << Arg_BitIndex));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reset(ref ushort Arg_Value, int Arg_BitIndex) => Arg_Value = (ushort) (Arg_Value & ~(1U << Arg_BitIndex));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Reset(ushort Arg_Value, int Arg_BitIndex) => (ushort) (Arg_Value & ~(1U << Arg_BitIndex));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reset(ref uint Arg_Value, int Arg_BitIndex) => Arg_Value &= ~(1U << Arg_BitIndex);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Reset(uint Arg_Value, int Arg_BitIndex) => Arg_Value &= ~(1U << Arg_BitIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Reset(ref ulong Arg_Value, int Arg_BitIndex) => Arg_Value &= ~(1UL << Arg_BitIndex);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Reset(ulong Arg_Value, int Arg_BitIndex) => Arg_Value &= ~(1UL << Arg_BitIndex);

    }

}
