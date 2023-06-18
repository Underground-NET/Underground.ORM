using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Syntax.Token.Operator;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Operator
{
    [TestClass()]
    public class CloseParenthesisTokenTests
    {
        [TestMethod()]
        public void CloseParenthesisTokenTest()
        {
            var token = new CloseParenthesisToken();

            Assert.IsTrue(token.IsOperator);
            Assert.IsTrue(token.IsParenthesis);
            Assert.IsTrue(token.IsCloseParenthesis);
        }
    }
}