using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Underground.ORM.Core.Translator.Syntax.Function.Tests
{
    [TestClass()]
    public class ConcatFunctionTokenTests
    {
        [TestMethod()]
        public void ConcatFunctionTokenTest()
        {
            var token = new ConcatFunctionToken("CONCAT");

            Assert.IsTrue(token.IsFunction);
            Assert.IsTrue(token.IsConcatFunction);
        }
    }
}