using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Translator.Syntax.Token.Operator;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Operator
{
    [TestClass()]
    public class SemicolonTokenTests
    {
        [TestMethod()]
        public void SemicolonTokenTest()
        {
            var token = new SemicolonToken(";");

            Assert.IsTrue(token.IsSemicolon);
            Assert.IsTrue(token.IsOperator);
        }
    }
}