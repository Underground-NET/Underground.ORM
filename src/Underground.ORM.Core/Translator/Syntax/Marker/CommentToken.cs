namespace Underground.ORM.Core.Translator.Syntax.Marker
{
    public class CommentToken : MySqlSyntaxToken
    {
        public override bool IsMarker { get; set; }

        public override bool IsComment { get; set; }

        public CommentToken(string token) :
            base(token)
        {
            IsMarker = true;
            IsComment = true;
        }
    }
}
