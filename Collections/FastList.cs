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

namespace Reaper1121.SharpToolbox.Collections {

    /// <summary>
    /// A faster and more advanced version of <see cref="List{T}"/> API
    /// </summary>
    /// <typeparam name="T">List element</typeparam>
    [SkipLocalsInit]
    public sealed class FastList<T> : IList<T>, System.Collections.ICollection { // TODO: Implement all functions from the standard List<T> API

        public int Capacity {
            get => Items.Length;
            set {
                int Func_ListItemCount = _Count;
                if (value < Func_ListItemCount) { value = Func_ListItemCount; }
                if (value == 0) { value = 1; }
                if (value != Items.Length) {
                    T[] Func_ListItems = new T[value];
                    if (Func_ListItemCount != 0) { Array.Copy(Items, 0, Func_ListItems, 0, Func_ListItemCount); }
                    Items = Func_ListItems;
                }
            }
        }

        private int _Count;
        public int Count => _Count;
        int ICollection<T>.Count => _Count;
        int System.Collections.ICollection.Count => _Count;

        private T[] Items;
        public T[] InnerArray => Items;
        public bool IsReadOnly => false;
        public bool IsSynchronized => false;
        public object SyncRoot => this;

        public T this[int Arg_ItemIndex] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Arg_ItemIndex < _Count ? Items[Arg_ItemIndex] : throw new ArgumentOutOfRangeException(nameof(Arg_ItemIndex), Arg_ItemIndex, "The index is out of range!");
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Items[Arg_ItemIndex] = Arg_ItemIndex < _Count ? value : throw new ArgumentOutOfRangeException(nameof(Arg_ItemIndex), Arg_ItemIndex, "The index is out of range!");
        }

        public FastList() : this(8) { }

        public FastList(int Arg_Capacity) {
            if (Arg_Capacity < 1) { Arg_Capacity = 1; }
            Items = new T[Arg_Capacity];
        }

