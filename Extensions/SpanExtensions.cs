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
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Reaper1121.SharpToolbox.Extensions {

    [SkipLocalsInit]
    public static class SpanExtensions {

        public static bool Compare<T>(this IList<T> Arg_Array1, Span<T> Arg_Array2) {
            bool Func_ExitStatus = Arg_Array1 != null && Arg_Array2 != null && Arg_Array1.Count == Arg_Array2.Length;
            if (Func_ExitStatus == true) {
                EqualityComparer<T> Func_ValueComparer = EqualityComparer<T>.Default;
                for (int Loop_Index = Arg_Array1!.Count - 1; Loop_Index != -1; --Loop_Index) {
                    if (Func_ValueComparer.Equals(Arg_Array1[Loop_Index], Arg_Array2[Loop_Index]) == false) {
                        Func_ExitStatus = false;
                        break;
                    }
                }
            }
            return Func_ExitStatus;
        }
        public static bool Compare<T>(this IList<T> Arg_Array1, Span<T> Arg_Array2, int Arg_ItemCount) {
            bool Func_ExitStatus = Arg_Array1 != null && Arg_Array2 != null && Arg_Array1.Count <= Arg_ItemCount && Arg_Array2.Length <= Arg_ItemCount;
            if (Func_ExitStatus == true) {
                EqualityComparer<T> Func_ValueComparer = EqualityComparer<T>.Default;
                for (--Arg_ItemCount; Arg_ItemCount != -1; --Arg_ItemCount) {
                    if (Func_ValueComparer.Equals(Arg_Array1![Arg_ItemCount], Arg_Array2[Arg_ItemCount]) == false) {
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
        /// <param name="Arg_Array2">The second compared span array</param>
        /// <returns><see langword="true"/> when the compared arrays match!</returns>
        public static bool CompareEquatable<T>(this IList<T> Arg_Array1, Span<T> Arg_Array2) where T : IEquatable<T> {
            bool Func_ExitStatus = Arg_Array1 != null && Arg_Array2 != null && Arg_Array1.Count == Arg_Array2.Length;
            if (Func_ExitStatus == true) {
                for (int Loop_Index = Arg_Array1!.Count - 1; Loop_Index != -1; --Loop_Index) {
                    if (Arg_Array1[Loop_Index].Equals(Arg_Array2[Loop_Index]) == false) {
                        Func_ExitStatus = false;
                        break;
                    }
                }
            }
            return Func_ExitStatus;
        }

        public static void CopyTo<T>(this Span<T> Arg_SourceArray, IList<T> Arg_DestinationArray) {
            if (Arg_DestinationArray != null) {
                int Func_SourceArrayLength = Arg_SourceArray.Length;
                if (Func_SourceArrayLength <= Arg_DestinationArray.Count) {
                    for (--Func_SourceArrayLength; Func_SourceArrayLength != -1; --Func_SourceArrayLength) {
                        Arg_DestinationArray[Func_SourceArrayLength] = Arg_SourceArray[Func_SourceArrayLength];
                    }
                } else { throw new ArgumentException("The destination array is too small to hold all source array elements!", nameof(Arg_DestinationArray)); }
            } else { throw new ArgumentNullException(nameof(Arg_DestinationArray)); }
        }

        public static void CopyTo<T>(this Span<T> Arg_SourceArray, IList<T> Arg_DestinationArray, int Arg_DestinationIndex) {
            if (Arg_DestinationArray != null) {
                if (Arg_DestinationIndex > -1 && Arg_DestinationIndex < Arg_DestinationArray.Count) {
                    int Func_SourceArrayLength = Arg_SourceArray.Length;
                    if (Arg_DestinationIndex + Func_SourceArrayLength <= Arg_DestinationArray.Count) {
                        for (--Func_SourceArrayLength; Func_SourceArrayLength != -1; --Func_SourceArrayLength) {
                            Arg_DestinationArray[Arg_DestinationIndex++] = Arg_SourceArray[Func_SourceArrayLength];
                        }
                    } else { throw new ArgumentException("The destination array from specified destination index is too small to hold all source array elements!", nameof(Arg_DestinationArray)); }
                } else { throw new ArgumentOutOfRangeException(nameof(Arg_DestinationIndex), Arg_DestinationIndex, "The destination index is outside destination array range!"); }
            } else { throw new ArgumentNullException(nameof(Arg_DestinationArray)); }
        }

    }

}
