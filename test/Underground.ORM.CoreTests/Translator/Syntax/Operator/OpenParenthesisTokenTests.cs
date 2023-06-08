using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Underground.ORM.Core.Translator.Syntax.Operator.Tests
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