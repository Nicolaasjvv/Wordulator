using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WordulatorCore.Utilities;

namespace WordulatorTests
{
    [TestClass]
    public class UnitTest1
    {
        protected string _testString = "this is a sentence with with some some some double some words and, some commas. Also this sentence,. contains .! wierd, punctuation";

        [TestMethod]
        public void Test1()
        {
             var result = Utils.GetUniqueWords(_testString);
             CollectionAssert.AllItemsAreUnique(result);
        }

        [TestMethod]
        public void TestGettingOnlyWords()
        {
            var result = Utils.GetWordsFromString(_testString);

            //var wordCounts = from word in result
            //                 group word by word into g
            //                 select new { g.Key, Count = g.Count() };


            Assert.AreEqual(result.Count,21);
        }
    }
}
