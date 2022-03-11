#nullable disable
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
using Reaper1121.SharpToolbox.Utilities;

namespace Reaper1121.SharpToolbox.Collections {

    [SkipLocalsInit]
    public abstract class NodeContainer<NT, CT> : IEnumerable<NT> where NT : NodeContainer<NT, CT>.Node where CT : NodeContainer<NT, CT> {

        private const int INITIAL_CAPACITY = 8;

        [SkipLocalsInit]
        public abstract class Node : IEnumerable<NT> {

            public event Action<NT> ParentChanged;
            public event Action<NT> ChildrenChanged;
            public event Action<NT> NodeRemove;

            private readonly FastList<Node> Nodes = new FastList<Node>(INITIAL_CAPACITY);
            private int Index;
            private int _ChildCount;
            public int ChildCount => _ChildCount;
            public NT[] Children {
                get {
                    NT[] Func_Children = null;
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
                    NT[] Func_Siblings = null;
                    int Func_SiblingCount = _SiblingCount;
                    if (Func_SiblingCount != 0) {
                        Func_Siblings = new NT[Func_SiblingCount];
                        Node[] Func_ContainerNodes = _Root.Nodes.InnerArray;
                        int Func_SiblingIndex = Index + 1;
                        for (int Loop_Index = 0; Loop_Index < Func_SiblingCount; Loop_Index++) {
                            Node Loop_SiblingNode = Func_ContainerNodes[Func_SiblingIndex];
                            Func_SiblingIndex += Loop_SiblingNode._ChildCount + 1;
                            Func_Siblings[Loop_Index] = UnsafeUtils.Reinterpret<Node, NT>(Loop_SiblingNode);
                        }
                    }
                    return Func_Siblings;
                }
            }

            private Node _Parent;
            public NT Parent {
                get => UnsafeUtils.Reinterpret<Node, NT>(_Parent);
                set => SetParent(value, false);
            }
            public NT[] Parents {
                get {
                    FastList<Node> Func_DynamicParents = new FastList<Node>(16);
                    Node Func_ParentNode = this;
                    while ((Func_ParentNode = Func_ParentNode._Parent) != null) { Func_DynamicParents.Add(Func_ParentNode); }
                    Node[] Func_Parents = null;
                    int Func_ParentCount = Func_DynamicParents.Count;
                    if (Func_ParentCount != 0) {
                        Func_Parents = Func_DynamicParents.InnerArray;
                        Array.Resize(ref Func_Parents, Func_ParentCount);
                    }
                    return UnsafeUtils.Reinterpret<Node[], NT[]>(Func_Parents);
                }
            }
            private NodeContainer<NT, CT> _Container;
            public CT Container => UnsafeUtils.Reinterpret<NodeContainer<NT, CT>, CT>(_Root._Container);
            private Node _Root;
            public NT Root => UnsafeUtils.Reinterpret<Node, NT>(_Root);
            public int Depth {
                get {
                    int Func_Depth = 0;
                    for (Node Loop_CurrentNode = _Parent; Loop_CurrentNode != null; Loop_CurrentNode = Loop_CurrentNode._Parent) {
                        ++Func_Depth;
                    }
                    return Func_Depth;
                }
            }

            public bool IsBeingRemoved { get; internal set; }
            private bool RemovalNotified;
            // TOOD: IsReadOnly (Root) for "ForEach" methods to avoid memory allocations.

            public Node(CT Arg_NodeContainer) {
                NodeContainer<NT, CT> Func_NodeContainer = UnsafeUtils.Reinterpret<CT, NodeContainer<NT, CT>>(Arg_NodeContainer);
                Nodes.Add(this);
                _Root = this;
                _Container = Func_NodeContainer;
                if (Func_NodeContainer != null) {
                    Func_NodeContainer.Nodes.Add(this);
                    ++Func_NodeContainer._ChildCount;
                    try {
                        Func_NodeContainer.NodeAdd?.Invoke(UnsafeUtils.Reinterpret<NodeContainer<NT, CT>, CT>(Func_NodeContainer));
                    } catch (Exception Func_Exception) { Environment.FailFast(null, Func_Exception); throw; }
                }
            }

