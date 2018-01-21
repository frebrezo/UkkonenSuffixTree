using System;
using UkkonenSuffixTree.Library;

namespace UkkonenSuffixTree
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("UkkonenSuffixTree \"<string>\"");
                Environment.Exit(1);
            }
            string s = args[0];
            SuffixTreeBuilder stb = new SuffixTreeBuilder();
            SuffixTree suffixTree = stb.BuildSuffixTree(s);
            suffixTree.PrintTree();
        }
    }
}
