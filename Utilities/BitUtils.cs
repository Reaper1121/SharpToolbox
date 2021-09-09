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

using System.Runtime.CompilerServices;

namespace Reaper1121.SharpToolbox.Utilities {

    /// <summary>
    /// Provides various bit operations
    /// </summary>
    [SkipLocalsInit]
    public static class BitUtils {

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
