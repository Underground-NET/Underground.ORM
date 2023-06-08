using Underground.ORM.Core.Translator.Syntax.Declaration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace Underground.ORM.Core.Translator.Syntax.Declaration.Tests
{
    [TestClass()]
    public class VariableTokenTests
    {
        [TestMethod()]
        public void VariableTokenTest()
        {
            var token = new VariableToken("`var1`");

            Assert.IsTrue(token.IsVar);
            Assert.IsTrue(token.IsDbType);
            Assert.IsFalse(token.IsString);
            Assert.IsTrue(token.IsDeclaration);
        }

        [TestMethod()]
        public void StringVariableTokenTest()
        {
            var token = new VariableToken("`var1`", DbType.String);

            Assert.IsTrue(token.IsVar);
            Assert.IsTrue(token.IsDbType);
            Assert.IsTrue(token.IsString);
            Assert.IsTrue(token.IsDeclaration);
        }
    }
}