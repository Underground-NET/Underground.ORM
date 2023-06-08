using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Underground.ORM.Core.Translator.Syntax.Operator.Tests
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