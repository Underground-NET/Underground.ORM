using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Underground.ORM.Core.Translator.Syntax.Reference.Tests
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