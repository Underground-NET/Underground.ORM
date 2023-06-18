using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Syntax.Token.Statement;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Statement
{
    [TestClass()]
    public class ReturnsStatementTokenTests
    {
        [TestMethod()]
        public void ReturnsStatementTokenTest()
        {
            var token = new ReturnsStatementToken();

            Assert.IsTrue(token.IsStatement);
            Assert.IsTrue(token.IsReturnsStatement);
        }
    }
}