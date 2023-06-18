using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Syntax.Token.Block;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Block
{
    [TestClass()]
    public class EndBlockTokenTests
    {
        [TestMethod()]
        public void EndBlockTokenTest()
        {
            var token = new EndBlockToken();

            Assert.IsTrue(token.IsBlock);
            Assert.IsTrue(token.IsEndBlock);
        }
    }
}