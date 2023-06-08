using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Underground.ORM.Core.Translator.Syntax.Block.Tests
{
    [TestClass()]
    public class BeginBlockTokenTests
    {
        [TestMethod()]
        public void BeginBlockTokenTest()
        {
            var token = new BeginBlockToken("BEGIN");

            Assert.IsTrue(token.IsBlock);
            Assert.IsTrue(token.IsBeginBlock);
        }
    }
}