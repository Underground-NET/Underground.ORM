using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace Underground.ORM.Core.Translator.Syntax.Declaration.Tests
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