using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Syntax.Token.Function;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Function
{
    [TestClass()]
    public class CharFunctionTokenTests
    {
        [TestMethod()]
        public void CharFunctionTokenTest()
        {
            var token = new CharFunctionToken("CHAR");

            Assert.IsTrue(token.IsFunction);
            Assert.IsTrue(token.IsCharFunction);
        }
    }
}