using System;

namespace UkkonenSuffixTree.Library
{
    /// <summary>
    /// Direct port of below Ukkonen Suffix Tree implementation:
    /// http://programmerspatch.blogspot.com.au/2013/02/ukkonens-suffix-tree-algorithm.html
    /// https://github.com/schmidda/ukkonen-suffixtree
    /// </summary>
    public class SuffixTree : Node<string>
    {
        public SuffixTreeNode RootNode { get; set; }

        public SuffixTree(string data, SuffixTreeNode root)
        {
            Value = data;
            RootNode = root;
        }

        public SuffixTree()
            : this(null, null)
        {
        }

        public void PrintTree()
        {
            RootNode.PrintTree(Value.Length);
        }
    }
}
