using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Reaper1121.SharpToolbox.Utilities;

namespace Reaper1121.SharpToolbox.Collections;

public interface INodeContainer : IEnumerable<INode> {

    int ChildCount { get; }
    int SiblingCount { get; }

    INode GetSibling(int Arg_Index);
    INode GetChild(int Arg_Index);

    void RemoveRoot(INode Arg_RootNode);

}

[SkipLocalsInit]
public abstract class NodeContainer<NT, CT> : INodeContainer where NT : Node<NT, CT> where CT : NodeContainer<NT, CT> {

    private const int INITIAL_CAPACITY = 8;

    public event Action<CT, NT>? RootRemoved;
    public event Action<CT>? RootAdded;

    protected readonly object SyncLock = new object();

    private int _ChildCount;
    public int ChildCount => _ChildCount;
    public NT[] Children {
        get {
            Monitor.Enter(SyncLock);
            try {
                NT[] Func_Children = Array.Empty<NT>();
                int Func_ChildCount = _ChildCount;
                if (Func_ChildCount != 0) {
                    FastList<NT> Func_ChildrenList = new FastList<NT>(Func_ChildCount);
                    Func_Children = Func_ChildrenList.InnerArray;
                    FastList<NT> Func_Siblings = Nodes;
                    for (int Loop_SiblingIndex = 0; Loop_SiblingIndex < Func_Siblings.Count; Loop_SiblingIndex++) {
                        NT Loop_Sibling = Func_Siblings[Loop_SiblingIndex];
                        Func_ChildrenList.Add(Loop_Sibling);
                        Loop_Sibling.CopyChildren(Func_ChildrenList, Func_ChildrenList.Count);
                    }
                }
                return Func_Children;
            } finally { Monitor.Exit(SyncLock); }
        }
    }
    public int SiblingCount => Nodes.Count;
    public NT[] Siblings {
        get {
            Monitor.Enter(SyncLock);
            try {
                return Nodes.ToArray();
            } finally { Monitor.Exit(SyncLock); }
        }
    }

    private readonly FastList<NT> Nodes = new FastList<NT>(INITIAL_CAPACITY);

    INode INodeContainer.GetSibling(int Arg_Index) => GetSibling(Arg_Index);
    public NT GetSibling(int Arg_Index) {
        Monitor.Enter(SyncLock);
        try {
            return Nodes[Arg_Index];
        } finally { Monitor.Exit(SyncLock); }
    }

    INode INodeContainer.GetChild(int Arg_Index) => GetChild(Arg_Index);
    public NT GetChild(int Arg_Index) {
        Monitor.Enter(SyncLock);
        try {
            NT? Func_FoundChild = null;
            int Func_TopChildIndex = -1;
            FastList<NT> Func_Siblings = Nodes;
            for (int Loop_SiblingIndex = Func_Siblings.Count - 1; Loop_SiblingIndex > -1; --Loop_SiblingIndex) {
                NT Loop_Sibling = Func_Siblings[Loop_SiblingIndex];
                Func_TopChildIndex += Loop_Sibling.ChildCount + 1;
                if (Func_TopChildIndex >= Arg_Index) {
                    Arg_Index = Func_TopChildIndex - Arg_Index;
                    Func_FoundChild = Arg_Index == 0 ? Loop_Sibling : Loop_Sibling.GetChild(Arg_Index);
                    break;
                }
            }
            return Func_FoundChild ?? throw new ArgumentOutOfRangeException(nameof(Arg_Index), Arg_Index, "The index is out of range!");
        } finally { Monitor.Exit(SyncLock); }
    }

    protected virtual void OnRootAdded(NT Arg_Node) { }
    protected virtual void OnRootRemoved(NT Arg_Node) { }

    internal void AddRoot(NT Arg_Node) {
        ArgumentNullException.ThrowIfNull(Arg_Node);
        if (Arg_Node.Parent != null) {
            throw new InvalidOperationException("The node must be root!");
        }
        Monitor.Enter(SyncLock);
        try {
            _ChildCount += Arg_Node.ChildCount + 1;
            Nodes.Add(Arg_Node);
            OnRootAdded(Arg_Node);
            if (HasChild(Arg_Node) == true) {
                RootAdded?.Invoke(UnsafeUtils.Reinterpret<NodeContainer<NT, CT>, CT>(this));
            }
        } finally { Monitor.Exit(SyncLock); }
    }

