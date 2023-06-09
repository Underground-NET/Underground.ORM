using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Translator.Syntax.Token.Statement;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Statement
{
    [TestClass()]
    public class NameFunctionStatementTokenTests
    {
        [TestMethod()]
        public void NameFunctionStatementTokenTest()
        {
            var token = new NameFunctionStatementToken("`functionName`");

            Assert.IsTrue(token.IsStatement);
            Assert.IsTrue(token.IsNameFunctionStatement);
        }
    }
}