            private void SetParent(Node Arg_Parent, bool Arg_IgnoreRemoval) {
                if (Arg_Parent != this) {
                    Node Func_PreviousParent = _Parent;
                    if (ReferenceEquals(Arg_Parent, Func_PreviousParent) == false) {
                        if (Arg_IgnoreRemoval == true || IsBeingRemoved == false) {
                            if (Arg_Parent != null) {
                                if (Arg_IgnoreRemoval == true || Arg_Parent.IsBeingRemoved == false) {
                                    if (Arg_Parent.Container == Container) {
                                        if (HasChild(Arg_Parent) == false) {
                                            Node[] Func_OldParentNodes = Parents;
                                            Node[] Func_NewParentNodes = Arg_Parent.Parents;
                                            try {
                                                int Func_TreeNodeCount = _ChildCount + 1;
                                                if (_Root == Arg_Parent._Root) {
                                                    _Root.MoveNodeWithChildren(Index, Arg_Parent.Index);
                                                } else {
                                                    Arg_Parent._Root.Nodes.InsertRange(_Root.Nodes, Func_TreeNodeCount, Index, Arg_Parent.Index + Arg_Parent._ChildCount + 1);
                                                    _Root.Nodes.RemoveAt(Index, Func_TreeNodeCount);
                                                    Arg_Parent._Root.UpdateNodeIndicies(Arg_Parent.Index + 1);
                                                    UpdateNodeRoot(Arg_Parent._Root);
                                                    _Container = Arg_Parent._Root._Container;
                                                }
                                                _Parent = Arg_Parent;
                                                ++Arg_Parent._SiblingCount;
                                                Node_AddChildCount(Arg_Parent, Func_TreeNodeCount);
                                                if (Func_PreviousParent != null) {
                                                    --Func_PreviousParent._SiblingCount;
                                                    Node_AddChildCount(Func_PreviousParent, -Func_TreeNodeCount);
                                                    InvokeArrayChildrenChanged(Func_OldParentNodes);
                                                }

                                                ChildrenChanged?.Invoke(UnsafeUtils.Reinterpret<Node, NT>(this));
                                                InvokeArrayChildrenChanged(Func_NewParentNodes);

                                                Node[] Func_NewChildren = Children;
                                                ParentChanged?.Invoke(UnsafeUtils.Reinterpret<Node, NT>(this));
                                                InvokeArrayParentChanged(Func_NewChildren);
                                            } catch (Exception Func_Exception) { Environment.FailFast(null, Func_Exception); throw; }
                                        } else { throw new ArgumentException("The parent cannot be a child of the node!", nameof(Arg_Parent)); }
                                    } else { throw new ArgumentException("The parent cannot belong to a different container!", nameof(Arg_Parent)); }
                                } else { throw new ArgumentException("The new parent is in the process of being removed!", nameof(Arg_Parent)); }
                            } else {
                                Node[] Func_OldParentNodes = Parents;
                                try {
                                    int Func_TreeNodeCount = _ChildCount + 1;
                                    Nodes.InsertRange(_Root.Nodes, Func_TreeNodeCount, Index, 0);
                                    _Root.Nodes.RemoveAt(Index, Func_TreeNodeCount);
                                    _Parent = null;
                                    _Container = null;
                                    UpdateNodeIndicies(0);
                                    UpdateNodeRoot(this);

                                    --Func_PreviousParent._SiblingCount;
                                    Node_AddChildCount(Func_PreviousParent, -Func_TreeNodeCount);

                                    InvokeArrayChildrenChanged(Func_OldParentNodes);

                                    Node[] Func_NewChildren = Children;
                                    ParentChanged?.Invoke(UnsafeUtils.Reinterpret<Node, NT>(this));
                                    InvokeArrayParentChanged(Func_NewChildren);
                                } catch (Exception Func_Exception) { Environment.FailFast(null, Func_Exception); throw; }
                            }
                        } else { throw new InvalidOperationException("The node is in the process of being removed!"); }
                    }
                } else { throw new ArgumentException("The parent cannot reference it self!", nameof(Arg_Parent)); }

                static void InvokeArrayParentChanged(Node[] Arg_Nodes) {
                    if (Arg_Nodes != null) {
                        for (int Loop_Index = 0; Loop_Index < Arg_Nodes.Length; Loop_Index++) {
                            Node Loop_Node = Arg_Nodes[Loop_Index];
                            Loop_Node.ParentChanged?.Invoke(UnsafeUtils.Reinterpret<Node, NT>(Loop_Node));
                        }
                    }
                }

                static void InvokeArrayChildrenChanged(Node[] Arg_Nodes) {
                    if (Arg_Nodes != null) {
                        for (int Loop_Index = 0; Loop_Index < Arg_Nodes.Length; Loop_Index++) {
                            Node Loop_Node = Arg_Nodes[Loop_Index];
                            Loop_Node.ChildrenChanged?.Invoke(UnsafeUtils.Reinterpret<Node, NT>(Loop_Node));
                        }
                    }
                }

                static void Node_AddChildCount(Node Arg_This, int Arg_ChildCount) {
                    Arg_This._ChildCount += Arg_ChildCount;
                    for (Arg_This = Arg_This._Parent; Arg_This != null; Arg_This = Arg_This._Parent) {
                        Arg_This._ChildCount += Arg_ChildCount;
                    }
                }

            }

