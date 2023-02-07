using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Reaper1121.SharpToolbox.Utilities;

namespace Reaper1121.SharpToolbox.Collections;

public interface INode : IEnumerable<INode> {

    int ChildCount { get; }
    int SiblingCount { get; }

    INode? Parent { get; set; }
    INodeContainer? Container { get; }
    INode Root { get; }

    INode GetSibling(int Arg_Index);
    INode GetChild(int Arg_Index);

}

[SkipLocalsInit]
public abstract class Node<NT, CT> : INode where NT : Node<NT, CT> where CT : NodeContainer<NT, CT> {

    private const int INITIAL_CAPACITY = 8;

    public event Action<NT>? ParentChanged;
    public event Action<NT>? ChildrenChanged;

    private readonly FastList<NT> Nodes = new FastList<NT>(INITIAL_CAPACITY);
    private int Index;
    private int _ChildCount;
    public int ChildCount => _ChildCount;
    public NT[] Children {
        get {
            NT[] Func_Children = Array.Empty<NT>();
            int Func_ChildCount = _ChildCount;
            if (Func_ChildCount != 0) {
                Func_Children = new NT[Func_ChildCount];
                _Root.Nodes.CopyTo(Index + 1, Func_Children, 0, Func_ChildCount);
            }
            return Func_Children;
        }
    }
    private int _SiblingCount;
    public int SiblingCount => _SiblingCount;
    public NT[] Siblings {
        get {
            NT[] Func_Siblings = Array.Empty<NT>();
            int Func_SiblingCount = _SiblingCount;
            if (Func_SiblingCount != 0) {
                Func_Siblings = new NT[Func_SiblingCount];
                NT[] Func_ContainerNodes = _Root.Nodes.InnerArray;
                int Func_SiblingIndex = Index + 1;
                for (int Loop_Index = 0; Loop_Index < Func_SiblingCount; Loop_Index++) {
                    NT Loop_SiblingNode = Func_ContainerNodes[Func_SiblingIndex];
                    Func_SiblingIndex += Loop_SiblingNode._ChildCount + 1;
                    Func_Siblings[Loop_Index] = Loop_SiblingNode;
                }
            }
            return Func_Siblings;
        }
    }

    private NT? _Parent;
    INode? INode.Parent {
        get => Parent;
        set => Parent = (NT?) value;
    }
    public NT? Parent {
        get => _Parent;
        set => SetParent(value);
    }
    public NT[] Parents {
        get {
            FastList<NT> Func_Parents = new FastList<NT>(16);
            NT? Func_ParentNode = UnsafeUtils.Reinterpret<Node<NT, CT>, NT>(this);
            while ((Func_ParentNode = Func_ParentNode._Parent) != null) { Func_Parents.Add(Func_ParentNode); }
            return Func_Parents.ToArray();
        }
    }
    private CT? _Container;
    INodeContainer? INode.Container => Container;
    public CT? Container => _Root._Container;
    private NT _Root;
    INode INode.Root => Root;
    public NT Root => _Root;
    public int Depth {
        get {
            int Func_Depth = 0;
            for (NT? Loop_CurrentNode = _Parent; Loop_CurrentNode != null; Loop_CurrentNode = Loop_CurrentNode._Parent) {
                ++Func_Depth;
            }
            return Func_Depth;
        }
    }

    public Node(CT Arg_NodeContainer) {
        _Container = Arg_NodeContainer ?? throw new ArgumentNullException(nameof(Arg_NodeContainer));
        NT Func_This = UnsafeUtils.Reinterpret<Node<NT, CT>, NT>(this);
        Nodes.Add(Func_This);
        _Root = Func_This;
        Arg_NodeContainer.AddRoot(Func_This);
    }

