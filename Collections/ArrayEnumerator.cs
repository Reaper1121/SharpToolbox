using System;
using System.Collections.Generic;

namespace Reaper1121.SharpToolbox.Collections;

public sealed class ArrayEnumerator<T> : IEnumerator<T> {

    public T Current => Items[CurrentIndex];
    object System.Collections.IEnumerator.Current => Current!;
    private readonly T[] Items;
    private int CurrentIndex = -1;

    public ArrayEnumerator(T[] Arg_Items) => Items = Arg_Items ?? throw new ArgumentNullException(nameof(Arg_Items));

    public bool MoveNext() {
        bool Func_ExitStatus = false;
        int Func_CurrentIndex = CurrentIndex + 1;
        if (Func_CurrentIndex < Items.Length) {
            CurrentIndex = Func_CurrentIndex;
            Func_ExitStatus = true;
        }
        return Func_ExitStatus;
    }

    public void Reset() => CurrentIndex = -1;

    void IDisposable.Dispose() { }

}
