using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Underground.ORM.Core.Translator.Syntax.Function.Tests
{
    [TestClass()]
    public class CoalesceFunctionTokenTests
    {
        [TestMethod()]
        public void CoalesceFunctionTokenTest()
        {
            var token = new CoalesceFunctionToken("COALESCE");

            Assert.IsTrue(token.IsFunction);
            Assert.IsTrue(token.IsCoalesceFunction);
        }
    }
}