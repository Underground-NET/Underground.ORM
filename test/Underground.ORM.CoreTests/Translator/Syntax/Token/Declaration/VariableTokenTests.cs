using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using Underground.ORM.Core.Translator.Syntax.Token.Declaration;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Declaration
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