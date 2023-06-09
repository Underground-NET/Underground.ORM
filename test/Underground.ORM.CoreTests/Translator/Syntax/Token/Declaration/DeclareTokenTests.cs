using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Translator.Syntax.Token.Declaration;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Declaration
{
    [TestClass()]
    public class DeclareTokenTests
    {
        [TestMethod()]
        public void DeclareTokenTest()
        {
            var token = new DeclareToken("DECLARE");

            Assert.IsTrue(token.IsDeclare);
            Assert.IsTrue(token.IsDeclaration);
        }
    }
}