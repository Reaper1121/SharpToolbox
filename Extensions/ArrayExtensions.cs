using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using InlineIL;
using Reaper1121.SharpToolbox.Utilities;

namespace Reaper1121.SharpToolbox.Extensions;

[SkipLocalsInit]
public static class ArrayExtensions {

    /// <summary>
    /// Reads the array at specified index without the array bounds check
    /// </summary>
    /// <typeparam name="T">Array element type</typeparam>
    /// <param name="Arg_Array">The array to read</param>
    /// <param name="Arg_Index">The array element index to read at</param>
    /// <returns>The read array element</returns>
    /// <exception cref="NullReferenceException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T UncheckedRead<T>(this T[] Arg_Array, int Arg_Index) => Unsafe.Add(ref System.Runtime.InteropServices.MemoryMarshal.GetArrayDataReference(Arg_Array), Arg_Index);

    /// <summary>
    /// Clones a array faster than built-in clone
    /// </summary>
    /// <typeparam name="T">Array element type</typeparam>
    /// <param name="Arg_Array">The array to clone</param>
    /// <exception cref="NullReferenceException"></exception>
    public static T[] Clone<T>(this T[] Arg_Array) => UnsafeUtils.Reinterpret<object, T[]>(Arg_Array.Clone());

    /// <summary>
    /// Converts the array compile-time type to a different type without making a copy/clone of the array.
    /// </summary>
    /// <typeparam name="ST">Source array element type</typeparam>
    /// <typeparam name="DT">Destination array element type </typeparam>
    /// <param name="Arg_SourceArray">The array to convert</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static DT[] ConvertType<ST, DT>(this ST[] Arg_SourceArray) where ST : class where DT : class {
        ArgumentNullException.ThrowIfNull(Arg_SourceArray);
        if (typeof(DT) != typeof(ST)) {
            for (int Loop_Index = Arg_SourceArray.Length - 1; Loop_Index != -1; --Loop_Index) {
                object? Loop_SourceObject = UnsafeUtils.Reinterpret<ST, object>(Arg_SourceArray[Loop_Index]);
                if (Loop_SourceObject != null && Loop_SourceObject.GetType() != typeof(DT)) {
                    throw new InvalidCastException("The source array has a object that is not of destination type!");
                }
            }
        }
        IL.Emit.Ldarg(nameof(Arg_SourceArray));
        return IL.Return<DT[]>();
    }

    /// <summary>
    /// Converts the array type to a different type without making a copy/clone of the array.
    /// <para/>
    /// WARNING: This function SKIPS type checks and should not be used unless you know what you're doing!
    /// </summary>
    /// <typeparam name="ST">Source array element type</typeparam>
    /// <typeparam name="DT">Destination array element type </typeparam>
    /// <param name="Arg_SourceArray">The array to convert</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DT[] ConvertTypeUnsafe<ST, DT>(this ST[] Arg_SourceArray) where ST : class where DT : class {
        IL.Push(Arg_SourceArray);
        return IL.Return<DT[]>();
    }

    /// <summary>
    /// Converts the array type to a different type and makes a copy/clone of the array.
    /// </summary>
    /// <typeparam name="ST">Source array element type</typeparam>
    /// <typeparam name="DT">Destination array element type </typeparam>
    /// <param name="Arg_SourceArray">The array to convert</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static DT[] CloneConvertType<ST, DT>(this ST[] Arg_SourceArray) where ST : class where DT : class {
        ArgumentNullException.ThrowIfNull(Arg_SourceArray);
        DT[] Func_ConvertedArray = new DT[Arg_SourceArray.Length];
        for (int Loop_Index = Arg_SourceArray.Length - 1; Loop_Index != -1; --Loop_Index) {
            object? Loop_SourceItem = UnsafeUtils.Reinterpret<ST, object?>(Arg_SourceArray[Loop_Index]);
            if (Loop_SourceItem != null) {
                if (Loop_SourceItem.GetType() == typeof(DT)) {
                    IL.Push(Func_ConvertedArray);
                    IL.Push(Loop_Index);
                    IL.Push(Loop_SourceItem);
                    IL.Emit.Stelem_Any<DT?>();
                } else { throw new InvalidCastException("The source array has a object that is not of destination type!"); }
            }
        }
        return Func_ConvertedArray;
    }

    /// <summary>
    /// Converts the array type to a different type and makes a copy/clone of the array.
    /// <para/>
    /// WARNING: This function SKIPS type checks and should not be used unless you know what you're doing!
    /// </summary>
    /// <typeparam name="ST">Source array element type</typeparam>
    /// <typeparam name="DT">Destination array element type </typeparam>
    /// <param name="Arg_SourceArray">The array to convert</param>
    /// <exception cref="ArgumentNullException"></exception>
    public static DT[] CloneConvertTypeUnsafe<ST, DT>(this ST[] Arg_SourceArray) where ST : class where DT : class {
        ArgumentNullException.ThrowIfNull(Arg_SourceArray);
        DT[] Func_ConvertedArray = new DT[Arg_SourceArray.Length];
        for (int Loop_Index = Arg_SourceArray.Length - 1; Loop_Index != -1; --Loop_Index) {
            IL.Push(Func_ConvertedArray);
            IL.Push(Loop_Index);
            IL.Push(Arg_SourceArray);
            IL.Push(Loop_Index);
            IL.Emit.Ldelem_Any<ST>();
            IL.Emit.Stelem_Any<DT>();
        }
        return Func_ConvertedArray;
    }

    /// <summary>
    /// Compares if the array of a class element type by reference is a exact match! This includes array length.
    /// </summary>
    /// <typeparam name="T">Array element type</typeparam>
    /// <param name="Arg_Array1">The first compared array</param>
    /// <param name="Arg_Array2">The second compared array</param>
    /// <returns><see langword="true"/> when the compared arrays match!</returns>
    public static bool CompareByRef<T>(this IList<T> Arg_Array1, IList<T> Arg_Array2) where T : class {
        bool Func_ExitStatus = Arg_Array1 != Arg_Array2;
        if (Func_ExitStatus == true && (Func_ExitStatus = Arg_Array1 != null && Arg_Array2 != null && Arg_Array1.Count == Arg_Array2.Count) == true) {
            for (int Loop_Index = Arg_Array1!.Count - 1; Loop_Index != -1; --Loop_Index) {
                if (ReferenceEquals(UnsafeUtils.Reinterpret<T, object?>(Arg_Array1[Loop_Index]), UnsafeUtils.Reinterpret<T, object?>(Arg_Array2![Loop_Index])) == false) {
                    Func_ExitStatus = false;
                    break;
                }
            }
        }
        return Func_ExitStatus;
    }
    public static bool CompareByRef<T>(this IList<T> Arg_Array1, IList<T> Arg_Array2, int Arg_ItemCount) where T : class {
        bool Func_ExitStatus = Arg_Array1 != Arg_Array2;
        if (Func_ExitStatus == true && (Func_ExitStatus = Arg_Array1 != null && Arg_Array2 != null && Arg_Array1.Count <= Arg_ItemCount && Arg_Array2.Count <= Arg_ItemCount) == true) {
            for (--Arg_ItemCount; Arg_ItemCount != -1; --Arg_ItemCount) {
                if (ReferenceEquals(UnsafeUtils.Reinterpret<T, object?>(Arg_Array1![Arg_ItemCount]), UnsafeUtils.Reinterpret<T, object?>(Arg_Array2![Arg_ItemCount])) == false) {
                    Func_ExitStatus = false;
                    break;
                }
            }
        }
        return Func_ExitStatus;
    }

    public static bool Compare<T>(this IList<T> Arg_Array1, IList<T> Arg_Array2) {
        bool Func_ExitStatus = Arg_Array1 != Arg_Array2;
        if (Func_ExitStatus == true && (Func_ExitStatus = Arg_Array1 != null && Arg_Array2 != null && Arg_Array1.Count == Arg_Array2.Count) == true) {
            EqualityComparer<T> Func_ValueComparer = EqualityComparer<T>.Default;
            for (int Loop_Index = Arg_Array1!.Count - 1; Loop_Index != -1; --Loop_Index) {
                if (Func_ValueComparer.Equals(Arg_Array1[Loop_Index], Arg_Array2![Loop_Index]) == false) {
                    Func_ExitStatus = false;
                    break;
                }
            }
        }
        return Func_ExitStatus;
    }
    public static bool Compare<T>(this IList<T> Arg_Array1, IList<T> Arg_Array2, int Arg_ItemCount) {
        bool Func_ExitStatus = Arg_Array1 != Arg_Array2;
        if (Func_ExitStatus == true && (Func_ExitStatus = Arg_Array1 != null && Arg_Array2 != null && Arg_Array1.Count >= Arg_ItemCount && Arg_Array2.Count >= Arg_ItemCount) == true) {
            EqualityComparer<T> Func_ValueComparer = EqualityComparer<T>.Default;
            for (--Arg_ItemCount; Arg_ItemCount != -1; --Arg_ItemCount) {
                if (Func_ValueComparer.Equals(Arg_Array1![Arg_ItemCount], Arg_Array2![Arg_ItemCount]) == false) {
                    Func_ExitStatus = false;
                    break;
                }
            }
        }
        return Func_ExitStatus;
    }

    /// <summary>
    /// Compares if the array of a IEquatable element type is a exact match! This includes array length.
    /// </summary>
    /// <typeparam name="T">Array element type</typeparam>
    /// <param name="Arg_Array1">The first compared array</param>
    /// <param name="Arg_Array2">The second compared array</param>
    /// <returns><see langword="true"/> when the compared arrays match!</returns>
    public static bool CompareEquatable<T>(this IList<T> Arg_Array1, IList<T> Arg_Array2) where T : IEquatable<T> {
        bool Func_ExitStatus = Arg_Array1 != Arg_Array2;
        if (Func_ExitStatus == false && (Func_ExitStatus = Arg_Array1 != null && Arg_Array2 != null && Arg_Array1.Count == Arg_Array2.Count) == true) {
            for (int Loop_Index = Arg_Array1!.Count - 1; Loop_Index != -1; --Loop_Index) {
                if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() == true) {
                    IEquatable<T> Loop_CompareObject = UnsafeUtils.Reinterpret<T, IEquatable<T>>(Arg_Array1[Loop_Index]);
                    if (Loop_CompareObject != null) {
                        if (Loop_CompareObject.Equals(Arg_Array2![Loop_Index]) == false) {
                            Func_ExitStatus = false;
                            break;
                        }
                    }
                } else {
                    if (Arg_Array1[Loop_Index].Equals(Arg_Array2![Loop_Index]) == false) {
                        Func_ExitStatus = false;
                        break;
                    }
                }
            }
        }
        return Func_ExitStatus;
    }

}
