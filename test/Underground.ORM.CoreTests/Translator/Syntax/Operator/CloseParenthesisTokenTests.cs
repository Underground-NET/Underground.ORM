using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Underground.ORM.Core.Translator.Syntax.Operator.Tests
{
    [TestClass()]
    public class CloseParenthesisTokenTests
    {
        [TestMethod()]
        public void CloseParenthesisTokenTest()
        {
            var token = new CloseParenthesisToken(")");

            Assert.IsTrue(token.IsOperator);
            Assert.IsTrue(token.IsParenthesis);
            Assert.IsTrue(token.IsCloseParenthesis);
        }
    }
}