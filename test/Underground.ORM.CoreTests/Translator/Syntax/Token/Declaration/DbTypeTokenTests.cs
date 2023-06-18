using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using Underground.ORM.Core.Syntax.Token.Declaration;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Declaration
{
    [TestClass()]
    public class DbTypeTokenTests
    {
        [TestMethod()]
        public void DbTypeTokenTest()
        {
            var token = new DbTypeToken("VARCHAR", DbType.String);

            Assert.IsTrue(token.IsDbType);
            Assert.IsTrue(token.IsString);
            Assert.IsTrue(token.IsDeclaration);
        }
    }
}