            private void UpdateNodeIndicies(int Arg_StartIndex) {
                FastList<Node> Func_NodesList = Nodes;
                Node[] Func_Nodes = Func_NodesList.InnerArray;
                int Func_NodeCount = Func_NodesList.Count;
                for (int Loop_Index = Arg_StartIndex; Loop_Index < Func_NodeCount; ++Loop_Index) { Func_Nodes[Loop_Index].Index = Loop_Index; }
            }
            private void UpdateNodeRoot(Node Arg_RootNode) {
                int Func_MaxChildIndex = Index + _ChildCount;
                Node[] Func_Nodes = _Root.Nodes.InnerArray;
                for (int Loop_ChildIndex = Index + 1; Loop_ChildIndex <= Func_MaxChildIndex; ++Loop_ChildIndex) {
                    Func_Nodes[Loop_ChildIndex]._Root = Arg_RootNode;
                }
            }
            private void MoveNodeWithChildren(int Arg_SourceIndex, int Arg_DestinationIndex) {
                int Func_NodeCount = Nodes.Count;
                if ((uint) Arg_SourceIndex < (uint) Func_NodeCount) {
                    if ((uint) Arg_DestinationIndex <= (uint) Func_NodeCount) {
                        FastList<Node> Func_Nodes = Nodes;
                        Node Func_SourceNode = Func_Nodes[Arg_SourceIndex];
                        Node Func_DestinationNode = Func_Nodes[Arg_DestinationIndex];

                        int Func_MoveNodeCount = Func_SourceNode._ChildCount + 1;
                        Node[] Func_MoveNodes = new Node[Func_MoveNodeCount];
                        Func_Nodes.CopyTo(Arg_SourceIndex, Func_MoveNodes, 0, Func_MoveNodeCount);
                        Func_Nodes.RemoveAt(Arg_SourceIndex, Func_MoveNodeCount);
                        try {
                            if (Arg_DestinationIndex < Arg_SourceIndex) {
                                Func_Nodes.InsertRange(Func_MoveNodes, Func_MoveNodeCount, Arg_DestinationIndex + Func_DestinationNode._ChildCount + 1);
                            } else {
                                Func_Nodes.InsertRange(Func_MoveNodes, Func_MoveNodeCount, (Arg_DestinationIndex + Func_DestinationNode._ChildCount + 1) - Func_MoveNodeCount);
                            }
                            for (int Loop_Index = ((Arg_DestinationIndex < Arg_SourceIndex) ? Func_DestinationNode : Func_SourceNode).Index; Loop_Index < Func_NodeCount; ++Loop_Index) { Func_Nodes[Loop_Index].Index = Loop_Index; }
                        } catch (Exception Func_Exception) { Environment.FailFast(null, Func_Exception); throw; }
                    } else { throw new ArgumentOutOfRangeException(nameof(Arg_DestinationIndex), Arg_DestinationIndex, "The node index is out of range!"); }
                } else { throw new ArgumentOutOfRangeException(nameof(Arg_SourceIndex), Arg_SourceIndex, "The node index is out of range!"); }
            }

