using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Syntax.Token.Function;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Function
{
    [TestClass()]
    public class CastFunctionTokenTests
    {
        [TestMethod()]
        public void CastFunctionTokenTest()
        {
            var token = new CastFunctionToken();

            Assert.IsTrue(token.IsFunction);
            Assert.IsTrue(token.IsCastFunction);
        }
    }
}