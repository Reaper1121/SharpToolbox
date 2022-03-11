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

namespace Reaper1121.SharpToolbox {

    /// <summary>
    /// A object wrapping GC allocated memory for unmanaged use.
    /// <para/>
    /// The memory gets automatically collected when GCMemory goes out of scope, just like regular GC objects.
    /// </summary>
    [SkipLocalsInit]
    public unsafe class GCMemory<T> where T : unmanaged {

        /// <summary>
        /// GC allocated memory for unmanaged use.
        /// </summary>
        public readonly T[] Memory;
        /// <summary>
        /// A pinned pointer to GC allocated memory
        /// </summary>
        public readonly T* UnsafePointer;
        /// <summary>
        /// The amount of allocated bytes
        /// </summary>
        public readonly ulong Size;

        /// <summary>
        /// Allocates GC memory for unmanaged use.
        /// </summary>
        /// <param name="Arg_ArrayLength">The amount of elements.</param>
        /// <param name="Arg_ZeroInitialize">Initialize the newly GC allocated memory to 0's</param>
        public GCMemory(int Arg_ArrayLength = 1, bool Arg_ZeroInitialize = true) {
            if (Arg_ArrayLength > 0) {
                if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() == false) {
                    Memory = Arg_ZeroInitialize == false ? GC.AllocateUninitializedArray<T>(1, true) : GC.AllocateArray<T>(1, true);
                    UnsafePointer = (T*) Unsafe.AsPointer(ref System.Runtime.InteropServices.MemoryMarshal.GetArrayDataReference(Memory));
                    Size = (ulong) sizeof(T) * (ulong) Arg_ArrayLength;
                } else { throw new ArgumentException("The GC allocated memory for unmanaged use cannot contain managed object references!"); }
            } else { throw new ArgumentException("The array length must be at least 1!", nameof(Arg_ArrayLength)); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator T[]?(GCMemory<T>? Arg_GCMemory) => Arg_GCMemory?.Memory;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T*(GCMemory<T>? Arg_GCMemory) => Arg_GCMemory != null ? Arg_GCMemory.UnsafePointer : null;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator IntPtr(GCMemory<T>? Arg_GCMemory) => Arg_GCMemory != null ? new IntPtr(Arg_GCMemory.UnsafePointer) : IntPtr.Zero;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator UIntPtr(GCMemory<T>? Arg_GCMemory) => Arg_GCMemory != null ? new UIntPtr(Arg_GCMemory.UnsafePointer) : UIntPtr.Zero;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator nuint(GCMemory<T>? Arg_GCMemory) => Arg_GCMemory != null ? (nuint) Arg_GCMemory.UnsafePointer : 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator nint(GCMemory<T>? Arg_GCMemory) => Arg_GCMemory != null ? (nint) Arg_GCMemory.UnsafePointer : 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ulong(GCMemory<T>? Arg_GCMemory) => Arg_GCMemory != null ? (ulong) Arg_GCMemory.UnsafePointer : 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator long(GCMemory<T>? Arg_GCMemory) => Arg_GCMemory != null ? (long) Arg_GCMemory.UnsafePointer : 0;

    }

}
