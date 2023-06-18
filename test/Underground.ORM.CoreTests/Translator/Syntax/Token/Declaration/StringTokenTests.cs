using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Syntax.Token.Declaration;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Declaration
{
    [TestClass()]
    public class StringTokenTests
    {
        [TestMethod()]
        public void StringTokenTest()
        {
            var token = new StringToken("string");

            Assert.IsTrue(token.IsDbType);
            Assert.IsTrue(token.IsString);
            Assert.IsTrue(token.IsDeclaration);
        }
    }
}