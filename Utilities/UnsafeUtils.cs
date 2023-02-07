using System;
using System.Runtime.CompilerServices;
using InlineIL;
using Reaper1121.SharpToolbox.Collections;

namespace Reaper1121.SharpToolbox.Utilities;

/// <summary>
/// Provides various dangerous low-level utilities, don't use this unless you know what you're doing.
/// </summary>
[SkipLocalsInit]
public static class UnsafeUtils {

    /// <summary>
    /// Retrieves the underlying array of a fast list
    /// </summary>
    /// <param name="Arg_List">The list of items</param>
    /// <typeparam name="T">The list item type</typeparam>
    /// <returns>The underlying array of the fast list</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] GetUnderlyingArray<T>(FastList<T> Arg_List) => Arg_List.InnerArray;

    /// <summary>
    /// Checks if a generic reference-type object is null
    /// </summary>
    /// <typeparam name="T">Generic reference-type object</typeparam>
    /// <param name="Arg_Object">The generic reference-type object that may be null.</param>
    /// <exception cref="InvalidProgramException">The generic object type is not a reference-type.</exception>
    /// <returns><see langword="true"/> when the object reference is null.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNull<T>(T? Arg_Object) {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() == false) {
            IL.Emit.Ldc_I4_0();
        } else {
            IL.Push(Arg_Object);
            IL.Emit.Ldnull();
            IL.Emit.Ceq();
        }
        IL.Emit.Ret();
        throw IL.Unreachable();
    }

    /// <summary>
    /// Tells the compiler that the object is of a different type at compile-time
    /// <para />
    /// WARNING: This does NOT change the object runtime type.
    /// </summary>
    /// <typeparam name="ST">Source Type</typeparam>
    /// <typeparam name="DT">Destination Type</typeparam>
    /// <param name="Arg_Object">The object of source type</param>
    /// <returns>The object of destination type</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DT Reinterpret<ST, DT>(ST Arg_Object) {
        IL.Push(Arg_Object);
        IL.Emit.Ret();
        throw IL.Unreachable();
    }

    /// <summary>
    /// Tells the compiler that the object reference is of a different type at compile-time
    /// <para />
    /// WARNING: This does NOT change the object runtime type.
    /// </summary>
    /// <typeparam name="ST">Source Type</typeparam>
    /// <typeparam name="DT">Destination Type</typeparam>
    /// <param name="Arg_Object">The object address of source type</param>
    /// <returns>The object address of destination type</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref DT ReinterpretRef<ST, DT>(ref ST Arg_Object) {
        IL.Push(ref Arg_Object!);
        IL.Emit.Ret();
        throw IL.Unreachable();
    }

    /// <summary>
    /// Tells the compiler that the object reference is of a different type at compile-time
    /// <para />
    /// WARNING: This does NOT change the object runtime type.
    /// </summary>
    /// <typeparam name="ST">Source Type</typeparam>
    /// <typeparam name="DT">Destination Type</typeparam>
    /// <param name="Arg_Object">The object address of source type</param>
    /// <returns>The object address of destination type</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly DT ReinterpretIn<ST, DT>(in ST Arg_Object) {
        IL.PushInRef(in Arg_Object);
        IL.Emit.Ret();
        throw IL.Unreachable();
    }

    /// <summary>
    /// Tells the C# runtime that the object is of a different type
    /// <para />
    /// </summary>
    /// <typeparam name="T">Destination Type</typeparam>
    /// <param name="Arg_Object">The object of source type</param>
    /// <returns>The object of destination type</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ReinterpretCast<T>(object Arg_Object) where T : class {
        IL.DeclareLocals(false, new LocalVar[] {
            new LocalVar(TypeRef.Type<RuntimeTypeHandle>())
        });
        IL.Push(Arg_Object);
        IL.Emit.Ldtoken<T>();
        IL.Emit.Stloc_0();
        IL.Emit.Ldloca_S(0);
        IL.Emit.Call(MethodRef.PropertyGet(TypeRef.Type<RuntimeTypeHandle>(), "Value"));
        IL.Emit.Call(MethodRef.Operator(TypeRef.Type<IntPtr>(), ConversionOperator.Explicit, ConversionDirection.To, TypeRef.Type<long>()));
        IL.Emit.Stind_I8();
        IL.Push(Arg_Object);
        IL.Emit.Ret();
        throw IL.Unreachable();
    }

}
