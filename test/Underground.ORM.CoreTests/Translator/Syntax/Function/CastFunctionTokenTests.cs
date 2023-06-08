using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Underground.ORM.Core.Translator.Syntax.Function.Tests
{
    [TestClass()]
    public class CastFunctionTokenTests
    {
        [TestMethod()]
        public void CastFunctionTokenTest()
        {
            var token = new CastFunctionToken("CAST");

            Assert.IsTrue(token.IsFunction);
            Assert.IsTrue(token.IsCastFunction);
        }
    }
}