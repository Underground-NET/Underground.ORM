using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Syntax.Token.Marker;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Marker
{
    [TestClass()]
    public class CommentTokenTests
    {
        [TestMethod()]
        public void CommentTokenTest()
        {
            var token = new CommentLineToken();

            Assert.IsTrue(token.IsMarker);
            Assert.IsTrue(token.IsComment);
        }
    }
}