            public unsafe bool ForEachSibling(delegate*<NT, bool> Arg_Callback, bool Arg_IsReversed = false) {
                bool Func_ExitStatus = true;
                if (Arg_Callback != null) {
                    int Func_SiblingCount = _SiblingCount;
                    if (Func_SiblingCount != 0) {
                        Node[] Func_Nodes = _Root.Nodes.InnerArray;
                        if (Arg_IsReversed == false) {
                            int Func_SiblingIndex = Index + 1;
                            for (--Func_SiblingCount; Func_SiblingCount != -1; --Func_SiblingCount) {
                                Node Loop_SiblingNode = Func_Nodes[Func_SiblingIndex];
                                Func_SiblingIndex += Loop_SiblingNode._ChildCount + 1;
                                if (Arg_Callback(UnsafeUtils.Reinterpret<Node, NT>(Loop_SiblingNode)) == false) { Func_ExitStatus = false; break; }
                            }
                        } else {
                            throw new NotImplementedException();
                            //int Func_LastSiblingIndex = Index + 1;
                            //for (int Loop_Index = 1; Loop_Index < Func_SiblingCount; ++Loop_Index) {
                            //    Node Loop_SiblingNode = Func_Nodes[Func_LastSiblingIndex];
                            //    Func_LastSiblingIndex += Loop_SiblingNode._ChildCount + 1;
                            //}
                            //for (--Func_SiblingCount; Func_SiblingCount != -1; --Func_SiblingCount) {
                            //    Node Loop_SiblingNode = Func_Nodes[Func_LastSiblingIndex];
                            //    Func_LastSiblingIndex += Loop_SiblingNode._ChildCount + 1;
                            //    if (Arg_Callback(UnsafeUtils.Reinterpret<Node, NT>(Loop_SiblingNode)) == false) { Func_ExitStatus = false; break; }
                            //}
                        }
                    }
                } else { throw new ArgumentNullException(nameof(Arg_Callback)); }
                return Func_ExitStatus;
            }
            public unsafe bool ForEachSibling<DATA>(ref DATA Arg_CallbackData, delegate*<NT, ref DATA, bool> Arg_Callback, bool Arg_IsReversed = false) {
                bool Func_ExitStatus = true;
                if (Arg_Callback != null) {
                    int Func_SiblingCount = _SiblingCount;
                    if (Func_SiblingCount != 0) {
                        Node[] Func_Nodes = _Root.Nodes.InnerArray;
                        if (Arg_IsReversed == false) {
                            int Func_SiblingIndex = Index + 1;
                            for (--Func_SiblingCount; Func_SiblingCount != -1; --Func_SiblingCount) {
                                Node Loop_SiblingNode = Func_Nodes[Func_SiblingIndex];
                                Func_SiblingIndex += Loop_SiblingNode._ChildCount + 1;
                                if (Arg_Callback(UnsafeUtils.Reinterpret<Node, NT>(Loop_SiblingNode), ref Arg_CallbackData) == false) { Func_ExitStatus = false; break; }
                            }
                        } else {
                            throw new NotImplementedException();
                        }
                    }
                } else { throw new ArgumentNullException(nameof(Arg_Callback)); }
                return Func_ExitStatus;
            }

