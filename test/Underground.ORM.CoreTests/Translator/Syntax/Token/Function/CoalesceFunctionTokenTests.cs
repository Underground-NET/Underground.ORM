using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Syntax.Token.Function;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Function
{
    [TestClass()]
    public class CoalesceFunctionTokenTests
    {
        [TestMethod()]
        public void CoalesceFunctionTokenTest()
        {
            var token = new CoalesceFunctionToken();

            Assert.IsTrue(token.IsFunction);
            Assert.IsTrue(token.IsCoalesceFunction);
        }
    }
}