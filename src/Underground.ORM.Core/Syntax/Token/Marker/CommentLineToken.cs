namespace Underground.ORM.Core.Syntax.Token.Marker
{
    public class CommentLineToken : SyntaxTokenBase
    {
        public bool IsMarker { get => Is.Marker; set => Is.Marker = value; }

        public bool IsComment { get => Is.Comment; set => Is.Comment = value; }

        public CommentLineToken() : this("#")
        {
        }

        public CommentLineToken(string token) :
            base(token)
        {
            IsMarker = true;
            IsComment = true;
        }
    }
}
