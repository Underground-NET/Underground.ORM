using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Translator.Syntax.Token.Statement;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Statement
{
    [TestClass()]
    public class CreateStatementTokenTests
    {
        [TestMethod()]
        public void CreateStatementTokenTest()
        {
            var token = new CreateStatementToken("CREATE");

            Assert.IsTrue(token.IsStatement);
            Assert.IsTrue(token.IsCreateStatement);
        }
    }
}