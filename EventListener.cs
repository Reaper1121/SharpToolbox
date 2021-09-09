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
using System.Threading;
using Reaper1121.SharpToolbox.Utilities;
using Reaper1121.SharpToolbox.Extensions;
using Reaper1121.SharpToolbox.Collections;
using System.Runtime.CompilerServices;

namespace MartynasSkirmantas.SharpToolbox {

    // TODO: Implement event listener class that supports events with return value

    [SkipLocalsInit]
    public unsafe class EventListenerUnsafe {

        private readonly object SyncLock = new object();
        private volatile bool EventHandlersMutated;
        private readonly FastList<Delegate> MutableEventHandlers = new FastList<Delegate>();
        private Delegate[] EventHandlers = Array.Empty<Delegate>();

        public void AddHandler(Delegate Arg_Handler) {
            if (Arg_Handler != null) {
                Monitor.Enter(SyncLock);
                try {
                    MutableEventHandlers.Add(Arg_Handler);
                    EventHandlersMutated = true;
                } finally { Monitor.Exit(SyncLock); }
            } else { throw new ArgumentNullException(nameof(Arg_Handler)); }
        }

        public void AddHandlers(Delegate[] Arg_Handlers) {
            if (Arg_Handlers != null) {
                Monitor.Enter(SyncLock);
                try {
                    FastList<Delegate> Func_EventHandlers = MutableEventHandlers;
                    for (int Loop_HandlerIndex = Arg_Handlers.Length - 1; Loop_HandlerIndex != -1; --Loop_HandlerIndex) {
                        Delegate Loop_Handler = Arg_Handlers[Loop_HandlerIndex];
                        if (Loop_Handler != null) {
                            Func_EventHandlers.Add(Loop_Handler);
                        }
                    }
                    EventHandlersMutated = true;
                } finally { Monitor.Exit(SyncLock); }
            } else { throw new ArgumentNullException(nameof(Arg_Handlers)); }
        }

        public bool ContainsHandler(Delegate Arg_Handler) {
            if (Arg_Handler != null) {
                Monitor.Enter(SyncLock);
                bool Func_HandlerFound = MutableEventHandlers.Contains(Arg_Handler);
                Monitor.Exit(SyncLock);
                return Func_HandlerFound;
            } else { throw new ArgumentNullException(nameof(Arg_Handler)); }
        }

        public void RemoveHandler(Delegate Arg_Handler) {
            if (Arg_Handler != null) {
                Monitor.Enter(SyncLock);
                MutableEventHandlers.RemoveUnordered(Arg_Handler);
                EventHandlersMutated = true;
                Monitor.Exit(SyncLock);
            } else { throw new ArgumentNullException(nameof(Arg_Handler)); }
        }

        public void RemoveHandlers(Delegate[] Arg_Handlers) {
            if (Arg_Handlers != null) {
                Monitor.Enter(SyncLock);
                FastList<Delegate> Func_EventHandlers = MutableEventHandlers;
                for (int Loop_HandlerIndex = Arg_Handlers.Length - 1; Loop_HandlerIndex != -1; --Loop_HandlerIndex) {
                    Delegate Loop_Handler = Arg_Handlers[Loop_HandlerIndex];
                    if (Loop_Handler != null) {
                        Func_EventHandlers.RemoveUnordered(Loop_Handler);
                    }
                }
                EventHandlersMutated = true;
                Monitor.Exit(SyncLock);
            } else { throw new ArgumentNullException(nameof(Arg_Handlers)); }
        }