    private void SetParent(NT? Arg_Parent) {
        if (Arg_Parent == this) {
            throw new ArgumentException("The parent cannot reference it self!", nameof(Arg_Parent));
        }
        NT? Func_PreviousParent = _Parent;
        if (ReferenceEquals(Arg_Parent, Func_PreviousParent) == false) {
            if (Arg_Parent != null) {
                if (Arg_Parent.Container != Container) {
                    throw new ArgumentException("The parent cannot belong to a different container!", nameof(Arg_Parent));
                }
                if (HasChild(Arg_Parent) == true) {
                    throw new ArgumentException("The parent cannot be a child of the node!", nameof(Arg_Parent));
                }
                NT[] Func_OldParentNodes = Parents;
                NT[] Func_NewParentNodes = Arg_Parent.Parents;
                try {
                    int Func_TreeNodeCount = _ChildCount + 1; // 1 = self
                    if (_Root == Arg_Parent._Root) {
                        MoveWithChildren(Arg_Parent.Index);
                    } else {
                        Arg_Parent._Root.Nodes.InsertRange(_Root.Nodes, Func_TreeNodeCount, Index, Arg_Parent.Index + Arg_Parent._ChildCount + 1);
                        _Root.Nodes.RemoveAt(Index, Func_TreeNodeCount);
                        Arg_Parent.UpdateNodeIndicies(Arg_Parent.Index + 1);
                        UpdateRoot(Arg_Parent._Root);
                    }
                    _Parent = Arg_Parent;
                    ++Arg_Parent._SiblingCount;
                    Node_AddChildCount(Arg_Parent, Func_TreeNodeCount);
                    if (Func_PreviousParent != null) {
                        --Func_PreviousParent._SiblingCount;
                        Node_AddChildCount(Func_PreviousParent, -Func_TreeNodeCount);
                        InvokeArrayChildrenChanged(Func_OldParentNodes);
                    }

                    ChildrenChanged?.Invoke(UnsafeUtils.Reinterpret<Node<NT, CT>, NT>(this));
                    InvokeArrayChildrenChanged(Func_NewParentNodes);

                    NT[] Func_NewChildren = Children;
                    ParentChanged?.Invoke(UnsafeUtils.Reinterpret<Node<NT, CT>, NT>(this));
                    InvokeArrayParentChanged(Func_NewChildren);
                } catch (Exception Func_Exception) { Environment.FailFast(null, Func_Exception); throw; }
            } else {
                NT[] Func_OldParentNodes = Parents;
                try {
                    int Func_TreeNodeCount = _ChildCount + 1;
                    _Parent = null;
                    _Container = _Root._Container;
                    Nodes.AddRange(_Root.Nodes, Index, Func_TreeNodeCount);
                    _Root.Nodes.RemoveAt(Index, Func_TreeNodeCount);
                    UpdateNodeIndicies(0);
                    UpdateRoot(UnsafeUtils.Reinterpret<Node<NT, CT>, NT>(this));

                    --Func_PreviousParent!._SiblingCount;
                    Node_AddChildCount(Func_PreviousParent, -Func_TreeNodeCount);

                    InvokeArrayChildrenChanged(Func_OldParentNodes);

                    NT[] Func_NewChildren = Children;
                    ParentChanged?.Invoke(UnsafeUtils.Reinterpret<Node<NT, CT>, NT>(this));
                    InvokeArrayParentChanged(Func_NewChildren);
                } catch (Exception Func_Exception) { Environment.FailFast(null, Func_Exception); throw; }
            }
        }

        static void InvokeArrayParentChanged(NT[] Arg_Nodes) {
            ArgumentNullException.ThrowIfNull(Arg_Nodes);
            for (int Loop_Index = 0; Loop_Index < Arg_Nodes.Length; Loop_Index++) {
                NT Loop_Node = Arg_Nodes[Loop_Index];
                Loop_Node.ParentChanged?.Invoke(Loop_Node);
            }
        }

        static void InvokeArrayChildrenChanged(NT[] Arg_Nodes) {
            ArgumentNullException.ThrowIfNull(Arg_Nodes);
            for (int Loop_Index = 0; Loop_Index < Arg_Nodes.Length; Loop_Index++) {
                NT Loop_Node = Arg_Nodes[Loop_Index];
                Loop_Node.ChildrenChanged?.Invoke(Loop_Node);
            }
        }

#nullable disable
        static void Node_AddChildCount(NT Arg_This, int Arg_ChildCount) {
            Arg_This._ChildCount += Arg_ChildCount;
            for (Arg_This = Arg_This._Parent; Arg_This != null; Arg_This = Arg_This._Parent) {
                Arg_This._ChildCount += Arg_ChildCount;
            }
        }
#nullable enable

    }

