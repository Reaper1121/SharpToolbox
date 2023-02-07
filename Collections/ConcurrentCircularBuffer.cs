using System;
using System.Collections.Generic;
using System.Threading;
using Reaper1121.SharpToolbox.Extensions;
using Reaper1121.SharpToolbox.Utilities;

namespace Reaper1121.SharpToolbox.Collections;

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
        if (TryPush(Arg_Item, Timeout.Infinite) == false) { throw new InvalidOperationException("Failed to push item to the buffer!"); }
    }
    public void Push(T Arg_Item, int Arg_Timeout) {
        if (TryPush(Arg_Item, Arg_Timeout) == false) { throw new InvalidOperationException("Failed to push item to the buffer!"); }
    }

    public bool TryPush(T Arg_Item) => TryPush(Arg_Item, Timeout.Infinite);
    public bool TryPush(T Arg_Item, int Arg_Timeout) {
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

    public T Pop() {
        Pop(out T Func_Item);
        return Func_Item;
    }
    public T Pop(int Arg_Timeout) {
        Pop(out T Func_Item, Arg_Timeout);
        return Func_Item;
    }
    public T Pop(CancellationToken Arg_CancellationToken) {
        Pop(out T Func_Item, Arg_CancellationToken);
        return Func_Item;
    }
    public T Pop(int Arg_Timeout, CancellationToken Arg_CancellationToken) {
        Pop(out T Func_Item, Arg_Timeout, Arg_CancellationToken);
        return Func_Item;
    }

    public void Pop(out T Func_Item) {
        if (TryPop(out Func_Item) == false) { throw new InvalidOperationException("Failed to pop item from the buffer!"); }
    }
    public void Pop(out T Func_Item, int Arg_Timeout) {
        if (TryPop(out Func_Item, Arg_Timeout) == false) { throw new InvalidOperationException("Failed to pop item from the buffer!"); }
    }
    public void Pop(out T Func_Item, CancellationToken Arg_CancellationToken) {
        if (TryPop(out Func_Item, Timeout.Infinite, Arg_CancellationToken) == false) { throw new InvalidOperationException("Failed to pop item from the buffer!"); }
    }
    public void Pop(out T Func_Item, int Arg_Timeout, CancellationToken Arg_CancellationToken) {
        if (TryPop(out Func_Item, Arg_Timeout, Arg_CancellationToken) == false) { throw new InvalidOperationException("Failed to pop item from the buffer!"); }
    }

    public bool TryPop(out T Arg_Item) => TryPop(out Arg_Item, Timeout.Infinite, CancellationToken.None);
    public bool TryPop(out T Arg_Item, int Arg_Timeout) => TryPop(out Arg_Item, Arg_Timeout, CancellationToken.None);
    public bool TryPop(out T Arg_Item, CancellationToken Arg_CancellationToken) => TryPop(out Arg_Item, Timeout.Infinite, Arg_CancellationToken);
    public bool TryPop(out T Arg_Item, int Arg_Timeout, CancellationToken Arg_CancellationToken) {
        if (Arg_Timeout >= -1) {
            bool Func_ExitStatus = false;
            try {
                Func_ExitStatus = AvailableItemSemaphore.Wait(Arg_Timeout, Arg_CancellationToken);
            } catch (OperationCanceledException) { }
            if (Func_ExitStatus == true) {
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
            }
            return Func_ExitStatus;
        } else { throw new ArgumentOutOfRangeException(nameof(Arg_Timeout), Arg_Timeout, "The timeout cannot be less than -1 (Infinite)!"); }
    }

    public T Peek() {
        Peek(out T Func_Item);
        return Func_Item;
    }
    public T Peek(int Arg_Timeout) {
        Peek(out T Func_Item, Arg_Timeout);
        return Func_Item;
    }
    public T Peek(CancellationToken Arg_CancellationToken) {
        Peek(out T Func_Item, Arg_CancellationToken);
        return Func_Item;
    }
    public T Peek(int Arg_Timeout, CancellationToken Arg_CancellationToken) {
        Peek(out T Func_Item, Arg_Timeout, Arg_CancellationToken);
        return Func_Item;
    }

    public void Peek(out T Func_Item) {
        if (TryPeek(out Func_Item) == false) { throw new InvalidOperationException("Failed to peek item from the buffer!"); }
    }
    public void Peek(out T Func_Item, int Arg_Timeout) {
        if (TryPeek(out Func_Item, Arg_Timeout) == false) { throw new InvalidOperationException("Failed to peek item from the buffer!"); }
    }
    public void Peek(out T Func_Item, CancellationToken Arg_CancellationToken) {
        if (TryPeek(out Func_Item, Arg_CancellationToken) == false) { throw new InvalidOperationException("Failed to peek item from the buffer!"); }
    }
    public void Peek(out T Func_Item, int Arg_Timeout, CancellationToken Arg_CancellationToken) {
        if (TryPeek(out Func_Item, Arg_Timeout, Arg_CancellationToken) == false) { throw new InvalidOperationException("Failed to peek item from the buffer!"); }
    }

    public bool TryPeek(out T Arg_Item) => TryPeek(out Arg_Item, Timeout.Infinite, CancellationToken.None);
    public bool TryPeek(out T Arg_Item, int Arg_Timeout) => TryPeek(out Arg_Item, Arg_Timeout, CancellationToken.None);
    public bool TryPeek(out T Arg_Item, CancellationToken Arg_CancellationToken) => TryPeek(out Arg_Item, Timeout.Infinite, Arg_CancellationToken);
    public bool TryPeek(out T Arg_Item, int Arg_Timeout, CancellationToken Arg_CancellationToken) {
        if (Arg_Timeout >= -1) {
            bool Func_ExitStatus = false;
            try {
                Func_ExitStatus = AvailableItemSemaphore.Wait(Arg_Timeout, Arg_CancellationToken);
            } catch (OperationCanceledException) { }
            if (Func_ExitStatus == true) {
                Monitor.Enter(PopLock);
                int Func_NextPopItem = LastPoppedItem + 1;
                if (Func_NextPopItem == _Capacity) { Func_NextPopItem = 0; }
                Arg_Item = _Buffer[Func_NextPopItem];
                Monitor.Exit(PopLock);
                AvailableItemSemaphore.Release();
                Func_ExitStatus = true;
            } else {
                Arg_Item = default!;
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