        public bool HasHandlers() {
            Monitor.Enter(SyncLock);
            bool Func_HasHandlers = MutableEventHandlers.Count != 0;
            Monitor.Exit(SyncLock);
            return Func_HasHandlers;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Delegate[] PrepareInvoking() {
            Delegate[] Func_EventHandlers = Volatile.Read(ref EventHandlers);
            if (EventHandlersMutated == true) {
                Monitor.Enter(SyncLock);
                try {
                    if (EventHandlersMutated == true) {
                        FastList<Delegate> Func_MutatedEventHandlers = MutableEventHandlers;
                        Volatile.Write(ref EventHandlers, Func_MutatedEventHandlers.Count != 0 ? Func_MutatedEventHandlers.ToArray() : Array.Empty<Delegate>());
                    }
                    Func_EventHandlers = EventHandlers;
                    EventHandlersMutated = false;
                } finally { Monitor.Exit(SyncLock); }
            }
            return Func_EventHandlers;
        }

        internal void UnsafeActionInvoke() {
            Delegate[] Func_EventHandlers = PrepareInvoking();
            for (int Loop_EventHandlerIndex = Func_EventHandlers.Length - 1; Loop_EventHandlerIndex != -1; --Loop_EventHandlerIndex) {
                UnsafeUtils.Reinterpret<Delegate, Action>(Func_EventHandlers.UncheckedRead(Loop_EventHandlerIndex))();
            }
        }
        internal void UnsafeActionInvoke<A1>(A1 Arg_Argument1) {
            Delegate[] Func_EventHandlers = PrepareInvoking();
            for (int Loop_EventHandlerIndex = Func_EventHandlers.Length - 1; Loop_EventHandlerIndex != -1; --Loop_EventHandlerIndex) {
                UnsafeUtils.Reinterpret<Delegate, Action<A1>>(Func_EventHandlers.UncheckedRead(Loop_EventHandlerIndex))(Arg_Argument1);
            }
        }
        internal void UnsafeActionInvoke<A1, A2>(A1 Arg_Argument1, A2 Arg_Argument2) {
            Delegate[] Func_EventHandlers = PrepareInvoking();
            for (int Loop_EventHandlerIndex = Func_EventHandlers.Length - 1; Loop_EventHandlerIndex != -1; --Loop_EventHandlerIndex) {
                UnsafeUtils.Reinterpret<Delegate, Action<A1, A2>>(Func_EventHandlers.UncheckedRead(Loop_EventHandlerIndex))(Arg_Argument1, Arg_Argument2);
            }
        }
        internal void UnsafeActionInvoke<A1, A2, A3>(A1 Arg_Argument1, A2 Arg_Argument2, A3 Arg_Argument3) {
            Delegate[] Func_EventHandlers = PrepareInvoking();
            for (int Loop_EventHandlerIndex = Func_EventHandlers.Length - 1; Loop_EventHandlerIndex != -1; --Loop_EventHandlerIndex) {
                UnsafeUtils.Reinterpret<Delegate, Action<A1, A2, A3>>(Func_EventHandlers.UncheckedRead(Loop_EventHandlerIndex))(Arg_Argument1, Arg_Argument2, Arg_Argument3);
            }
        }
        internal void UnsafeActionInvoke<A1, A2, A3, A4>(A1 Arg_Argument1, A2 Arg_Argument2, A3 Arg_Argument3, A4 Arg_Argument4) {
            Delegate[] Func_EventHandlers = PrepareInvoking();
            for (int Loop_EventHandlerIndex = Func_EventHandlers.Length - 1; Loop_EventHandlerIndex != -1; --Loop_EventHandlerIndex) {
                 UnsafeUtils.Reinterpret<Delegate, Action<A1, A2, A3, A4>>(Func_EventHandlers.UncheckedRead(Loop_EventHandlerIndex))(Arg_Argument1, Arg_Argument2, Arg_Argument3, Arg_Argument4);
            }
        }

        internal void UnsafeFuncInvoke<RT>() {
            Delegate[] Func_EventHandlers = PrepareInvoking();
            for (int Loop_EventHandlerIndex = Func_EventHandlers.Length - 1; Loop_EventHandlerIndex != -1; --Loop_EventHandlerIndex) {
                UnsafeUtils.Reinterpret<Delegate, Func<RT>>(Func_EventHandlers.UncheckedRead(Loop_EventHandlerIndex))();
            }
        }
        internal void UnsafeFuncInvoke<RT, A1>(A1 Arg_Argument1) {
            Delegate[] Func_EventHandlers = PrepareInvoking();
            for (int Loop_EventHandlerIndex = Func_EventHandlers.Length - 1; Loop_EventHandlerIndex != -1; --Loop_EventHandlerIndex) {
                UnsafeUtils.Reinterpret<Delegate, Func<A1, RT>>(Func_EventHandlers.UncheckedRead(Loop_EventHandlerIndex))(Arg_Argument1);
            }
        }
        internal void UnsafeFuncInvoke<RT, A1, A2>(A1 Arg_Argument1, A2 Arg_Argument2) {
            Delegate[] Func_EventHandlers = PrepareInvoking();
            for (int Loop_EventHandlerIndex = Func_EventHandlers.Length - 1; Loop_EventHandlerIndex != -1; --Loop_EventHandlerIndex) {
                UnsafeUtils.Reinterpret<Delegate, Func<A1, A2, RT>>(Func_EventHandlers.UncheckedRead(Loop_EventHandlerIndex))(Arg_Argument1, Arg_Argument2);
            }
        }
        internal void UnsafeFuncInvoke<RT, A1, A2, A3>(A1 Arg_Argument1, A2 Arg_Argument2, A3 Arg_Argument3) {
            Delegate[] Func_EventHandlers = PrepareInvoking();
            for (int Loop_EventHandlerIndex = Func_EventHandlers.Length - 1; Loop_EventHandlerIndex != -1; --Loop_EventHandlerIndex) {
                UnsafeUtils.Reinterpret<Delegate, Func<A1, A2, A3, RT>>(Func_EventHandlers.UncheckedRead(Loop_EventHandlerIndex))(Arg_Argument1, Arg_Argument2, Arg_Argument3);
            }
        }
        internal void UnsafeFuncInvoke<RT, A1, A2, A3, A4>(A1 Arg_Argument1, A2 Arg_Argument2, A3 Arg_Argument3, A4 Arg_Argument4) {
            Delegate[] Func_EventHandlers = PrepareInvoking();
            for (int Loop_EventHandlerIndex = Func_EventHandlers.Length - 1; Loop_EventHandlerIndex != -1; --Loop_EventHandlerIndex) {
                UnsafeUtils.Reinterpret<Delegate, Func<A1, A2, A3, A4, RT>>(Func_EventHandlers.UncheckedRead(Loop_EventHandlerIndex))(Arg_Argument1, Arg_Argument2, Arg_Argument3, Arg_Argument4);
            }
        }

        internal void UnsafeFuncInvokeCallback<RT>(Action<RT> Arg_InvokedCallback) {
            if (Arg_InvokedCallback != null) {
                Delegate[] Func_EventHandlers = PrepareInvoking();
                for (int Loop_EventHandlerIndex = Func_EventHandlers.Length - 1; Loop_EventHandlerIndex != -1; --Loop_EventHandlerIndex) {
                    Arg_InvokedCallback(UnsafeUtils.Reinterpret<Delegate, Func<RT>>(Func_EventHandlers.UncheckedRead(Loop_EventHandlerIndex))());
                }
            } else { throw new ArgumentNullException(nameof(Arg_InvokedCallback)); }
        }
        internal void UnsafeFuncInvokeCallback<RT, A1>(Action<RT, A1> Arg_InvokedCallback, A1 Arg_Argument1) {
            if (Arg_InvokedCallback != null) {
                Delegate[] Func_EventHandlers = PrepareInvoking();
                for (int Loop_EventHandlerIndex = Func_EventHandlers.Length - 1; Loop_EventHandlerIndex != -1; --Loop_EventHandlerIndex) {
                    Arg_InvokedCallback(UnsafeUtils.Reinterpret<Delegate, Func<A1, RT>>(Func_EventHandlers.UncheckedRead(Loop_EventHandlerIndex))(Arg_Argument1), Arg_Argument1);
                }
            } else { throw new ArgumentNullException(nameof(Arg_InvokedCallback)); }
        }
        internal void UnsafeFuncInvokeCallback<RT, A1, A2>(Action<RT, A1, A2> Arg_InvokedCallback, A1 Arg_Argument1, A2 Arg_Argument2) {
            if (Arg_InvokedCallback != null) {
                Delegate[] Func_EventHandlers = PrepareInvoking();
                for (int Loop_EventHandlerIndex = Func_EventHandlers.Length - 1; Loop_EventHandlerIndex != -1; --Loop_EventHandlerIndex) {
                    Arg_InvokedCallback(UnsafeUtils.Reinterpret<Delegate, Func<A1, A2, RT>>(Func_EventHandlers.UncheckedRead(Loop_EventHandlerIndex))(Arg_Argument1, Arg_Argument2), Arg_Argument1, Arg_Argument2);
                }
            } else { throw new ArgumentNullException(nameof(Arg_InvokedCallback)); }
        }
        internal void UnsafeFuncInvokeCallback<RT, A1, A2, A3>(Action<RT, A1, A2, A3> Arg_InvokedCallback, A1 Arg_Argument1, A2 Arg_Argument2, A3 Arg_Argument3) {
            if (Arg_InvokedCallback != null) {
                Delegate[] Func_EventHandlers = PrepareInvoking();
                for (int Loop_EventHandlerIndex = Func_EventHandlers.Length - 1; Loop_EventHandlerIndex != -1; --Loop_EventHandlerIndex) {
                    Arg_InvokedCallback(UnsafeUtils.Reinterpret<Delegate, Func<A1, A2, A3, RT>>(Func_EventHandlers.UncheckedRead(Loop_EventHandlerIndex))(Arg_Argument1, Arg_Argument2, Arg_Argument3), Arg_Argument1, Arg_Argument2, Arg_Argument3);
                }
            } else { throw new ArgumentNullException(nameof(Arg_InvokedCallback)); }
        }
        internal void UnsafeFuncInvokeCallback<RT, A1, A2, A3, A4>(Action<RT, A1, A2, A3, A4> Arg_InvokedCallback, A1 Arg_Argument1, A2 Arg_Argument2, A3 Arg_Argument3, A4 Arg_Argument4) {
            if (Arg_InvokedCallback != null) {
                Delegate[] Func_EventHandlers = PrepareInvoking();
                for (int Loop_EventHandlerIndex = Func_EventHandlers.Length - 1; Loop_EventHandlerIndex != -1; --Loop_EventHandlerIndex) {
                    Arg_InvokedCallback(UnsafeUtils.Reinterpret<Delegate, Func<A1, A2, A3, A4, RT>>(Func_EventHandlers.UncheckedRead(Loop_EventHandlerIndex))(Arg_Argument1, Arg_Argument2, Arg_Argument3, Arg_Argument4), Arg_Argument1, Arg_Argument2, Arg_Argument3, Arg_Argument4);
                }
            } else { throw new ArgumentNullException(nameof(Arg_InvokedCallback)); }
        }

    }

