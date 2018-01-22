using System;
using System.Collections.Generic;

namespace UkkonenSuffixTree.Library
{
    /// <summary>
    /// Direct port of below Ukkonen Suffix Tree implementation:
    /// http://programmerspatch.blogspot.com/2013/02/ukkonens-suffix-tree-algorithm.html
    /// https://github.com/schmidda/ukkonen-suffixtree
    /// </summary>
    public class SuffixTreeBuilder
    {
        protected SuffixTreeNode _root;
        protected SuffixTreeNode _f;
        protected SuffixTreeNode _current;
        protected SuffixTreeNodePosition _last;
        protected SuffixTreeNodePosition _oldBeta;
        /// <summary>
        /// Last value of j in the previous extension.
        /// </summary>
        protected int _oldj;
        /// <summary>
        /// End of currrent leaves.
        /// </summary>
        protected int _e;

        public SuffixTreeBuilder()
        {
            _root = null;
            _f = null;
            _current = null;
            _last = null;
            _oldBeta = null;
            _oldj = 0;
            _e = 0;
        }

        protected SuffixTreeNodePosition WalkDown(SuffixTreeNode v, SuffixTreePath p)
        {
            SuffixTreeNodePosition q = null;
            int start = p.Start;
            int len = p.Length;
            v = v.FindChild(v.Value[start]);
            while (len > 0)
            {
                if (len <= v.Length)
                {
                    q = new SuffixTreeNodePosition(v, v.Start + len - 1);
                    break;
                }
                else
                {
                    start += v.Length;
                    len -= v.Length;
                    v = v.FindChild(v.Value[start]);
                }
            }
            return q;
        }

        protected SuffixTreeNodePosition FindBeta(string str, int j, int i)
        {
            SuffixTreeNodePosition p = null;
            if (_oldj > 0 && _oldj == j)
            {
                p = _oldBeta;
            }
            else if (j > i)  // empty string
            {
                p = new SuffixTreeNodePosition(_root, 0);
            }
            else if (j == 0)    // entire string
            {
                p = new SuffixTreeNodePosition(_f, i);
            }
            else // walk across tree
            {
                SuffixTreeNode v = _last.Node;
                int len = _last.Location - _last.Node.Start + 1;
                SuffixTreePath q = new SuffixTreePath(v.Start, len);
                v = v.ParentNode;
                while (v != _root && v.SuffixLinkNode == null)
                {
                    q.Prepend(v.Length);
                    v = v.ParentNode;
                }
                if (v != _root)
                {
                    v = v.SuffixLinkNode;
                    p = WalkDown(v, q);
                }
                else
                {
                    p = WalkDown(_root, new SuffixTreePath(j, i - j + 1));
                }
            }
            _last = p;
            return p;
        }

        protected void UpdateCurrentSuffixLinkNode(SuffixTreeNode v)
        {
            if (_current != null)
            {
                _current.SuffixLinkNode = v;
                _current = null;
            }
        }

        protected void UpdateOldBeta(SuffixTreeNodePosition p, int i)
        {
            if (p.Node.GetEnd(_e) > p.Location)
            {
                _oldBeta = new SuffixTreeNodePosition(p.Node, p.Location + 1);
            }
            else
            {
                char c = i < p.Node.Value.Length ? p.Node.Value[i] : (char)0;
                SuffixTreeNode u = p.Node.FindChild(c);
                _oldBeta = new SuffixTreeNodePosition(u, u.Start);
            }
        }

        protected bool Extension(string str, int j, int i)
        {
            bool res = true;
            char c = i < str.Length ? str[i] : (char)0;
            SuffixTreeNodePosition p = FindBeta(str, j, i - 1);
            // rule 1 (once a leaf always a leaf)
            if (p.Node.IsLeaf && p.AtEdgeEnd(_e))
            {
                res = true;
            }
            // rule 2
            else if (!p.Continues(c, _e))
            {
                //printf("applying rule 2 at j=%d for phase %d\n",j,i);
                SuffixTreeNode leaf = new SuffixTreeNode(str, i);
                if (p.Node == _root || p.AtEdgeEnd(_e))
                {
                    p.Node.AddChildNode(leaf);
                    UpdateCurrentSuffixLinkNode(p.Node);
                }
                else
                {
                    SuffixTreeNode u = p.Node.SplitNode(p.Location);
                    UpdateCurrentSuffixLinkNode(u);
                    if (i - j == 1)
                    {
                        u.SuffixLinkNode = _root;
                    }
                    else
                    {
                        _current = u;
                    }
                    u.AddChildNode(leaf);
                }
                UpdateOldBeta(p, i);
            }
            // rule 3
            else
            {
                //printf("applying rule 3 at j=%d for phase %d\n",j,i);
                UpdateCurrentSuffixLinkNode(p.Node);
                UpdateOldBeta(p, i);
                res = false;
            }
            return res;
        }

        protected void Phase(string str, int i)
        {
            int j = 0;
            _current = null;
            for (j = _oldj; j <= i; ++j)
            {
                if (!Extension(str, j, i))
                    break;
            }
            // remember number of last extension for next phase
            _oldj = j > i ? i : j;
            // update all leaf ends
            ++_e;
        }

        protected void SetE(SuffixTreeNode v)
        {
            if (v.IsLeaf)
            {
                v.Length = _e - v.Start + 1;
            }
            IEnumerable<SuffixTreeNode> iter = v.GetChildren();
            foreach (SuffixTreeNode u in iter)
            {
                SetE(u);
            }
        }

        public SuffixTree BuildSuffixTree(string str)
        {
            _e = 0;
            _root = null;
            _f = null;
            _current = null;
            _last = new SuffixTreeNodePosition(null, 0);
            _oldBeta = new SuffixTreeNodePosition(null, 0);
            _oldj = 0;
            _root = new SuffixTreeNode(str);
            _root.PrintTree(_e);
            _f = new SuffixTreeNode(str, 0);
            // phase == 0.
            _root.AddChildNode(_f);
            _root.PrintTree(_e);
            // Handle phase > 0. Walk the string, one more than the length of the string.
            for (int i = 1; i <= str.Length; ++i)
            {
                Phase(str, i);
                Console.WriteLine($"phase [{i}]");
                _root.PrintTree(_e);
            }
            SetE(_root);
            return new SuffixTree(str, _root);
        }
    }
}
