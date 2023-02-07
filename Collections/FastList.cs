using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Reaper1121.SharpToolbox.Collections;

/// <summary>
/// A faster and far more advanced version of <see cref="List{T}"/> API
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
    /// <summary>
    /// This allows for unsafe access to the underlying FastList array.
    /// </summary>
    internal T[] InnerArray => Items;
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
        if (Arg_Capacity < 0) {
            throw new ArgumentException("The capacity cannot be negative!", nameof(Arg_Capacity));
        }
        if (Items.Length < Arg_Capacity) {
            int DoubledCapacity = Items.Length * 2;
            if (DoubledCapacity < Arg_Capacity) {
                DoubledCapacity = Arg_Capacity;
            }
            Capacity = DoubledCapacity;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly T ItemRef(int Arg_ItemIndex) {
        if (Arg_ItemIndex >= _Count) {
            throw new ArgumentOutOfRangeException(nameof(Arg_ItemIndex), Arg_ItemIndex, "The index is out of range!");
        }
        return ref Items[Arg_ItemIndex];
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
        int Func_ItemCount = Arg_Items.Length;
        if ((uint) Arg_StartIndex >= (uint) Func_ItemCount) {
            throw new ArgumentOutOfRangeException(nameof(Arg_StartIndex), Arg_StartIndex, "The index is out of range!");
        }
        if (Func_ItemCount != 0) {
            if (Arg_ItemCount < 0) {
                throw new ArgumentException("The value cannot be negative!", nameof(Arg_ItemCount));
            }
            if (Arg_StartIndex + Arg_ItemCount > Func_ItemCount) {
                throw new ArgumentException("The count is greater than number of elements starting from the index!", nameof(Arg_ItemCount));
            }
            EnsureCapacity(Items.Length + Arg_ItemCount);
            Array.Copy(Arg_Items, Arg_StartIndex, Items, _Count, Arg_ItemCount);
            _Count += Arg_ItemCount;
        }
    }
    public void AddRange(IList<T> Arg_Items, int Arg_StartIndex, int Arg_ItemCount) {
        _ = Arg_Items ?? throw new ArgumentNullException(nameof(Arg_Items));
        int Func_ItemCount = Arg_Items.Count;
        if ((uint) Arg_StartIndex >= (uint) Func_ItemCount) {
            throw new ArgumentOutOfRangeException(nameof(Arg_StartIndex), Arg_StartIndex, "The index is out of range!");
        }
        if (Func_ItemCount != 0) {
            if (Arg_ItemCount < 0) {
                throw new ArgumentException("The value cannot be negative!", nameof(Arg_ItemCount));
            }
            if (Arg_StartIndex + Arg_ItemCount > Func_ItemCount) {
                throw new ArgumentException("The count is greater than number of elements starting from the index!", nameof(Arg_ItemCount));
            }
            EnsureCapacity(Items.Length + Arg_ItemCount);
            switch (Arg_Items) {
                case T[] Func_Items:
                    Array.Copy(Func_Items, Arg_StartIndex, Items, _Count, Arg_ItemCount);
                    break;
                case FastList<T> Func_Items:
                    Array.Copy(Func_Items.Items, Arg_StartIndex, Items, _Count, Arg_ItemCount);
                    break;
                case List<T> Func_Items:
                    Func_Items.CopyTo(Arg_StartIndex, Items, _Count, Arg_ItemCount);
                    break;
                default: {
                    int Func_DestinationIndex = _Count;
                    for (; Arg_ItemCount > 0; Arg_ItemCount--) {
                        Items[Func_DestinationIndex++] = Arg_Items[Arg_StartIndex++];
                    }
                    break;
                }
            }
            _Count += Arg_ItemCount;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void UnsafeSetCapacity(int Arg_Capacity) {
        T[] Func_ListItems = new T[Arg_Capacity];
        Array.Copy(Items, 0, Func_ListItems, 0, _Count);
        Items = Func_ListItems;
    }

    public void Insert(int Arg_DestinationIndex, T Arg_Item) {
        int Func_ListItemCount = _Count;
        if ((uint) Arg_DestinationIndex > (uint) Func_ListItemCount) {
            throw new ArgumentOutOfRangeException(nameof(Arg_DestinationIndex), Arg_DestinationIndex, "The index is out of range!");
        }
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
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void InsertRange(IList<T> Arg_Items, int Arg_DestinationIndex) => InsertRange(Arg_Items ?? throw new ArgumentNullException(nameof(Arg_Items)), Arg_Items.Count, 0, Arg_DestinationIndex);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void InsertRange(IList<T> Arg_Items, int Arg_InsertCount, int Arg_DestinationIndex) => InsertRange(Arg_Items, Arg_InsertCount, 0, Arg_DestinationIndex);

    public void InsertRange(IList<T> Arg_Items, int Arg_InsertCount, int Arg_StartIndex, int Arg_DestinationIndex) {
        _ = Arg_Items ?? throw new ArgumentNullException(nameof(Arg_Items));
        int Func_InsertItemsCount = Arg_Items.Count;
        if ((uint) Arg_StartIndex >= (uint) Func_InsertItemsCount) {
            throw new ArgumentOutOfRangeException(nameof(Arg_StartIndex), Arg_StartIndex, "The index is out of range!");
        }
        if ((uint) Arg_DestinationIndex > (uint) _Count) {
            throw new ArgumentOutOfRangeException(nameof(Arg_DestinationIndex), Arg_DestinationIndex, "The index is out of range!");
        }
        if (Arg_InsertCount < 0) {
            throw new ArgumentException("The value cannot be negative!", nameof(Arg_InsertCount));
        }
        if (Arg_StartIndex + Arg_InsertCount > Func_InsertItemsCount) {
            throw new ArgumentException("The count is greater than number of elements starting from the index!", nameof(Arg_InsertCount));
        }
        if (Arg_InsertCount != 0) {
            EnsureCapacity(_Count + Arg_InsertCount);
            if (Arg_DestinationIndex != _Count) {
                Array.Copy(Items, Arg_DestinationIndex, Items, Arg_DestinationIndex + Arg_InsertCount, _Count - Arg_DestinationIndex);
            }
            if (Arg_Items == this) {
                if (Arg_StartIndex <= Arg_DestinationIndex) {
                    if (Arg_StartIndex + Arg_InsertCount > Arg_DestinationIndex) {
                        int Func_MovedItemCount = Arg_InsertCount - Arg_DestinationIndex;
                        Array.Copy(Items, Arg_DestinationIndex, Items, Arg_DestinationIndex + Func_MovedItemCount, Func_MovedItemCount);
                        Array.Copy(Items, Arg_StartIndex, Items, Arg_DestinationIndex, Arg_InsertCount - Func_MovedItemCount);
                    } else if (Arg_StartIndex != Arg_DestinationIndex) {
                        Array.Copy(Items, Arg_StartIndex, Items, Arg_DestinationIndex, Arg_InsertCount);
                    }
                } else {
                    Array.Copy(Items, Arg_StartIndex + Arg_InsertCount, Items, Arg_DestinationIndex, Arg_InsertCount);
                }
            } else {
                if (Arg_InsertCount == Arg_Items.Count && Arg_StartIndex == 0) {
                    Arg_Items.CopyTo(Items, Arg_DestinationIndex);
                } else {
                    switch (Arg_Items) {
                        case T[] Func_InsertItems:
                            Array.Copy(Func_InsertItems, Arg_StartIndex, Items, Arg_DestinationIndex, Arg_InsertCount);
                            break;
                        case FastList<T> Func_InsertItems:
                            Array.Copy(Func_InsertItems.Items, Arg_StartIndex, Items, Arg_DestinationIndex, Arg_InsertCount);
                            break;
                        case List<T> Func_InsertItems:
                            Func_InsertItems.CopyTo(Arg_StartIndex, Items, Arg_DestinationIndex, Arg_InsertCount);
                            break;
                        default: {
                            for (; Arg_InsertCount > 0; Arg_InsertCount--) {
                                Items[Arg_DestinationIndex++] = Arg_Items[Arg_StartIndex++];
                            }
                            break;
                        }
                    }
                }
            }
            _Count += Arg_InsertCount;
        }
    }

    void System.Collections.ICollection.CopyTo(Array Arg_Array, int Arg_ArrayStartIndex) => Array.Copy(Items, 0, Arg_Array, Arg_ArrayStartIndex, _Count);
    public void CopyTo(T[] Arg_Array, int Arg_ArrayStartIndex) => Array.Copy(Items, 0, Arg_Array, Arg_ArrayStartIndex, _Count);
    public void CopyTo(int Arg_StartIndex, T[] Arg_DestinationArray, int Arg_DestinationStartIndex, int Arg_CopyCount) => Array.Copy(Items, Arg_StartIndex, Arg_DestinationArray, Arg_DestinationStartIndex, Arg_CopyCount);

    public int IndexOf(T Arg_Item) {
        int Func_ItemIndex = -1;
        int Func_Count = _Count;
        T[] Func_ListItems = Items;
        EqualityComparer<T> Func_ValueComparer = EqualityComparer<T>.Default;
        for (int Loop_ItemIndex = 0; Loop_ItemIndex < Func_Count; ++Loop_ItemIndex) {
            if (Func_ValueComparer.Equals(Func_ListItems[Loop_ItemIndex], Arg_Item) == true) {
                Func_ItemIndex = Loop_ItemIndex;
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
        for (int Loop_ItemIndex = 0; Loop_ItemIndex < Func_Count; ++Loop_ItemIndex) {
            if (Arg_Comparison(Func_ListItems[Loop_ItemIndex], Arg_Item) == 0) {
                Func_ItemIndex = Loop_ItemIndex;
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
        for (int Loop_ItemIndex = 0; Loop_ItemIndex < Func_Count; ++Loop_ItemIndex) {
            if (Arg_ItemPredicate(Func_ListItems[Loop_ItemIndex]) == true) {
                Func_ItemIndex = Loop_ItemIndex;
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
        if ((uint) Arg_Index >= (uint) Func_ListItemCount) {
            throw new ArgumentOutOfRangeException(nameof(Arg_Index), Arg_Index, "The index is out of range!");
        }
        _Count = --Func_ListItemCount;
        T[] Func_ListItems = Items;
        Array.Copy(Func_ListItems, Arg_Index + 1, Func_ListItems, Arg_Index, Func_ListItemCount - Arg_Index);
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() == true) {
            Func_ListItems[Func_ListItemCount] = default!;
        }
    }

    public void RemoveUnorderedAt(int Arg_Index) {
        int Func_ListItemCount = _Count;
        if ((uint) Arg_Index >= (uint) Func_ListItemCount) {
            throw new ArgumentOutOfRangeException(nameof(Arg_Index), Arg_Index, "The index is out of range!");
        }
        _Count = --Func_ListItemCount;
        T[] Func_ListItems = Items;
        Func_ListItems[Arg_Index] = Func_ListItems[Func_ListItemCount];
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() == true) {
            Func_ListItems[Func_ListItemCount] = default!;
        }
    }

    public void RemoveAt(int Arg_StartIndex, int Arg_RemoveCount) {
        if (Arg_RemoveCount < 0) {
            throw new ArgumentException("The value cannot be negative!", nameof(Arg_RemoveCount));
        }
        int Func_ListItemCount = _Count;
        if ((uint) Arg_StartIndex >= (uint) Func_ListItemCount) {
            throw new ArgumentOutOfRangeException(nameof(Arg_StartIndex), Arg_StartIndex, "The index is out of range!");
        }
        if (Arg_StartIndex + Arg_RemoveCount > Func_ListItemCount) {
            throw new ArgumentException("The count is greater than number of elements starting from the index!", nameof(Arg_RemoveCount));
        }
        if (Arg_RemoveCount != 0) {
            Func_ListItemCount -= Arg_RemoveCount;
            _Count = Func_ListItemCount;
            T[] Func_ListItems = Items;
            Array.Copy(Func_ListItems, Arg_StartIndex + Arg_RemoveCount, Func_ListItems, Arg_StartIndex, Func_ListItemCount - Arg_StartIndex);
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() == true) {
                Array.Clear(Func_ListItems, Func_ListItemCount, Arg_RemoveCount);
            }
        }
    }

    void IList<T>.RemoveAt(int Arg_Index) => RemoveAt(Arg_Index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() {
        int Func_Count = _Count;
        _Count = 0;
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>() == true && Func_Count != 0) {
            Array.Clear(Items, 0, Func_Count);
        }
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
