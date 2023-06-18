using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Syntax.Token.Statement;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Statement
{
    [TestClass()]
    public class FunctionStatementTokenTests
    {
        [TestMethod()]
        public void FunctionStatementTokenTest()
        {
            var token = new FunctionStatementToken();

            Assert.IsTrue(token.IsStatement);
            Assert.IsTrue(token.IsFunctionStatement);
        }
    }
}