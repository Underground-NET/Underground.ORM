using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Translator.Syntax.Token.Declaration;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Declaration
{
    [TestClass()]
    public class DefaultTokenTests
    {
        [TestMethod()]
        public void DefaultTokenTest()
        {
            var token = new DefaultToken("DEFAULT");

            Assert.IsTrue(token.IsDefault);
            Assert.IsTrue(token.IsDeclaration);
        }
    }
}