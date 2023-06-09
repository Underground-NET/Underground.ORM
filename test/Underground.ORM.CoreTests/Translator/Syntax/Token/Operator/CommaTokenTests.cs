using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Translator.Syntax.Token.Operator;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Operator
{
    [TestClass()]
    public class CommaTokenTests
    {
        [TestMethod()]
        public void CommaTokenTest()
        {
            var token = new CommaToken(",");

            Assert.IsTrue(token.IsComma);
            Assert.IsTrue(token.IsOperator);
        }
    }
}