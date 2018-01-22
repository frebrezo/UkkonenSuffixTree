using System;

namespace UkkonenSuffixTree.Library
{
    /// <summary>
    /// Describes a character-positon in the tree.
    /// Direct port of below Ukkonen Suffix Tree implementation:
    /// http://programmerspatch.blogspot.com/2013/02/ukkonens-suffix-tree-algorithm.html
    /// https://github.com/schmidda/ukkonen-suffixtree
    /// </summary>
    public class SuffixTreeNodePosition
    {
        public SuffixTreeNodePosition(SuffixTreeNode node, int location)
        {
            Node = node;
            Location = location;
        }

        public SuffixTreeNode Node { get; set; }
        public int Location { get; set; }

        public bool AtEdgeEnd(int e)
        {
            return Location == Node.GetEnd(e);
        }

        public bool Continues(char c, int e)
        {
            if (Node.GetEnd(e) > Location)
            {
                char tempChar = Location + 1 < Node.Value.Length ? Node.Value[Location + 1] : (char)0;
                return tempChar == c;
            }
            else
            {
                return Node.FindChild(c) != null;
            }
        }
    }
}