    public class EventListener : EventListenerUnsafe {

        public void AddHandler(Action Arg_Handler) => base.AddHandler(Arg_Handler);
        public void AddHandlers(Action[] Arg_Handlers) => base.AddHandlers(Arg_Handlers);
        public void ContainsHandler(Action Arg_Handler) => base.ContainsHandler(Arg_Handler);
        public void RemoveHandler(Action Arg_Handler) => base.RemoveHandler(Arg_Handler);
        public void RemoveHandlers(Action[] Arg_Handlers) => base.RemoveHandlers(Arg_Handlers);
        public void Invoke() => UnsafeActionInvoke();

        public static EventListener operator +(EventListener Arg_Listener, Action Arg_Handler) {
            Arg_Listener.AddHandler(Arg_Handler);
            return Arg_Listener;
        }

        public static EventListener operator -(EventListener Arg_Listener, Action Arg_Handler) {
            Arg_Listener.RemoveHandler(Arg_Handler);
            return Arg_Listener;
        }

        public static EventListener operator +(EventListener Arg_Listener, Action[] Arg_Handlers) {
            Arg_Listener.AddHandlers(Arg_Handlers);
            return Arg_Listener;
        }

        public static EventListener operator -(EventListener Arg_Listener, Action[] Arg_Handlers) {
            Arg_Listener.RemoveHandlers(Arg_Handlers);
            return Arg_Listener;
        }

    }

