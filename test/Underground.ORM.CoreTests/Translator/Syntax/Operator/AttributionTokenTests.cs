using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Underground.ORM.Core.Translator.Syntax.Operator.Tests
{
    [TestClass()]
    public class AttributionTokenTests
    {
        [TestMethod()]
        public void AttributionTokenTest()
        {
            var token = new AttributionToken("=");

            Assert.IsTrue(token.IsOperator);
            Assert.IsTrue(token.IsAttribution);
        }
    }
}