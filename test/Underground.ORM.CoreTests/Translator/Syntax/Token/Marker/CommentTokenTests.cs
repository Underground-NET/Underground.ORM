using Microsoft.VisualStudio.TestTools.UnitTesting;
using Underground.ORM.Core.Translator.Syntax.Token.Marker;

namespace Underground.ORM.CoreTests.Translator.Syntax.Token.Marker
{
    [TestClass()]
    public class CommentTokenTests
    {
        [TestMethod()]
        public void CommentTokenTest()
        {
            var token = new CommentToken("# Comment");

            Assert.IsTrue(token.IsMarker);
            Assert.IsTrue(token.IsComment);
        }
    }
}