            public unsafe bool ForEachChild(delegate*<NT, bool> Arg_Callback, bool Arg_IsReversed = false) {
                bool Func_ExitStatus = true;
                if (Arg_Callback != null) {
                    Node[] Func_Nodes = _Root.Nodes.InnerArray;
                    if (Arg_IsReversed == false) {
                        int Func_MaxChildIndex = Index + _ChildCount;
                        for (int Loop_ChildIndex = Index + 1; Loop_ChildIndex <= Func_MaxChildIndex; ++Loop_ChildIndex) {
                            if (Arg_Callback(UnsafeUtils.Reinterpret<Node, NT>(Func_Nodes[Loop_ChildIndex])) == false) { Func_ExitStatus = false; break; }
                        }
                    } else {
                        int Func_NodeIndex = Index;
                        for (int Loop_ChildIndex = Index + _ChildCount; Loop_ChildIndex != Func_NodeIndex; --Loop_ChildIndex) {
                            if (Arg_Callback(UnsafeUtils.Reinterpret<Node, NT>(Func_Nodes[Loop_ChildIndex])) == false) { Func_ExitStatus = false; break; }
                        }
                    }
                } else { throw new ArgumentNullException(nameof(Arg_Callback)); }
                return Func_ExitStatus;
            }
            public unsafe bool ForEachChild<DATA>(ref DATA Arg_CallbackData, delegate*<NT, ref DATA, bool> Arg_Callback, bool Arg_IsReversed = false) {
                bool Func_ExitStatus = true;
                if (Arg_Callback != null) {
                    Node[] Func_Nodes = _Root.Nodes.InnerArray;
                    if (Arg_IsReversed == false) {
                        int Func_MaxChildIndex = Index + _ChildCount;
                        for (int Loop_ChildIndex = Index + 1; Loop_ChildIndex <= Func_MaxChildIndex; ++Loop_ChildIndex) {
                            if (Arg_Callback(UnsafeUtils.Reinterpret<Node, NT>(Func_Nodes[Loop_ChildIndex]), ref Arg_CallbackData) == false) { Func_ExitStatus = false; break; }
                        }
                    } else {
                        int Func_NodeIndex = Index;
                        for (int Loop_ChildIndex = Index + _ChildCount; Loop_ChildIndex != Func_NodeIndex; --Loop_ChildIndex) {
                            if (Arg_Callback(UnsafeUtils.Reinterpret<Node, NT>(Func_Nodes[Loop_ChildIndex]), ref Arg_CallbackData) == false) { Func_ExitStatus = false; break; }
                        }
                    }
                } else { throw new ArgumentNullException(nameof(Arg_Callback)); }
                return Func_ExitStatus;
            }

            public void CopyChildren(FastList<NT> Arg_DestinationList, int Arg_DestinationIndex) {
                if (Arg_DestinationList != null) {
                    Arg_DestinationList.InsertRange(UnsafeUtils.Reinterpret<FastList<Node>, FastList<NT>>(Nodes), _ChildCount, Index + 1, Arg_DestinationIndex);
                } else { throw new ArgumentNullException(nameof(Arg_DestinationList)); }
            }

            public bool HasChild(NT Arg_Node) => HasChild(UnsafeUtils.Reinterpret<NT, Node>(Arg_Node));
            private bool HasChild(Node Arg_Node) {
                bool Func_HasChild = false;
                if (Arg_Node != null) {
                    if (_Root == Arg_Node._Root) {
                        int Func_NodeIndex = Index;
                        int Func_ChildIndex = Arg_Node.Index;
                        Func_HasChild = Func_NodeIndex < Func_ChildIndex && Func_NodeIndex + _ChildCount >= Func_ChildIndex;
                    }
                }
                return Func_HasChild;
            }

            public void Remove() {
                if (_Parent == null) {
                    NodeContainer<NT, CT> Func_NodeContainer;
                    if (IsBeingRemoved == false && (Func_NodeContainer = _Container) != null) {
                        try {
                            int Func_RemoveCount = _ChildCount + 1;
                            Node[] Func_RemoveNodes = Nodes.InnerArray;
                            for (int Loop_Index = 0; Loop_Index < Func_RemoveCount; ++Loop_Index) { Func_RemoveNodes[Loop_Index].IsBeingRemoved = true; }
                            SetParent(null, true);
                            for (int Loop_Index = 0; Loop_Index < Func_RemoveCount; ++Loop_Index) {
                                Node Loop_ChildNode = Func_RemoveNodes[Loop_Index];
                                if (Loop_ChildNode.RemovalNotified == false) {
                                    Loop_ChildNode.RemovalNotified = true;
                                    Loop_ChildNode.NodeRemove?.Invoke(UnsafeUtils.Reinterpret<Node, NT>(Loop_ChildNode));
                                    Func_NodeContainer.NodeRemove?.Invoke(UnsafeUtils.Reinterpret<NodeContainer<NT, CT>, CT>(Func_NodeContainer));
                                }
                            }
                            _Container = null;
                            Func_NodeContainer._ChildCount -= Func_RemoveCount;
                            Func_NodeContainer.Nodes.Remove(this);
                        } catch (Exception Func_Exception) { Environment.FailFast(null, Func_Exception); throw; }
                    }
                } else { throw new InvalidOperationException("The node cannot have a parent for it to be removed!"); }
            }

