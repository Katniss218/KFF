using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KFF.Tests
{
    [TestClass]
    public class KFFParserTests
    {
        KFFParser parser = new KFFParser();

        [TestMethod]
        public void TestMethod1()
        {
            KFFFile file = parser.Parse( "test", "TagName = 50;" );

            Assert.IsTrue( file.tags.count == 1 );
        }


        [TestMethod]
        public void TestParsing_Name()
        {
            string s = "name";
            int pos = 0;

            KFFParser.Name( "", ref s, ref pos );

        }
    }
}
