using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Syntax.Token.Statement;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Statement
{
    [TestClass()]
    public class ReturnStatementTokenTests
    {
        [TestMethod()]
        public void ReturnStatementTokenTest()
        {
            var token = new ReturnStatementToken();

            Assert.IsTrue(token.IsStatement);
            Assert.IsTrue(token.IsReturnStatement);
        }
    }
}