    public class EventListener<A1> : EventListenerUnsafe {

        public void AddHandler(Action<A1> Arg_Handler) => base.AddHandler(Arg_Handler);
        public void AddHandlers(Action<A1>[] Arg_Handlers) => base.AddHandlers(Arg_Handlers);
        public void ContainsHandler(Action<A1> Arg_Handler) => base.ContainsHandler(Arg_Handler);
        public void RemoveHandler(Action<A1> Arg_Handler) => base.RemoveHandler(Arg_Handler);
        public void RemoveHandlers(Action<A1>[] Arg_Handlers) => base.RemoveHandlers(Arg_Handlers);
        public void Invoke(A1 Arg_Argument1) => UnsafeActionInvoke(Arg_Argument1);

        public static EventListener<A1> operator +(EventListener<A1> Arg_Listener, Action<A1> Arg_Handler) {
            Arg_Listener.AddHandler(Arg_Handler);
            return Arg_Listener;
        }

        public static EventListener<A1> operator -(EventListener<A1> Arg_Listener, Action<A1> Arg_Handler) {
            Arg_Listener.RemoveHandler(Arg_Handler);
            return Arg_Listener;
        }

        public static EventListener<A1> operator +(EventListener<A1> Arg_Listener, Action<A1>[] Arg_Handlers) {
            Arg_Listener.AddHandlers(Arg_Handlers);
            return Arg_Listener;
        }

