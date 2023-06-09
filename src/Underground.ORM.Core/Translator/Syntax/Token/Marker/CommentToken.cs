namespace Underground.ORM.Core.Translator.Syntax.Token.Marker
{
    public class CommentToken : MySqlSyntaxToken
    {
        public bool IsMarker { get => Is.Marker; set => Is.Marker = value; }

        public bool IsComment { get => Is.Comment; set => Is.Comment = value; }

        public CommentToken(string token) :
            base(token)
        {
            IsMarker = true;
            IsComment = true;
        }
    }
}
