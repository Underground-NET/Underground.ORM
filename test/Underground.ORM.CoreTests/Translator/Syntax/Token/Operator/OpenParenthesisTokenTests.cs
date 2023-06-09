using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Translator.Syntax.Token.Operator;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Operator
{
    [TestClass()]
    public class OpenParenthesisTokenTests
    {
        [TestMethod()]
        public void OpenParenthesisTokenTest()
        {
            var token = new OpenParenthesisToken("(");

            Assert.IsTrue(token.IsOperator);
            Assert.IsTrue(token.IsParenthesis);
            Assert.IsTrue(token.IsOpenParenthesis);
        }
    }
}