            public IEnumerator<NT> GetEnumerator() => new ArrayEnumerator<NT>(Siblings);
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => new ArrayEnumerator<NT>(Siblings);

        }

        public event Action<CT> NodeRemove;
        public event Action<CT> NodeAdd;

        private int _ChildCount;
        public int ChildCount => _ChildCount;
        public NT[] Children {
            get {
                NT[] Func_Children = null;
                int Func_ChildCount = _ChildCount;
                if (Func_ChildCount != 0) {
                    FastList<NT> Func_ChildrenList = new FastList<NT>(Func_ChildCount);
                    Func_Children = Func_ChildrenList.InnerArray;
                    FastList<Node> Func_Siblings = Nodes;
                    int Func_SiblingCount = Nodes.Count;
                    for (int Loop_SiblingIndex = 0; Loop_SiblingIndex < Func_SiblingCount; Loop_SiblingIndex++) {
                        Node Loop_Sibling = Func_Siblings[Loop_SiblingIndex];
                        Func_ChildrenList.Add(UnsafeUtils.Reinterpret<Node, NT>(Loop_Sibling));
                        Loop_Sibling.CopyChildren(Func_ChildrenList, Func_ChildrenList.Count);
                    }
                }
                return Func_Children;
            }
        }
        public int SiblingCount => Nodes.Count;
        public NT[] Siblings => Nodes.Count != 0 ? UnsafeUtils.Reinterpret<Node[], NT[]>(Nodes.ToArray()) : null;

        private readonly FastList<Node> Nodes = new FastList<Node>(INITIAL_CAPACITY);

        public bool HasChild(NT Arg_Node) => Arg_Node?.Container == UnsafeUtils.Reinterpret<NodeContainer<NT, CT>, CT>(this);

        public unsafe void ForEachSibling(delegate*<NT, bool> Arg_Callback, bool Arg_IsReversed = false) {
            if (Arg_Callback != null) {
                if (_ChildCount != 0) {
                    Node[] Func_SiblingNodes = Nodes.InnerArray;
                    if (Arg_IsReversed == false) {
                        int Func_SiblingCount = Nodes.Count;
                        for (int Loop_Index = 0; Loop_Index < Func_SiblingCount && Arg_Callback(UnsafeUtils.Reinterpret<Node, NT>(Func_SiblingNodes[Loop_Index])) == true; ++Loop_Index) { }
                    } else {
                        for (int Loop_Index = Nodes.Count - 1; Loop_Index != -1 && Arg_Callback(UnsafeUtils.Reinterpret<Node, NT>(Func_SiblingNodes[Loop_Index])) == true; --Loop_Index) { }
                    }
                }
            } else { throw new ArgumentNullException(nameof(Arg_Callback)); }
        }
        public unsafe void ForEachSibling<DATA>(ref DATA Arg_CallbackData, delegate*<NT, ref DATA, bool> Arg_Callback, bool Arg_IsReversed = false) {
            if (Arg_Callback != null) {
                if (_ChildCount != 0) {
                    Node[] Func_SiblingNodes = Nodes.InnerArray;
                    if (Arg_IsReversed == false) {
                        int Func_SiblingCount = Nodes.Count;
                        for (int Loop_Index = 0; Loop_Index < Func_SiblingCount && Arg_Callback(UnsafeUtils.Reinterpret<Node, NT>(Func_SiblingNodes[Loop_Index]), ref Arg_CallbackData) == true; ++Loop_Index) { }
                    } else {
                        for (int Loop_Index = Nodes.Count - 1; Loop_Index != -1 && Arg_Callback(UnsafeUtils.Reinterpret<Node, NT>(Func_SiblingNodes[Loop_Index]), ref Arg_CallbackData) == true; --Loop_Index) { }
                    }
                }
            } else { throw new ArgumentNullException(nameof(Arg_Callback)); }
        }

