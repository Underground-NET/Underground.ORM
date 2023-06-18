using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Syntax.Token.Operator;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Operator
{
    [TestClass()]
    public class AttributionTokenTests
    {
        [TestMethod()]
        public void AttributionTokenTest()
        {
            var token = new AttributionToken();

            Assert.IsTrue(token.IsOperator);
            Assert.IsTrue(token.IsAttribution);
        }
    }
}