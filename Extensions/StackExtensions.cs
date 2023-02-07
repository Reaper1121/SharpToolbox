using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Reaper1121.SharpToolbox.Extensions;

[SkipLocalsInit]
public static class StackExtensions {

    /// <summary>
    /// Pushes an array of items to a stack, all changes are undone when a exception occurs.
    /// </summary>
    /// <typeparam name="T">Stack item type</typeparam>
    /// <param name="Arg_SourceStack">The stack to push items to</param>
    /// <param name="Arg_Items">The item array that will be pushed to the stack</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="OutOfMemoryException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ConstrainedPushRange<T>(this Stack<T> Arg_SourceStack, T[] Arg_Items) {
        ArgumentNullException.ThrowIfNull(Arg_SourceStack);
        ArgumentNullException.ThrowIfNull(Arg_Items);
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
        ArgumentNullException.ThrowIfNull(Arg_SourceStack);
        ArgumentNullException.ThrowIfNull(Arg_Items);
        if (Arg_ItemCount > 0) {
            if (Arg_Items.Length < Arg_ItemCount) { throw new ArgumentOutOfRangeException(nameof(Arg_ItemCount), Arg_ItemCount, "The item count is greater than the items array length!"); }
            uint Func_PushedItems = 0;
            try {
                for (int Loop_Index = 0; Loop_Index < Arg_ItemCount; ++Loop_Index) {
                    Arg_SourceStack.Push(Arg_Items[Loop_Index]);
                    ++Func_PushedItems;
                }
            } catch {
                for (; Func_PushedItems != 0; --Func_PushedItems) {
                    Arg_SourceStack.TryPop(out T _);
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
        ArgumentNullException.ThrowIfNull(Arg_SourceStack);
        if (Arg_ItemCount > 0) {
            if (Arg_SourceStack.Count < Arg_ItemCount) { throw new ArgumentOutOfRangeException(nameof(Arg_ItemCount), Arg_ItemCount, "The item count is greater than the stack count!"); }
            for (; Arg_ItemCount != 0; --Arg_ItemCount) {
                Arg_SourceStack.TryPop(out T _);
            }
        }
    }

}
