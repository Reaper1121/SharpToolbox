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
using InlineIL;

namespace Reaper1121.SharpToolbox.Utilities {

    /// <summary>
    /// Provides various dangerous low-level utilities, don't use this unless you know what you're doing.
    /// </summary>
    [SkipLocalsInit]
    public static class UnsafeUtils {

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
                throw new InvalidProgramException("The unsafe method expected a reference-type generic object!");
            }
            IL.Push(Arg_Object);
            IL.Emit.Ldnull();
            IL.Emit.Ceq();
            return IL.Return<bool>();
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
            return IL.Return<DT>();
        }

        /// <summary>
        /// Tells the compiler that the object address is of a different type at compile-time
        /// <para />
        /// WARNING: This does NOT change the object runtime type.
        /// </summary>
        /// <typeparam name="ST">Source Type</typeparam>
        /// <typeparam name="DT">Destination Type</typeparam>
        /// <param name="Arg_Object">The object address of source type</param>
        /// <returns>The object address of destination type</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref DT Reinterpret<ST, DT>(ref ST Arg_Object) {
            IL.Push(ref Arg_Object!);
            return ref IL.ReturnRef<DT>();
        }

    }

}