    private void UpdateNodeIndicies(int Arg_StartIndex) {
        FastList<NT> Func_NodesList = _Root.Nodes;
        NT[] Func_Nodes = Func_NodesList.InnerArray;
        int Func_NodeCount = Func_NodesList.Count;
        for (int Loop_Index = Arg_StartIndex; Loop_Index < Func_NodeCount; ++Loop_Index) { Func_Nodes[Loop_Index].Index = Loop_Index; }
    }
    private void UpdateRoot(NT Arg_RootNode) {
        int Func_MaxChildIndex = Index + _ChildCount;
        NT[] Func_Nodes = Arg_RootNode.Nodes.InnerArray;
        for (int Loop_ChildIndex = Index + 1; Loop_ChildIndex <= Func_MaxChildIndex; ++Loop_ChildIndex) {
            Func_Nodes[Loop_ChildIndex]._Root = Arg_RootNode;
        }
    }
    private void MoveWithChildren(int Arg_DestinationIndex) {
        FastList<NT> Func_Nodes = _Root.Nodes;
        int Func_NodeCount = Func_Nodes.Count;
        int Func_SourceIndex = Index;
        if ((uint) Func_SourceIndex >= (uint) Func_NodeCount) {
            throw new ArgumentOutOfRangeException(nameof(Func_SourceIndex), Func_SourceIndex, "The node index is out of range!");
        }
        if ((uint) Arg_DestinationIndex > (uint) Func_NodeCount) {
            throw new ArgumentOutOfRangeException(nameof(Arg_DestinationIndex), Arg_DestinationIndex, "The node index is out of range!");
        }
        int Func_MoveNodeCount = Func_Nodes[Func_SourceIndex]._ChildCount + 1;

        // Notes: destination (parent) cannot be a child of source, source and destination should not be the same index

        //NT[] Func_MoveNodes = new NT[Func_MoveNodeCount];
        //Func_Nodes.CopyTo(Func_SourceIndex, Func_MoveNodes, 0, Func_MoveNodeCount);
        //Func_Nodes.RemoveAt(Func_SourceIndex, Func_MoveNodeCount);
        try {
            /* if (Arg_DestinationIndex < Func_SourceIndex) {
                Func_Nodes.InsertRange(Func_MoveNodes, Func_MoveNodeCount, Arg_DestinationIndex + Func_Nodes[Arg_DestinationIndex]._ChildCount + 1);
            } else {
                Func_Nodes.InsertRange(Func_MoveNodes, Func_MoveNodeCount, (Arg_DestinationIndex + Func_Nodes[Arg_DestinationIndex]._ChildCount + 1) - Func_MoveNodeCount);
            } */
            Func_Nodes.InsertRange(Func_Nodes, Func_MoveNodeCount, Func_SourceIndex, Arg_DestinationIndex + Func_Nodes[Arg_DestinationIndex]._ChildCount + 1);
            if (Arg_DestinationIndex < Func_SourceIndex) {
                Func_Nodes.RemoveAt(Func_SourceIndex + Func_MoveNodeCount, Func_MoveNodeCount);
            } else {
                Func_Nodes.RemoveAt(Func_SourceIndex, Func_MoveNodeCount);
            }
            for (int Loop_Index = Arg_DestinationIndex < Func_SourceIndex ? Arg_DestinationIndex : Func_SourceIndex; Loop_Index < Func_NodeCount; ++Loop_Index) { Func_Nodes[Loop_Index].Index = Loop_Index; }
        } catch (Exception Func_Exception) { Environment.FailFast(null, Func_Exception); throw; }
    }

    INode INode.GetSibling(int Arg_Index) => GetSibling(Arg_Index);
    public NT GetSibling(int Arg_Index) {
        if ((uint) Arg_Index >= (uint) _SiblingCount) {
            throw new ArgumentOutOfRangeException(nameof(Arg_Index), Arg_Index, "The index is out of range!");
        }
        NT? Func_LastSibling = _Root.Nodes[Index + 1];
        for (; Arg_Index > 0; Arg_Index--) {
            Func_LastSibling = _Root.Nodes[Func_LastSibling.Index + Func_LastSibling._ChildCount + 1];
        }
        return Func_LastSibling;
    }
    INode INode.GetChild(int Arg_Index) => GetChild(Arg_Index);
    public NT GetChild(int Arg_Index) => (uint) Arg_Index < (uint) _ChildCount ? _Root.Nodes[Index + Arg_Index + 1] : throw new ArgumentOutOfRangeException(nameof(Arg_Index), Arg_Index, "The index is out of range!");

