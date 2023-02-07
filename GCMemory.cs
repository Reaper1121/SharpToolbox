using System;
using System.Runtime.CompilerServices;

namespace Reaper1121.SharpToolbox;

/// <summary>
/// A object wrapping GC allocated memory for unmanaged use.
/// <para/>
/// The memory gets automatically collected when GCMemory goes out of scope, just like regular GC objects.
/// </summary>
[SkipLocalsInit]
public unsafe class GCMemory<T> where T : unmanaged {

    /// <summary>
    /// GC allocated memory for unmanaged use
    /// </summary>
    private readonly ulong[] Memory;
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
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() == true) {
            throw new ArgumentException("The GC allocated memory for unmanaged use cannot contain managed object references!");
        }
        if (Arg_ArrayLength < 0) { Arg_ArrayLength = 0; }
        ulong Func_Size = AlignValue(Size = (ulong) sizeof(T) * (ulong) Arg_ArrayLength, 8);
        Memory = Arg_ZeroInitialize == false ? GC.AllocateUninitializedArray<ulong>((int) (Func_Size / 8), true) : GC.AllocateArray<ulong>((int) (Func_Size / 8), true); // NOTE: ulong is used to mitigate C# int.MaxValue array length limit issue
        UnsafePointer = (T*) Unsafe.AsPointer(ref System.Runtime.InteropServices.MemoryMarshal.GetArrayDataReference(Memory));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ulong AlignValue(ulong Arg_Value, ulong Arg_Alignment) {
            ulong Func_ValueRemainder = Arg_Value % Arg_Alignment;
            return Func_ValueRemainder == 0 ? Arg_Value : Arg_Value + (Arg_Alignment - Func_ValueRemainder);
        }

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T*(GCMemory<T>? Arg_GCMemory) => Arg_GCMemory != null ? Arg_GCMemory.UnsafePointer : null;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator IntPtr(GCMemory<T>? Arg_GCMemory) => Arg_GCMemory != null ? new IntPtr(Arg_GCMemory.UnsafePointer) : IntPtr.Zero;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator UIntPtr(GCMemory<T>? Arg_GCMemory) => Arg_GCMemory != null ? new UIntPtr(Arg_GCMemory.UnsafePointer) : UIntPtr.Zero;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator nuint(GCMemory<T>? Arg_GCMemory) => Arg_GCMemory != null ? (nuint) Arg_GCMemory.UnsafePointer : 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator nint(GCMemory<T>? Arg_GCMemory) => Arg_GCMemory != null ? (nint) Arg_GCMemory.UnsafePointer : 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator ulong(GCMemory<T>? Arg_GCMemory) => Arg_GCMemory != null ? (ulong) Arg_GCMemory.UnsafePointer : 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator long(GCMemory<T>? Arg_GCMemory) => Arg_GCMemory != null ? (long) Arg_GCMemory.UnsafePointer : 0;

}
