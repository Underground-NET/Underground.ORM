using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Translator.Syntax.Token.Statement;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Statement
{
    [TestClass()]
    public class ReturnsStatementTokenTests
    {
        [TestMethod()]
        public void ReturnsStatementTokenTest()
        {
            var token = new ReturnsStatementToken("RETURNS");

            Assert.IsTrue(token.IsStatement);
            Assert.IsTrue(token.IsReturnsStatement);
        }
    }
}