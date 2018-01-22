using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UkkonenSuffixTree.Library
{
    /// <summary>
    /// Direct port of below Ukkonen Suffix Tree implementation:
    /// http://programmerspatch.blogspot.com/2013/02/ukkonens-suffix-tree-algorithm.html
    /// https://github.com/schmidda/ukkonen-suffixtree
    /// </summary>
    public class SuffixTreeNode : Node<string>
    {
        public static readonly int MAX_LIST_CHILDREN = 6;
        public static readonly int LEN_MASK = 0x7FFFFFFF;
        public static readonly int KIND_MASK = unchecked((int)0x80000000);

        public bool PARENT_HASH(SuffixTreeNode p) => (p.Length & KIND_MASK) == unchecked((int)0x80000000);
        public bool PARENT_LIST(SuffixTreeNode p) => (p.Length & KIND_MASK) == 0;

        public static readonly int BAR_VALUE = 61708863;
        public static readonly int BAR_SPACE = 536870912;

        protected string _valueDerived;
        public string ValueDerived
        {
            get
            {
                if (_valueDerived == null)
                {
                    _valueDerived = GenerateLabel(this, Value.Length);
                }
                return _valueDerived;
            }
        }

        protected int _start;
        public int Start
        {
            get
            {
                return _start;
            }
            set
            {
                _start = value;
                _valueDerived = null;
            }
        }

        protected int _length;
        public int Length
        {
            get { return LEN_MASK & _length; }
            set
            {
                _length = (_length & KIND_MASK) + value;
                _valueDerived = null;
            }
        }
        public SuffixTreeNode SuffixLinkNode { get; set; }
        public SuffixTreeNode NextNode { get; set; }
        public SuffixTreeNode ParentNode { get; set; }
        public SuffixTreeNode ChildNode { get; set; }
        public IDictionary<char, LinkedList<SuffixTreeNode>> ChildNodeTable { get; protected set; }

        public bool IsLeaf
        {
            get
            {
                if (PARENT_LIST(this))
                    return ChildNode == null;
                else
                    return false;
            }
        }

        public int Kind
        {
            get
            {
                return Length & KIND_MASK;
            }
        }

        /// <summary>
        /// Constructor. Instantiates a non-leaf SuffixTreeNode with parentNode.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="parentNode"></param>
        /// <remarks>
        /// Leaf SuffixTreeNode is identified by Length != int.MaxValue.
        /// </remarks>
        public SuffixTreeNode(string data, int start, int length, SuffixTreeNode parentNode)
        {
            Value = data;
            Start = start;
            Length = length;
            ParentNode = parentNode;
        }

        /// <summary>
        /// Constructor. Instantiates a non-leaf SuffixTreeNode.
        /// If start and location is initialized to 0, then
        /// SuffixTreeNode is a root node.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <remarks>
        /// Leaf SuffixTreeNode is identified by Length != int.MaxValue.
        /// </remarks>
        public SuffixTreeNode(string data, int start, int length)
            : this(data, start, length, null)
        {
        }

        /// <summary>
        /// Constructor. Instantiates a leaf SuffixTreeNode with parentNode.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="parentNode"></param>
        /// <remarks>
        /// Leaf SuffixTreeNode is identified by Length == int.MaxValue.
        /// </remarks>
        public SuffixTreeNode(string data, int start, SuffixTreeNode parentNode)
            : this(data, start, int.MaxValue, parentNode)
        {
        }

        /// <summary>
        /// Constructor. Instantiates a leaf SuffixTreeNode.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <remarks>
        /// Leaf SuffixTreeNode is identified by Length == int.MaxValue.
        /// </remarks>
        public SuffixTreeNode(string data, int start)
            : this(data, start, int.MaxValue, null)
        {
        }

        /// <summary>
        /// Default constructor. Instantiates a root SuffixTreeNode.
        /// </summary>
        /// <remarks>
        /// Initializes start = 0, length = 0, and ParentNode = null.
        /// </remarks>
        public SuffixTreeNode(string data)
            : this(data, 0, 0, null)
        {
        }

        /// <summary>
        /// Default constructor. Instantiates a root SuffixTreeNode.
        /// </summary>
        /// <remarks>
        /// Initializes start = 0, length = 0, and ParentNode = null.
        /// </remarks>
        public SuffixTreeNode()
            : this(null, 0, 0, null)
        {
        }

        public int GetEnd(int max)
        {
            if (Length == int.MaxValue)
                return max;
            else
                return Start + Length - 1;
        }

        public SuffixTreeNode FindChild(char c)
        {
            SuffixTreeNode v = this;
            if (PARENT_LIST(this))
            {
                v = v.ChildNode;
                char tempChar = v.Start < v.Value.Length ? v.Value[v.Start] : (char)0;
                while (v != null && tempChar != c)
                {
                    v = v.NextNode;
                    tempChar = (char)0;
                    if (v != null)
                        tempChar = v.Start < v.Value.Length ? v.Value[v.Start] : (char)0;
                }
                return v;
            }
            else if (PARENT_HASH(v))
            {
                SuffixTreeNode u = null;
                if (v.ChildNodeTable != null)
                {
                    LinkedList<SuffixTreeNode> childNodeList = null;
                    if (v.ChildNodeTable.TryGetValue(c, out childNodeList))
                    {
                        u = childNodeList.Last.Value;
                    }
                }
                return u;
            }
            else
            {
                return null;
            }
        }

        public int GetNumChildren()
        {
            int size = 0;
            if (PARENT_LIST(this))
            {
                SuffixTreeNode temp = ChildNode;
                while (temp != null)
                {
                    ++size;
                    temp = temp.NextNode;
                }
            }
            else
            {
                size = ChildNodeTable?.Count ?? 0;
            }
            return size;
        }

        public IEnumerable<SuffixTreeNode> GetChildren()
        {
            LinkedList<SuffixTreeNode> iter = new LinkedList<SuffixTreeNode>();
            if (PARENT_LIST(this))
            {
                SuffixTreeNode v = ChildNode;
                while (v != null)
                {
                    iter.AddLast(v);
                    v = v.NextNode;
                }
            }
            else
            {
                if (ChildNodeTable != null)
                {
                    LinkedList<SuffixTreeNode> b = null;
                    foreach (char c in ChildNodeTable.Keys)
                    {
                        if (ChildNodeTable.TryGetValue(c, out b))
                        {
                            foreach (SuffixTreeNode node in b)
                            {
                                iter.AddLast(node);
                            }
                        }
                    }
                }
            }
            return iter;
        }

        protected void AddChildToChildNodeTable(SuffixTreeNode node)
        {
            if (ChildNodeTable == null)
                ChildNodeTable = new Dictionary<char, LinkedList<SuffixTreeNode>>();

            char c = node.Value.FirstOrDefault();
            LinkedList<SuffixTreeNode> childNodeList = null;
            if (ChildNodeTable.TryGetValue(c, out childNodeList))
            {
                childNodeList.AddLast(node);
            }
            else
            {
                childNodeList = new LinkedList<SuffixTreeNode>();
                childNodeList.AddLast(node);
                ChildNodeTable.Add(c, childNodeList);
            }
        }

        public void AppendSiblingNode(SuffixTreeNode node)
        {
            SuffixTreeNode temp = ChildNode;
            int size = 1;
            while(temp.NextNode != null)
            {
                ++size;
                if (size >= MAX_LIST_CHILDREN)
                {
                    AddChildToChildNodeTable(node);
                    return;
                }
                temp = temp.NextNode;
            }
            temp.NextNode = node;
        }

        public void AddChildNode(SuffixTreeNode node)
        {
            if (PARENT_LIST(this))
            {
                if (ChildNode == null)
                {
                    ChildNode = node;
                }
                else
                {
                    AppendSiblingNode(node);
                }
            }
            else
            {
                AddChildToChildNodeTable(node);
            }
            node.ParentNode = this;
        }

        public void ReplaceChildNode(SuffixTreeNode v, SuffixTreeNode u)
        {
            if (PARENT_LIST(v.ParentNode))
            {
                // isolate v and repair the list of children
                SuffixTreeNode child = v.ParentNode.ChildNode;
                SuffixTreeNode prev = child;
                while (child != null && child != v)
                {
                    prev = child;
                    child = child.NextNode;
                }
                if (child == prev)
                    v.ParentNode.ChildNode = u;
                else
                    prev.NextNode = u;
                u.NextNode = child.NextNode;
                v.NextNode = null;
                //node_print_children(v.ParentNode);
            }
            else if (PARENT_HASH(v.ParentNode))
            {
                if (ChildNodeTable != null)
                {
                    char c = u.Value.FirstOrDefault();
                    LinkedList<SuffixTreeNode> childNodeList = null;
                    if (ChildNodeTable.TryGetValue(c, out childNodeList))
                    {
                        childNodeList.Remove(v);
                        childNodeList.AddLast(u);
                    }
                }
            }
        }

        public SuffixTreeNode SplitNode(int loc)
        {
            // create front edge u leading to internal node v
            int u_len = loc - Start + 1;
            SuffixTreeNode u = new SuffixTreeNode(Value, Start, u_len);
            // now shorten the following node v
            if (!IsLeaf)
                Length -= u_len;
            // replace v with u in the children of v->parent
            ReplaceChildNode(this, u);
            Start = loc + 1;
            // reset parents
            u.ParentNode = ParentNode;
            ParentNode = u;
            // NB v is the ONLY child of u
            u.ChildNode = this;
            return u;
        }

        protected void PrintBars(int[] bars, bool skip_last)
        {
            if (bars != null)
            {
                int j, i = 0;
                while (bars[i] != 0)
                {
                    int bar_value = bars[i] & BAR_VALUE;
                    for (j = 0; j < bar_value; j++)
                        Console.Write(" ");
                    if (!skip_last || bars[i + 1] != 0)
                    {
                        if ((bars[i] & BAR_SPACE) > 0)
                            Console.Write(" ");
                        else
                            Console.Write("|");
                    }
                    i++;
                }
            }
        }

        protected void PrintBarLine(int[] bars)
        {
            PrintBars(bars, false);
            Console.WriteLine();
        }

        protected void SetLastBar(int[] bars, int mode)
        {
            int i = 0;
            if (bars != null)
            {
                while (i < bars.Length && bars[i] != 0)
                    i++;
            }
            if (i > 0)
                bars[i - 1] |= mode;
        }

        protected string GenerateLabel(SuffixTreeNode v, int e)
        {
            StringBuilder sb = new StringBuilder();
            int i, start, end;
            end = v.GetEnd(e);
            start = v.Start;
            for (i = v.Start; i <= end; i++)
            {
                char c = i < v.Value.Length ? v.Value[i] : (char)0;
                if (c == 0)
                    sb.Append("$");
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        protected int PrintLabel(SuffixTreeNode v, int e)
        {
            int start, end;
            end = v.GetEnd(e);
            start = v.Start;
            Console.Write(GenerateLabel(v, e));
            // print terminal star for unfinished leaves
            if (v.GetNumChildren() == 0 && e < v.Value.Length)
                Console.Write("*");
            return end - start + 1;
        }

        protected void PrintNode(IEnumerable<SuffixTreeNode> iter, int[] bars, int e)
        {
            int depth;
            bool first = true;
            SuffixTreeNode lastNode = iter.LastOrDefault();
            foreach (SuffixTreeNode u in iter)
            {
                if (!first)
                {
                    PrintBarLine(bars);
                    PrintBars(bars, true);
                    first = false;
                }
                if (u == lastNode)
                    SetLastBar(bars, BAR_SPACE);
                Console.Write("-");
                depth = PrintLabel(u, e);
                if (u.IsLeaf)
                    Console.WriteLine();
                else
                    PrintNode(u.GetChildren(), AddBar(bars, depth), e);
            }
        }

        protected int[] AddBar(int[] bars, int bar)
        {
            int nbars = 0;
            // count bars
            if (bars != null)
            {
                int[] temp = bars;
                while (nbars < temp.Length && temp[nbars] != 0)
                    nbars++;
            }
            int[] new_bars = new int[nbars + 1];
            if (new_bars != null)
            {
                int i;
                for (i = 0; i < nbars; i++)
                    new_bars[i] = bars[i];
                new_bars[i] = bar;
            }
            return new_bars;
        }

        public void PrintTree(int e)
        {
            int depth = 0;
            Console.Write("[R]");
            ++depth;
            PrintNode(GetChildren(), AddBar(null, depth), e);
            Console.WriteLine("\n");
        }
    }
}
