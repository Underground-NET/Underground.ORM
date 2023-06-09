namespace Underground.ORM.Core.Translator.Syntax.Token.Block
{
    public class EndBlockToken : MySqlSyntaxToken
    {
        public bool IsBlock { get => Is.Block; set => Is.Block = value; }

        public bool IsEndBlock { get => Is.EndBlock; set => Is.EndBlock = value; }

        public EndBlockToken(string token) :
            base(token)
        {
            IsBlock = true;
            IsEndBlock = true;
        }
    }
}