        public void EnsureCapacity(int Arg_Capacity) {
            if (Arg_Capacity > -1) {
                if (Items.Length < Arg_Capacity) {
                    int DoubledCapacity = Items.Length * 2;
                    if (DoubledCapacity < Arg_Capacity) {
                        DoubledCapacity = Arg_Capacity;
                    }
                    Capacity = DoubledCapacity;
                }
            } else { throw new ArgumentException("The capacity cannot be negative!", nameof(Arg_Capacity)); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T Arg_Item) {
            int Func_Count = _Count;
            T[] Func_Items = Items;
            if (Func_Count != Func_Items.Length) {
                _Count = Func_Count + 1;
                Func_Items[Func_Count] = Arg_Item;
                return;
            }
            AddWithResize(Arg_Item);

            [MethodImpl(MethodImplOptions.NoInlining)]
            void AddWithResize(T Arg_Item) {
                T[] Func_ListItems = new T[Items.Length * 2];
                Array.Copy(Items, 0, Func_ListItems, 0, _Count);
                Items = Func_ListItems;
                Func_ListItems[_Count++] = Arg_Item;
            }

        }

        public void AddRange(T[] Arg_Items) {
            _ = Arg_Items ?? throw new ArgumentNullException(nameof(Arg_Items));
            int Func_ItemsLength = Arg_Items.Length;
            if (Func_ItemsLength != 0) {
                EnsureCapacity(_Count + Func_ItemsLength);
                Array.Copy(Arg_Items, 0, Items, _Count, Func_ItemsLength);
                _Count += Func_ItemsLength;
            }
        }
        public void AddRange(IList<T> Arg_Items) {
            _ = Arg_Items ?? throw new ArgumentNullException(nameof(Arg_Items));
            int Func_ItemsCount = Arg_Items.Count;
            if (Func_ItemsCount != 0) {
                EnsureCapacity(_Count + Func_ItemsCount);
                Arg_Items.CopyTo(Items, _Count);
                _Count += Func_ItemsCount;
            }
        }

        public void AddRange(T[] Arg_Items, int Arg_StartIndex, int Arg_ItemCount) {
            _ = Arg_Items ?? throw new ArgumentNullException(nameof(Arg_Items));
            int Func_ItemsLength = Arg_Items.Length;
            if (Arg_ItemCount > -1 && Arg_ItemCount <= Func_ItemsLength) {
                if ((uint) Arg_StartIndex < (uint) Func_ItemsLength) {
                    if (Arg_StartIndex + Arg_ItemCount < Func_ItemsLength) {
                        EnsureCapacity(Items.Length + Arg_ItemCount);
                        Array.Copy(Arg_Items, 0, Items, _Count, Arg_ItemCount);
                        _Count += Arg_ItemCount;
                    } else { throw new ArgumentException("The start index + item count cannot be greater than item array length!", nameof(Arg_Items)); }
                } else { throw new ArgumentOutOfRangeException(nameof(Arg_StartIndex), Arg_StartIndex, "The start index cannot be negative or greater than item array length!"); }
            } else { throw new ArgumentOutOfRangeException(nameof(Arg_ItemCount), Arg_ItemCount, "The item count cannot be greater than item array length or negative!"); }
        }
        public void AddRange(FastList<T> Arg_Items, int Arg_StartIndex, int Arg_ItemCount) {
            _ = Arg_Items ?? throw new ArgumentNullException(nameof(Arg_Items));
            int Func_ItemsCount = Arg_Items._Count;
            if (Arg_ItemCount > -1 && Arg_ItemCount <= Func_ItemsCount) {
                if ((uint) Arg_StartIndex < (uint) Func_ItemsCount) {
                    if (Arg_StartIndex + Arg_ItemCount < Func_ItemsCount) {
                        EnsureCapacity(Items.Length + Arg_ItemCount);
                        Array.Copy(Arg_Items.Items, 0, Items, _Count, Arg_ItemCount);
                        _Count += Arg_ItemCount;
                    } else { throw new ArgumentException("The start index + item count cannot be greater than item list count!", nameof(Arg_Items)); }
                } else { throw new ArgumentOutOfRangeException(nameof(Arg_StartIndex), Arg_StartIndex, "The start index cannot be negative or greater than item list count!"); }
            } else { throw new ArgumentOutOfRangeException(nameof(Arg_ItemCount), Arg_ItemCount, "The item count cannot be greater than item list count or negative!"); }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void UnsafeSetCapacity(int Arg_Capacity) {
            T[] Func_ListItems = new T[Arg_Capacity];
            Array.Copy(Items, 0, Func_ListItems, 0, _Count);
            Items = Func_ListItems;
        }

        public void Insert(int Arg_DestinationIndex, T Arg_Item) {
            if (Arg_DestinationIndex > -1) {
                int Func_ListItemCount = _Count;
                if (Arg_DestinationIndex <= Func_ListItemCount) {
                    int Func_ListCapacity = Items.Length;
                    if (Func_ListItemCount == Func_ListCapacity) {
                        Func_ListCapacity *= 2;
                        UnsafeSetCapacity(Func_ListCapacity);
                    }
                    if (Arg_DestinationIndex != Func_ListItemCount) {
                        Array.Copy(Items, Arg_DestinationIndex, Items, Arg_DestinationIndex + 1, Func_ListItemCount - Arg_DestinationIndex);
                    }
                    Items[Arg_DestinationIndex] = Arg_Item;
                    _Count = Func_ListItemCount + 1;
                } else { throw new ArgumentOutOfRangeException(nameof(Arg_DestinationIndex), Arg_DestinationIndex, "The destination index cannot be greater than list count!"); }
            } else { throw new ArgumentException("The destination index cannot be negative!", nameof(Arg_DestinationIndex)); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InsertRange(T[] Arg_Items, int Arg_DestinationIndex) {
            _ = Arg_Items ?? throw new ArgumentNullException(nameof(Arg_Items));
            InsertRange(Arg_Items, Arg_Items.Length, 0, Arg_DestinationIndex);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InsertRange(FastList<T> Arg_Items, int Arg_DestinationIndex) {
            _ = Arg_Items ?? throw new ArgumentNullException(nameof(Arg_Items));
            InsertRange(Arg_Items, Arg_Items._Count, 0, Arg_DestinationIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InsertRange(T[] Arg_Items, int Arg_InsertCount, int Arg_DestinationIndex) => InsertRange(Arg_Items, Arg_InsertCount, 0, Arg_DestinationIndex);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InsertRange(FastList<T> Arg_Items, int Arg_InsertCount, int Arg_DestinationIndex) => InsertRange(Arg_Items, Arg_InsertCount, 0, Arg_DestinationIndex);

        public void InsertRange(T[] Arg_InsertItems, int Arg_InsertCount, int Arg_StartIndex, int Arg_DestinationIndex) {
            _ = Arg_InsertItems ?? throw new ArgumentNullException(nameof(Arg_InsertItems));
            int Func_InsertItemsLength = Arg_InsertItems.Length;
            if (Arg_InsertCount > -1 && Arg_InsertCount <= Func_InsertItemsLength) {
                if ((uint) Arg_StartIndex < (uint) Func_InsertItemsLength) {
                    if (Arg_StartIndex + Arg_InsertCount <= Func_InsertItemsLength) {
                        if ((uint) Arg_DestinationIndex <= (uint) _Count) {
                            if (Arg_InsertCount != 0) {
                                EnsureCapacity(_Count + Arg_InsertCount);
                                if (Arg_DestinationIndex != _Count) {
                                    Array.Copy(Items, Arg_DestinationIndex, Items, Arg_DestinationIndex + Arg_InsertCount, _Count - Arg_DestinationIndex);
                                }
                                Array.Copy(Arg_InsertItems, Arg_StartIndex, Items, Arg_DestinationIndex, Arg_InsertCount);
                                _Count += Arg_InsertCount;
                            }
                        } else { throw new ArgumentOutOfRangeException(nameof(Arg_DestinationIndex), Arg_DestinationIndex, "The destination index cannot be greater than list item count or negative!"); }
                    } else { throw new ArgumentException("The start index + insert count cannot be greater than insert items array length!", nameof(Arg_InsertItems)); }
                } else { throw new ArgumentOutOfRangeException(nameof(Arg_StartIndex), Arg_StartIndex, "The start index cannot be negative or greater than or equal to insert item array length!"); }
            } else { throw new ArgumentOutOfRangeException(nameof(Arg_InsertCount), Arg_InsertCount, "The insert item count cannot be greater than insert item array length or negative!"); }
        }

        public void InsertRange(FastList<T> Arg_InsertItems, int Arg_InsertCount, int Arg_StartIndex, int Arg_DestinationIndex) { // TODO: Fix issues of passing "this" as insert items
            _ = Arg_InsertItems ?? throw new ArgumentNullException(nameof(Arg_InsertItems));
            int Func_InsertItemsCount = Arg_InsertItems._Count;
            if (Arg_InsertCount > -1 && Arg_InsertCount <= Func_InsertItemsCount) {
                if ((uint) Arg_StartIndex < (uint) Func_InsertItemsCount) {
                    if (Arg_StartIndex + Arg_InsertCount <= Func_InsertItemsCount) {
                        if ((uint) Arg_DestinationIndex <= (uint) _Count) {
                            if (Arg_InsertCount != 0) {
                                EnsureCapacity(_Count + Arg_InsertCount);
                                if (Arg_DestinationIndex != _Count) {
                                    Array.Copy(Items, Arg_DestinationIndex, Items, Arg_DestinationIndex + Arg_InsertCount, _Count - Arg_DestinationIndex);
                                }
                                Array.Copy(Arg_InsertItems.Items, Arg_StartIndex, Items, Arg_DestinationIndex, Arg_InsertCount);
                                _Count += Arg_InsertCount;
                            }
                        } else { throw new ArgumentOutOfRangeException(nameof(Arg_DestinationIndex), Arg_DestinationIndex, "The destination index cannot be greater than list item count or negative!"); }
                    } else { throw new ArgumentException("The start index + insert count cannot be greater than insert items count!", nameof(Arg_InsertItems)); }
                } else { throw new ArgumentOutOfRangeException(nameof(Arg_StartIndex), Arg_StartIndex, "The start index cannot be negative, greater than or equal to insert item list count!"); }
            } else { throw new ArgumentOutOfRangeException(nameof(Arg_InsertCount), Arg_InsertCount, "The insert item count cannot be greater than insert item list count or negative!"); }
        }

        public void Copy(FastList<T> Arg_List) => throw new NotImplementedException();

        void System.Collections.ICollection.CopyTo(Array Arg_Array, int Arg_ArrayStartIndex) => Array.Copy(Items, 0, Arg_Array, Arg_ArrayStartIndex, _Count);
        public void CopyTo(T[] Arg_Array, int Arg_ArrayStartIndex) => Array.Copy(Items, 0, Arg_Array, Arg_ArrayStartIndex, _Count);
        public void CopyTo(int Arg_StartIndex, T[] Arg_DestinationArray, int Arg_DestinationStartIndex, int Arg_CopyCount) {
            _ = Arg_DestinationArray ?? throw new ArgumentNullException(nameof(Arg_DestinationArray));
            if (Arg_CopyCount > -1) {
                if ((uint) Arg_StartIndex < (uint) _Count) {
                    if (Arg_StartIndex + Arg_CopyCount <= _Count) {
                        if ((uint) Arg_DestinationStartIndex < (uint) Arg_DestinationArray.Length) {
                            if (Arg_DestinationStartIndex + Arg_CopyCount <= Arg_DestinationArray.Length) {
                                if (Arg_CopyCount != 0) {
                                    Array.Copy(Items, Arg_StartIndex, Arg_DestinationArray, Arg_DestinationStartIndex, Arg_CopyCount);
                                }
                            } else { throw new ArgumentException("The destination start index + copy count cannot be greater than destination array length!"); }
                        } else { throw new ArgumentException("The index cannot be negative or greater than list count!", nameof(Arg_DestinationStartIndex)); }
                    } else { throw new ArgumentException("The start index + copy count cannot be greater than list count!"); }
                } else { throw new ArgumentException("The index cannot be negative or greater than list count!", nameof(Arg_StartIndex)); }
            } else { throw new ArgumentException("Argument cannot be negative!", nameof(Arg_CopyCount)); }
        }

        public int IndexOf(T Arg_Item) {
            int Func_ItemIndex = -1;
            int Func_Count = _Count;
            T[] Func_ListItems = Items;
            EqualityComparer<T> Func_ValueComparer = EqualityComparer<T>.Default;
            for (int Loop_Index = 0; Loop_Index < Func_Count; ++Loop_Index) {
                if (Func_ValueComparer.Equals(Func_ListItems[Loop_Index], Arg_Item) == true) {
                    Func_ItemIndex = Loop_Index;
                    break;
                }
            }
            return Func_ItemIndex;
        }

        public int IndexOf(T Arg_Item, Comparison<T> Arg_Comparison) {
            _ = Arg_Comparison ?? throw new ArgumentNullException(nameof(Arg_Comparison));
            int Func_ItemIndex = -1;
            int Func_Count = _Count;
            T[] Func_ListItems = Items;
            for (int Loop_Index = 0; Loop_Index < Func_Count; ++Loop_Index) {
                if (Arg_Comparison(Func_ListItems[Loop_Index], Arg_Item) == 0) {
                    Func_ItemIndex = Loop_Index;
                    break;
                }
            }
            return Func_ItemIndex;
        }

        public int IndexOf(Predicate<T> Arg_ItemPredicate) {
            _ = Arg_ItemPredicate ?? throw new ArgumentNullException(nameof(Arg_ItemPredicate));
            int Func_ItemIndex = -1;
            int Func_Count = _Count;
            T[] Func_ListItems = Items;
            for (int Loop_Index = 0; Loop_Index < Func_Count; ++Loop_Index) {
                if (Arg_ItemPredicate(Func_ListItems[Loop_Index]) == true) {
                    Func_ItemIndex = Loop_Index;
                    break;
                }
            }
            return Func_ItemIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T Arg_Item) => IndexOf(Arg_Item) != -1;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T Arg_Item, Comparison<T> Arg_Comparison) => IndexOf(Arg_Item, Arg_Comparison) != -1;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Predicate<T> Arg_ItemPredicate) => IndexOf(Arg_ItemPredicate) != -1;

        public bool Remove(T Arg_Item) {
            int Func_ItemIndex = IndexOf(Arg_Item);
            if (Func_ItemIndex != -1) {
                RemoveAt(Func_ItemIndex);
                return true;
            }
            return false;
        }

        public bool RemoveUnordered(T Arg_Item) {
            int Func_ItemIndex = IndexOf(Arg_Item);
            if (Func_ItemIndex != -1) {
                RemoveUnorderedAt(Func_ItemIndex);
                return true;
            }
            return false;
        }

        public void RemoveAt(int Arg_Index) {
            int Func_ListItemCount = _Count;
            if ((uint) Arg_Index < (uint) Func_ListItemCount) {
                _Count = --Func_ListItemCount;
                T[] Func_ListItems = Items;
                if (Arg_Index < Func_ListItemCount) {
                    Array.Copy(Func_ListItems, Arg_Index + 1, Func_ListItems, Arg_Index, Func_ListItemCount - Arg_Index);
                }
                if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() == true) {
                    Func_ListItems[Func_ListItemCount] = default!;
                }
            } else { throw new ArgumentOutOfRangeException(nameof(Arg_Index), Arg_Index, "The index cannot be greater than list item count or negative!"); }
        }

        public void RemoveUnorderedAt(int Arg_Index) {
            int Func_ListItemCount = _Count;
            if ((uint) Arg_Index < (uint) Func_ListItemCount) {
                _Count = --Func_ListItemCount;
                T[] Func_ListItems = Items;
                if (Arg_Index < Func_ListItemCount) {
                    Func_ListItems[Arg_Index] = Func_ListItems[Func_ListItemCount];
                    if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() == true) {
                        Func_ListItems[Func_ListItemCount] = default!;
                    }
                }
            } else { throw new ArgumentOutOfRangeException(nameof(Arg_Index), Arg_Index, "The index cannot be greater than list item count or negative!"); }
        }

        public void RemoveAt(int Arg_StartIndex, int Arg_RemoveCount) {
            if (Arg_RemoveCount > -1) {
                int Func_ListItemCount = _Count;
                if ((uint) Arg_StartIndex < (uint) Func_ListItemCount) {
                    if (Arg_StartIndex + Arg_RemoveCount <= Func_ListItemCount) {
                        if (Arg_RemoveCount != 0) {
                            Func_ListItemCount -= Arg_RemoveCount;
                            _Count = Func_ListItemCount;
                            T[] Func_ListItems = Items;
                            if (Arg_StartIndex < Func_ListItemCount) {
                                Array.Copy(Func_ListItems, Arg_StartIndex + Arg_RemoveCount, Func_ListItems, Arg_StartIndex, Func_ListItemCount - Arg_StartIndex);
                            }
                            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() == true) {
                                Array.Clear(Func_ListItems, Func_ListItemCount, Arg_RemoveCount);
                            }
                        }
                    } else { throw new ArgumentException("The start index + remove count cannot be greater than list item count!"); }
                } else { throw new ArgumentOutOfRangeException(nameof(Arg_StartIndex), Arg_StartIndex, "The start index cannot be greater than list item count or negative!"); }
            } else { throw new ArgumentException("The remove count cannot be negative!", nameof(Arg_RemoveCount)); }
        }

        void IList<T>.RemoveAt(int Arg_Index) => RemoveAt(Arg_Index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() == true) {
                int Func_Count = _Count;
                _Count = 0;
                Array.Clear(Items, 0, Func_Count);
            } else { _Count = 0; }
        }

        public T[] ToArray() {
            T[] Func_ConvertedArray;
            int Func_ItemCount = _Count;
            if (Func_ItemCount != 0) {
                Func_ConvertedArray = new T[Func_ItemCount];
                Array.Copy(Items, 0, Func_ConvertedArray, 0, Func_ItemCount);
            } else {
                Func_ConvertedArray = Array.Empty<T>();
            }
            return Func_ConvertedArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator() => new ArrayEnumerator<T>(ToArray());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => new ArrayEnumerator<T>(ToArray());

        public static explicit operator T[](FastList<T> Arg_List) => Arg_List.ToArray();

    }

}
