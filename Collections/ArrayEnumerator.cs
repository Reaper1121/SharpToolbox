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

namespace Reaper1121.SharpToolbox.Collections {

    public sealed class ArrayEnumerator<T> : IEnumerator<T> {

        public T Current { get { return Items[CurrentIndex]; } }
        object System.Collections.IEnumerator.Current { get { return Items[CurrentIndex]; } }
        private readonly T[] Items;
        private int CurrentIndex = -1;

        public ArrayEnumerator(T[] Arg_Items) {
            Items = Arg_Items ?? throw new ArgumentNullException(nameof(Arg_Items));
        }

        public void Dispose() { }

        public bool MoveNext() {
            bool Func_ExitStatus = false;
            int Func_CurrentIndex = CurrentIndex + 1;
            if (Func_CurrentIndex < Items.Length) {
                CurrentIndex = Func_CurrentIndex;
                Func_ExitStatus = true;
            }
            return Func_ExitStatus;
        }

        public void Reset() {
            CurrentIndex = -1;
        }

    }

}
