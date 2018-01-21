using System;

namespace UkkonenSuffixTree.Library
{
    /// <summary>
    /// Direct port of below Ukkonen Suffix Tree implementation:
    /// http://programmerspatch.blogspot.com.au/2013/02/ukkonens-suffix-tree-algorithm.html
    /// https://github.com/schmidda/ukkonen-suffixtree
    /// </summary>
    public class SuffixTreePath
    {
        public int Start { get; set; }
        public int Length { get; set; }

        public SuffixTreePath(int start, int length)
        {
            Start = start;
            Length = length;
        }

        public void Prepend(int len)
        {
            Start -= len;
            Length += len;
        }
    }
}
