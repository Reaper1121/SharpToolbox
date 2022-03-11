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
using System.Threading;
using Reaper1121.SharpToolbox.Extensions;
using Reaper1121.SharpToolbox.Utilities;

namespace Reaper1121.SharpToolbox.Collections {

    public sealed class ConcurrentCircularBuffer<T> : ICircularBuffer<T> {

        public bool IsSynchronized => true;
        public object SyncRoot => throw new NotSupportedException("The synchronisation is done internally by the concurrent circular buffer!");

        private readonly object PushLock = new object();
        private readonly object PopLock = new object();
        private readonly SemaphoreSlim AvailableItemSemaphore;

        private readonly T[] _Buffer;
        private volatile int LastPoppedItem;
        private volatile int _Count;
        public int Count => _Count;
        private readonly int _Capacity;
        public int Capacity => _Capacity;
        private int LastItem;

        public T this[int Arg_Index] {
            get {
                Monitor.Enter(PushLock);
                try {
                    return _Buffer[Arg_Index];
                } finally { Monitor.Exit(PushLock); }
            }
            set {
                Monitor.Enter(PushLock);
                Monitor.Enter(PopLock);
                try {
                    _Buffer[Arg_Index] = value;
                } finally {
                    Monitor.Exit(PopLock);
                    Monitor.Exit(PushLock);
                }
            }
        }
        public T[] Buffer {
            get {
                Monitor.Enter(PushLock);
                try {
                    return _Buffer.Clone<T>();
                } finally { Monitor.Exit(PushLock); }
            }
        }

        public ConcurrentCircularBuffer(int Arg_Capacity) {
            if (Arg_Capacity > 2) {
                _Capacity = Arg_Capacity;
                LastItem = Arg_Capacity - 1;
                LastPoppedItem = Arg_Capacity - 1;
                _Buffer = new T[Arg_Capacity];
                AvailableItemSemaphore = new SemaphoreSlim(0, Arg_Capacity);
            } else { throw new ArgumentOutOfRangeException(nameof(Arg_Capacity), "The capacity must be at least of 2!"); }
        }

        public void Push(T Arg_Item) {
            if (TryPush(Arg_Item, Timeout.Infinite) == false) { throw new InvalidOperationException("Failed to pop item from the buffer!"); }
        }

        public bool TryPush(T Arg_Item, int Arg_Timeout = 0) {
            if (Arg_Timeout >= -1) {
                bool Func_ExitStatus = true;
                Monitor.Enter(PushLock);
                int Func_LastItem = LastItem;
                if (_Count != 0 && Func_LastItem == LastPoppedItem && (Func_ExitStatus = Arg_Timeout != 0) == true) {
                    int Func_StartTime = Environment.TickCount;
                    while (Func_LastItem == LastPoppedItem) {
                        Thread.Yield();
                        if (Arg_Timeout != -1 && (Environment.TickCount - Func_StartTime) >= Arg_Timeout) {
                            Func_ExitStatus = false;
                            break;
                        }
                    }
                }
                if (Func_ExitStatus == true) {
                    if (++Func_LastItem == _Capacity) { Func_LastItem = 0; }
                    _Buffer[Func_LastItem] = Arg_Item;
                    LastItem = Func_LastItem;
                    Interlocked.Increment(ref _Count);
                    AvailableItemSemaphore.Release();
                }
                Monitor.Exit(PushLock);
                return Func_ExitStatus;
            } else { throw new ArgumentOutOfRangeException(nameof(Arg_Timeout), Arg_Timeout, "The timeout cannot be less than -1 (Infinite)!"); }
        }

        public T Pop() => TryPop(out T Func_Item, Timeout.Infinite) == true ? Func_Item : throw new InvalidOperationException("Failed to pop item from the buffer!");
        public void Pop(out T Func_Item) {
            if (TryPop(out Func_Item, Timeout.Infinite) == false) { throw new InvalidOperationException("Failed to pop item from the buffer!"); }
        }

        public bool TryPop(out T Arg_Item, int Arg_Timeout = 0) {
            if (Arg_Timeout >= -1) {
                bool Func_ExitStatus;
                if (AvailableItemSemaphore.Wait(Arg_Timeout) == true) {
                    Monitor.Enter(PopLock);
                    int Func_NextPopItem = LastPoppedItem + 1;
                    if (Func_NextPopItem == _Capacity) { Func_NextPopItem = 0; }
                    Arg_Item = _Buffer[Func_NextPopItem];
                    if (System.Runtime.CompilerServices.RuntimeHelpers.IsReferenceOrContainsReferences<T>() == true) { _Buffer[Func_NextPopItem] = default!; }
                    Interlocked.Decrement(ref _Count);
                    LastPoppedItem = Func_NextPopItem;
                    Monitor.Exit(PopLock);
                    Func_ExitStatus = true;
                } else {
                    Arg_Item = default!;
                    Func_ExitStatus = false;
                }
                return Func_ExitStatus;
            } else { throw new ArgumentOutOfRangeException(nameof(Arg_Timeout), Arg_Timeout, "The timeout cannot be less than -1 (Infinite)!"); }
        }

        public void Clear() {
            Monitor.Enter(PushLock);
            SemaphoreSlim Func_Semaphore = AvailableItemSemaphore;
            for (int Loop_Index = Func_Semaphore.CurrentCount - 1; Loop_Index != -1; --Loop_Index) { Func_Semaphore.Wait(0); }
            Monitor.Enter(PopLock);
            _Count = 0;
            LastPoppedItem = LastItem;
            Monitor.Exit(PopLock);
            Monitor.Exit(PushLock);
        }

        public void CopyTo(Array Arg_Array, int Arg_Index) {
            Monitor.Enter(PushLock);
            try {
                T[] Func_Buffer = _Buffer;
                Array.Copy(Func_Buffer, 0, Arg_Array, Arg_Index, Func_Buffer.Length);
            } finally { Monitor.Exit(PushLock); }
        }

        public void CopyTo(T[] Arg_Array, int Arg_Index) => CopyTo(UnsafeUtils.Reinterpret<T[], Array>(Arg_Array), Arg_Index);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new ArrayEnumerator<T>(Buffer);
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => new ArrayEnumerator<T>(Buffer);

    }

}
