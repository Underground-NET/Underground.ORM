using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Underground.ORM.Core.Translator.Syntax.Statement.Tests
{
    [TestClass()]
    public class ReturnStatementTokenTests
    {
        [TestMethod()]
        public void ReturnStatementTokenTest()
        {
            var token = new ReturnStatementToken("RETURN");

            Assert.IsTrue(token.IsStatement);
            Assert.IsTrue(token.IsReturnStatement);
        }
    }
}