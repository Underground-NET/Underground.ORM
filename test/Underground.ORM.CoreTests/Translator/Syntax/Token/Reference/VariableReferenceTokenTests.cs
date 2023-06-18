using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Syntax.Token.Reference;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Reference
{
    [TestClass()]
    public class VariableReferenceTokenTests
    {
        [TestMethod()]
        public void VariableReferenceTokenTest()
        {
            var token = new VariableReferenceToken("`var1`");

            Assert.IsTrue(token.IsReference);
            Assert.IsTrue(token.IsVarRef);
        }
    }
}