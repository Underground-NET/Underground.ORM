using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Underground.ORM.Core.Translator.Syntax.Declaration.Tests
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