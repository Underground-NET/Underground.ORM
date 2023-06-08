using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Underground.ORM.Core.Translator.Syntax.Function.Tests
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