        public static EventListener<A1> operator -(EventListener<A1> Arg_Listener, Action<A1>[] Arg_Handlers) {
            Arg_Listener.RemoveHandlers(Arg_Handlers);
            return Arg_Listener;
        }

    }

    public class EventListener<A1, A2> : EventListenerUnsafe {

        public void AddHandler(Action<A1, A2> Arg_Handler) => base.AddHandler(Arg_Handler);
        public void AddHandlers(Action<A1, A2>[] Arg_Handlers) => base.AddHandlers(Arg_Handlers);
        public void ContainsHandler(Action<A1, A2> Arg_Handler) => base.ContainsHandler(Arg_Handler);
        public void RemoveHandler(Action<A1, A2> Arg_Handler) => base.RemoveHandler(Arg_Handler);
        public void RemoveHandlers(Action<A1, A2>[] Arg_Handlers) => base.RemoveHandlers(Arg_Handlers);
        public void Invoke(A1 Arg_Argument1, A2 Arg_Argument2) => UnsafeActionInvoke(Arg_Argument1, Arg_Argument2);

        public static EventListener<A1, A2> operator +(EventListener<A1, A2> Arg_Listener, Action<A1, A2> Arg_Handler) {
            Arg_Listener.AddHandler(Arg_Handler);
            return Arg_Listener;
        }

        public static EventListener<A1, A2> operator -(EventListener<A1, A2> Arg_Listener, Action<A1, A2> Arg_Handler) {
            Arg_Listener.RemoveHandler(Arg_Handler);
            return Arg_Listener;
        }

        public static EventListener<A1, A2> operator +(EventListener<A1, A2> Arg_Listener, Action<A1, A2>[] Arg_Handlers) {
            Arg_Listener.AddHandlers(Arg_Handlers);
            return Arg_Listener;
        }

        public static EventListener<A1, A2> operator -(EventListener<A1, A2> Arg_Listener, Action<A1, A2>[] Arg_Handlers) {
            Arg_Listener.RemoveHandlers(Arg_Handlers);
            return Arg_Listener;
        }

    }

    public class EventListener<A1, A2, A3> : EventListenerUnsafe {