    void INodeContainer.RemoveRoot(INode Arg_RootNode) => RemoveRoot((NT) Arg_RootNode);
    public void RemoveRoot(NT Arg_Node) {
        ArgumentNullException.ThrowIfNull(Arg_Node);
        if (Arg_Node.Parent != null) {
            throw new InvalidOperationException("The node must be root!");
        }
        Monitor.Enter(SyncLock);
        try {
            _ChildCount -= Arg_Node.ChildCount + 1;
            Nodes.Remove(Arg_Node);
            Arg_Node.MarkRemoved();
            OnRootRemoved(Arg_Node);
            RootRemoved?.Invoke(UnsafeUtils.Reinterpret<NodeContainer<NT, CT>, CT>(this), Arg_Node);
        } finally { Monitor.Exit(SyncLock); }
    }

    public bool HasChild(NT Arg_Node) => Arg_Node?.Container == UnsafeUtils.Reinterpret<NodeContainer<NT, CT>, CT>(this);

    public unsafe bool ForEachSibling(Func<NT, object?, bool> Arg_Callback, object? Arg_UserData, bool Arg_IsReversed = false) {
        ArgumentNullException.ThrowIfNull(Arg_Callback);
        Monitor.Enter(SyncLock);
        try {
            bool Func_ContinueRequested = true;
            FastList<NT> Func_SiblingNodes = Nodes;
            if (Arg_IsReversed == false) {
                for (int Loop_Index = 0; Loop_Index < Func_SiblingNodes.Count && (Func_ContinueRequested = Arg_Callback(Func_SiblingNodes[Loop_Index], Arg_UserData)) == true; ++Loop_Index) { }
            } else {
                for (int Loop_Index = Func_SiblingNodes.Count - 1; Loop_Index > -1 && (Func_ContinueRequested = Arg_Callback(Func_SiblingNodes[Loop_Index], Arg_UserData)) == true; --Loop_Index) { }
            }
            return Func_ContinueRequested;
        } finally { Monitor.Exit(SyncLock); }
    }

    public unsafe bool ForEachChild(Func<NT, object?, bool> Arg_Callback, object? Arg_UserData, bool Arg_IsReversed = false) {
        ArgumentNullException.ThrowIfNull(Arg_Callback);
        Monitor.Enter(SyncLock);
        try {
            bool Func_ContinueRequested = true;
            FastList<NT> Func_SiblingNodes = Nodes;
            if (Arg_IsReversed == false) {
                for (int Loop_Index = 0; Loop_Index < Func_SiblingNodes.Count; ++Loop_Index) {
                    NT Loop_SiblingNode = Func_SiblingNodes[Loop_Index];
                    if (Arg_Callback(Loop_SiblingNode, Arg_UserData) == false || (Func_ContinueRequested = Loop_SiblingNode.ForEachChild(Arg_Callback, Arg_UserData, Arg_IsReversed)) == false) { break; }
                }
            } else {
                for (int Loop_Index = Func_SiblingNodes.Count - 1; Loop_Index > -1; --Loop_Index) {
                    NT Loop_SiblingNode = Func_SiblingNodes[Loop_Index];
                    if (Arg_Callback(Loop_SiblingNode, Arg_UserData) == false || (Func_ContinueRequested = Loop_SiblingNode.ForEachChild(Arg_Callback, Arg_UserData, Arg_IsReversed)) == false) { break; }
                }
            }
            return Func_ContinueRequested;
        } finally { Monitor.Exit(SyncLock); }
    }

    public void Clear() {
        Monitor.Enter(SyncLock);
        try {
            FastList<NT> Func_Siblings = Nodes;
            for (int Loop_SiblingIndex = Func_Siblings.Count - 1; Loop_SiblingIndex > -1; --Loop_SiblingIndex) {
                RemoveRoot(Func_Siblings[Loop_SiblingIndex]);
            }
        } finally { Monitor.Exit(SyncLock); }
    }

    public IEnumerator<NT> GetEnumerator() => Nodes.GetEnumerator();
    IEnumerator<INode> IEnumerable<INode>.GetEnumerator() => Nodes.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => Nodes.GetEnumerator();

}