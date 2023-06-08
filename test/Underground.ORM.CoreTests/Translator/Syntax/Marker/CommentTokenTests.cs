using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Underground.ORM.Core.Translator.Syntax.Marker.Tests
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