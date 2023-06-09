using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Translator.Syntax.Token.Function;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Function
{
    [TestClass()]
    public class ConcatFunctionTokenTests
    {
        [TestMethod()]
        public void ConcatFunctionTokenTest()
        {
            var token = new ConcatFunctionToken("CONCAT");

            Assert.IsTrue(token.IsFunction);
            Assert.IsTrue(token.IsConcatFunction);
        }
    }
}