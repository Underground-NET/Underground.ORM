using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Syntax.Token.Block;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Block
{
    [TestClass()]
    public class BeginBlockTokenTests
    {
        [TestMethod()]
        public void BeginBlockTokenTest()
        {
            var token = new BeginBlockToken();

            Assert.IsTrue(token.IsBlock);
            Assert.IsTrue(token.IsBeginBlock);
        }
    }
}