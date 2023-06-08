using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Underground.ORM.Core.Translator.Syntax.Block.Tests
{
    [TestClass()]
    public class EndBlockTokenTests
    {
        [TestMethod()]
        public void EndBlockTokenTest()
        {
            var token = new EndBlockToken("BEGIN");

            Assert.IsTrue(token.IsBlock);
            Assert.IsTrue(token.IsEndBlock);
        }
    }
}