    public unsafe bool ForEachSibling(Func<NT, object?, bool> Arg_Callback, object? Arg_UserData, bool Arg_IsReversed = false) {
        ArgumentNullException.ThrowIfNull(Arg_Callback);
        bool Func_ContinueRequested = true;
        NT[] Func_Nodes = _Root.Nodes.InnerArray;
        int Func_MaxNodeIndex = Index + _ChildCount;
        if (Arg_IsReversed == false) {
            for (int Loop_SiblingIndex = Index + 1; Loop_SiblingIndex <= Func_MaxNodeIndex && (Func_ContinueRequested = Arg_Callback(Func_Nodes[Loop_SiblingIndex], Arg_UserData)) == true; Loop_SiblingIndex += Func_Nodes[Loop_SiblingIndex]._ChildCount + 1) { }
        } else {
            Func_ContinueRequested = ReverseRecursiveForEachSibling(Arg_Callback, Arg_UserData, Index + 1, Func_MaxNodeIndex);
        }
        return Func_ContinueRequested;

        bool ReverseRecursiveForEachSibling(Func<NT, object?, bool> Arg_Callback, object? Arg_UserData, int Arg_SiblingIndex, int Arg_FinalNodeIndex) {
            bool Func_ContinueRequested = false;
            if (Arg_SiblingIndex <= Arg_FinalNodeIndex) {
                NT Func_Sibling = _Root.Nodes.InnerArray[Arg_SiblingIndex];
                Func_ContinueRequested = ReverseRecursiveForEachSibling(Arg_Callback, Arg_UserData, Arg_SiblingIndex + Func_Sibling._ChildCount + 1, Arg_FinalNodeIndex);
                if (Func_ContinueRequested == true) {
                    Func_ContinueRequested = Arg_Callback(Func_Sibling, Arg_UserData);
                }
            }
            return Func_ContinueRequested;
        }

    }

    public unsafe bool ForEachChild(Func<NT, object?, bool> Arg_Callback, object? Arg_UserData, bool Arg_IsReversed = false) {
        ArgumentNullException.ThrowIfNull(Arg_Callback);
        bool Func_ContinueRequested = true;
        NT[] Func_Nodes = _Root.Nodes.InnerArray;
        if (Arg_IsReversed == false) {
            int Func_MaxChildIndex = Index + _ChildCount;
            for (int Loop_ChildIndex = Index + 1; Loop_ChildIndex <= Func_MaxChildIndex && (Func_ContinueRequested = Arg_Callback(Func_Nodes[Loop_ChildIndex], Arg_UserData)) == true; ++Loop_ChildIndex) { }
        } else {
            int Func_NodeIndex = Index;
            for (int Loop_ChildIndex = Index + _ChildCount; Loop_ChildIndex != Func_NodeIndex && (Func_ContinueRequested = Arg_Callback(Func_Nodes[Loop_ChildIndex], Arg_UserData)) == true; --Loop_ChildIndex) { }
        }
        return Func_ContinueRequested;
    }

    public void CopyChildren(FastList<NT> Arg_DestinationList, int Arg_DestinationIndex) {
        ArgumentNullException.ThrowIfNull(Arg_DestinationList);
        Arg_DestinationList.InsertRange(_Root.Nodes, _ChildCount, Index + 1, Arg_DestinationIndex);
    }

    public bool HasChild(NT Arg_Node) {
        ArgumentNullException.ThrowIfNull(Arg_Node);
        bool Func_HasChild = false;
        if (_Root == Arg_Node._Root) {
            int Func_NodeIndex = Index;
            int Func_ChildIndex = Arg_Node.Index;
            Func_HasChild = Func_NodeIndex < Func_ChildIndex && Func_NodeIndex + _ChildCount >= Func_ChildIndex;
        }
        return Func_HasChild;
    }

    internal void MarkRemoved() {
        if (_Parent != null) {
            throw new InvalidOperationException("The node must be root!");
        }
        _Container = null;
    }

    public IEnumerator<NT> GetEnumerator() => new ArrayEnumerator<NT>(Siblings);
    IEnumerator<INode> IEnumerable<INode>.GetEnumerator() => new ArrayEnumerator<NT>(Siblings);
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => new ArrayEnumerator<NT>(Siblings);

}