        public unsafe void ForEachChild(delegate*<NT, bool> Arg_Callback, bool Arg_IsReversed = false) {
            if (Arg_Callback != null) {
                if (_ChildCount != 0) {
                    Node[] Func_SiblingNodes = Nodes.InnerArray;
                    if (Arg_IsReversed == false) {
                        int Func_SiblingCount = Nodes.Count;
                        for (int Loop_Index = 0; Loop_Index < Func_SiblingCount; ++Loop_Index) {
                            Node Loop_SiblingNode = Func_SiblingNodes[Loop_Index];
                            if (Arg_Callback(UnsafeUtils.Reinterpret<Node, NT>(Loop_SiblingNode)) == false || Loop_SiblingNode.ForEachChild(Arg_Callback, Arg_IsReversed) == false) { break; }
                        }
                    } else {
                        for (int Loop_Index = Nodes.Count - 1; Loop_Index != -1; --Loop_Index) {
                            Node Loop_SiblingNode = Func_SiblingNodes[Loop_Index];
                            if (Arg_Callback(UnsafeUtils.Reinterpret<Node, NT>(Loop_SiblingNode)) == false || Loop_SiblingNode.ForEachChild(Arg_Callback, Arg_IsReversed) == false) { break; }
                        }
                    }
                }
            } else { throw new ArgumentNullException(nameof(Arg_Callback)); }
        }
        public unsafe void ForEachChild<DATA>(ref DATA Arg_CallbackData, delegate*<NT, ref DATA, bool> Arg_Callback, bool Arg_IsReversed = false) {
            if (Arg_Callback != null) {
                if (_ChildCount != 0) {
                    Node[] Func_SiblingNodes = Nodes.InnerArray;
                    if (Arg_IsReversed == false) {
                        int Func_SiblingCount = Nodes.Count;
                        for (int Loop_Index = 0; Loop_Index < Func_SiblingCount; ++Loop_Index) {
                            Node Loop_SiblingNode = Func_SiblingNodes[Loop_Index];
                            if (Arg_Callback(UnsafeUtils.Reinterpret<Node, NT>(Loop_SiblingNode), ref Arg_CallbackData) == false || Loop_SiblingNode.ForEachChild(ref Arg_CallbackData, Arg_Callback, Arg_IsReversed) == false) { break; }
                        }
                    } else {
                        for (int Loop_Index = Nodes.Count - 1; Loop_Index != -1; --Loop_Index) {
                            Node Loop_SiblingNode = Func_SiblingNodes[Loop_Index];
                            if (Arg_Callback(UnsafeUtils.Reinterpret<Node, NT>(Loop_SiblingNode), ref Arg_CallbackData) == false || Loop_SiblingNode.ForEachChild(ref Arg_CallbackData, Arg_Callback, Arg_IsReversed) == false) { break; }
                        }
                    }
                }
            } else { throw new ArgumentNullException(nameof(Arg_Callback)); }
        }

        public void ClearNodes() {
            int Func_SiblingCount = Nodes.Count;
            if (Func_SiblingCount != 0) {
                NT[] Func_Siblings = Siblings;
                for (--Func_SiblingCount; Func_SiblingCount != -1; --Func_SiblingCount) {
                    Func_Siblings[Func_SiblingCount].Remove();
                }
            }
        }

        public IEnumerator<NT> GetEnumerator() => UnsafeUtils.Reinterpret<IEnumerator<Node>, IEnumerator<NT>>(Nodes.GetEnumerator());
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => Nodes.GetEnumerator();

    }

}