        public void AddHandler(Action<A1, A2, A3> Arg_Handler) => base.AddHandler(Arg_Handler);
        public void AddHandlers(Action<A1, A2, A3>[] Arg_Handlers) => base.AddHandlers(Arg_Handlers);
        public void ContainsHandler(Action<A1, A2, A3> Arg_Handler) => base.ContainsHandler(Arg_Handler);
        public void RemoveHandler(Action<A1, A2, A3> Arg_Handler) => base.RemoveHandler(Arg_Handler);
        public void RemoveHandlers(Action<A1, A2, A3>[] Arg_Handlers) => base.RemoveHandlers(Arg_Handlers);
        public void Invoke(A1 Arg_Argument1, A2 Arg_Argument2, A3 Arg_Argument3) => UnsafeActionInvoke(Arg_Argument1, Arg_Argument2, Arg_Argument3);

        public static EventListener<A1, A2, A3> operator +(EventListener<A1, A2, A3> Arg_Listener, Action<A1, A2, A3> Arg_Handler) {
            Arg_Listener.AddHandler(Arg_Handler);
            return Arg_Listener;
        }

        public static EventListener<A1, A2, A3> operator -(EventListener<A1, A2, A3> Arg_Listener, Action<A1, A2, A3> Arg_Handler) {
            Arg_Listener.RemoveHandler(Arg_Handler);
            return Arg_Listener;
        }

        public static EventListener<A1, A2, A3> operator +(EventListener<A1, A2, A3> Arg_Listener, Action<A1, A2, A3>[] Arg_Handlers) {
            Arg_Listener.AddHandlers(Arg_Handlers);
            return Arg_Listener;
        }

        public static EventListener<A1, A2, A3> operator -(EventListener<A1, A2, A3> Arg_Listener, Action<A1, A2, A3>[] Arg_Handlers) {
            Arg_Listener.RemoveHandlers(Arg_Handlers);
            return Arg_Listener;
        }

    }

    public class EventListener<A1, A2, A3, A4> : EventListenerUnsafe {

        public void AddHandler(Action<A1, A2, A3, A4> Arg_Handler) => base.AddHandler(Arg_Handler);
        public void AddHandlers(Action<A1, A2, A3, A4>[] Arg_Handlers) => base.AddHandlers(Arg_Handlers);
        public void ContainsHandler(Action<A1, A2, A3, A4> Arg_Handler) => base.ContainsHandler(Arg_Handler);
        public void RemoveHandler(Action<A1, A2, A3, A4> Arg_Handler) => base.RemoveHandler(Arg_Handler);
        public void RemoveHandlers(Action<A1, A2, A3, A4>[] Arg_Handlers) => base.RemoveHandlers(Arg_Handlers);
        public void Invoke(A1 Arg_Argument1, A2 Arg_Argument2, A3 Arg_Argument3, A4 Arg_Argument4) => UnsafeActionInvoke(Arg_Argument1, Arg_Argument2, Arg_Argument3, Arg_Argument4);

        public static EventListener<A1, A2, A3, A4> operator +(EventListener<A1, A2, A3, A4> Arg_Listener, Action<A1, A2, A3, A4> Arg_Handler) {
            Arg_Listener.AddHandler(Arg_Handler);
            return Arg_Listener;
        }

        public static EventListener<A1, A2, A3, A4> operator -(EventListener<A1, A2, A3, A4> Arg_Listener, Action<A1, A2, A3, A4> Arg_Handler) {
            Arg_Listener.RemoveHandler(Arg_Handler);
            return Arg_Listener;
        }

        public static EventListener<A1, A2, A3, A4> operator +(EventListener<A1, A2, A3, A4> Arg_Listener, Action<A1, A2, A3, A4>[] Arg_Handlers) {
            Arg_Listener.AddHandlers(Arg_Handlers);
            return Arg_Listener;
        }

        public static EventListener<A1, A2, A3, A4> operator -(EventListener<A1, A2, A3, A4> Arg_Listener, Action<A1, A2, A3, A4>[] Arg_Handlers) {
            Arg_Listener.RemoveHandlers(Arg_Handlers);
            return Arg_Listener;
        }

    }

}
