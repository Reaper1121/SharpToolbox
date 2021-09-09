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
using System.Collections.Generic;

namespace MartynasSkirmantas.SharpToolbox.Extensions {

    [SkipLocalsInit]
    public static class StackExtensions {

        /// <summary>
        /// Pushes an array of items to a stack, all changes are undone when exception occurs.
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="Arg_SourceStack">The stack to push items to</param>
        /// <param name="Arg_Items">The item array that will be pushed to the stack</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="OutOfMemoryException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ConstrainedPushRange<T>(this Stack<T> Arg_SourceStack, T[] Arg_Items) {
            if (Arg_Items == null) { throw new ArgumentNullException(nameof(Arg_Items)); }
            ConstrainedPushRange(Arg_SourceStack, Arg_Items, Arg_Items.Length);
        }

        /// <summary>
        /// Pushes an array of items to a stack, all changes are undone when exception occurs.
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="Arg_SourceStack">The stack to push items to</param>
        /// <param name="Arg_Items">The item array that will be pushed to the stack</param>
        /// <param name="Arg_ItemCount">The amount of items that will be pushed to the stack</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="OutOfMemoryException"></exception>
        public static void ConstrainedPushRange<T>(this Stack<T> Arg_SourceStack, T[] Arg_Items, int Arg_ItemCount) {
            if (Arg_SourceStack == null) { throw new ArgumentNullException(nameof(Arg_SourceStack)); }
            if (Arg_ItemCount > 0) {
                if (Arg_Items == null) { throw new ArgumentNullException(nameof(Arg_Items)); }
                if (Arg_Items.Length < Arg_ItemCount) { throw new ArgumentOutOfRangeException(nameof(Arg_ItemCount), Arg_ItemCount, "The item count is greater than the items array length!"); }
                uint Func_PushedItems = 0;
                try {
                    for (int Loop_Index = 0; Loop_Index < Arg_ItemCount; ++Loop_Index) {
                        Arg_SourceStack.Push(Arg_Items[Loop_Index]);
                        ++Func_PushedItems;
                    }
                } catch {
                    for (; Func_PushedItems != 0; --Func_PushedItems) {
                        Arg_SourceStack.TryPop(out T unused);
                    }
                    throw;
                }
            }
        }

        /// <summary>
        /// Pops a number of items from the stack
        /// </summary>
        /// <typeparam name="T">Stack item type</typeparam>
        /// <param name="Arg_SourceStack">The stack to pop items from</param>
        /// <param name="Arg_ItemCount">The amount of items that will be popped from the stack</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Pop<T>(this Stack<T> Arg_SourceStack, int Arg_ItemCount) {
            if (Arg_SourceStack == null) { throw new ArgumentNullException(nameof(Arg_SourceStack)); }
            if (Arg_ItemCount > 0) {
                if (Arg_SourceStack.Count < Arg_ItemCount) { throw new ArgumentOutOfRangeException(nameof(Arg_ItemCount), Arg_ItemCount, "The item count is greater than the stack count!"); }
                for (; Arg_ItemCount != 0; --Arg_ItemCount) {
                    Arg_SourceStack.TryPop(out T unused);
                }
            }
        }

    }

}
