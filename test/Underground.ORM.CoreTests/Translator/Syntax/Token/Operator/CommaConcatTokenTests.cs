using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Translator.Syntax.Token.Operator;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Operator
{
    [TestClass()]
    public class CommaConcatTokenTests
    {
        [TestMethod()]
        public void CommaConcatTokenTest()
        {
            var token = new CommaConcatToken(",");

            Assert.IsTrue(token.IsOperator);
            Assert.IsTrue(token.IsCommaConcat);
        }
    }
}