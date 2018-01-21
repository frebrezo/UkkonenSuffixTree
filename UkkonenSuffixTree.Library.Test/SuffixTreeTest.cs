using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UkkonenSuffixTree.Library.Test
{
    [TestClass]
    public class SuffixTreeTest
    {
        [TestMethod]
        public void BuildSuffixTree_Simple_Test()
        {
            string s = "abc";
            SuffixTreeBuilder stb = new SuffixTreeBuilder();
            SuffixTree suffixTree = stb.BuildSuffixTree(s);
            Assert.AreEqual(4, suffixTree.RootNode.GetNumChildren());
            List<SuffixTreeNode> nodes = suffixTree.RootNode.GetChildren().ToList();
            string substring = null;
            substring = "abc$";
            Assert.AreEqual(substring, nodes.FirstOrDefault((n) => n.ValueDerived == substring).ValueDerived);
            substring = "bc$";
            Assert.AreEqual(substring, nodes.FirstOrDefault((n) => n.ValueDerived == substring).ValueDerived);
            substring = "c$";
            Assert.AreEqual(substring, nodes.FirstOrDefault((n) => n.ValueDerived == substring).ValueDerived);
        }

        [TestMethod]
        public void BuildSuffixTree_Banana_Test()
        {
            string s = "banana";
            SuffixTreeBuilder stb = new SuffixTreeBuilder();
            SuffixTree suffixTree = stb.BuildSuffixTree(s);
            Assert.AreEqual(4, suffixTree.RootNode.GetNumChildren());
            List<SuffixTreeNode> nodes = suffixTree.RootNode.GetChildren().ToList();
            string substring = null;
            substring = "banana$";
            Assert.AreEqual(substring, nodes.FirstOrDefault((n) => n.ValueDerived == substring).ValueDerived);
            substring = "a";
            Assert.AreEqual(substring, nodes.FirstOrDefault((n) => n.ValueDerived == substring).ValueDerived);
            substring = "na";
            Assert.AreEqual(substring, nodes.FirstOrDefault((n) => n.ValueDerived == substring).ValueDerived);
            substring = "na$";
            Assert.AreEqual(substring, nodes.FirstOrDefault((n) => n.ValueDerived == substring).ValueDerived);
        }
    }
}
