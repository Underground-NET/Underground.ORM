namespace Underground.ORM.Core.Translator.Syntax.Token.Block
{
    public class BeginBlockToken : MySqlSyntaxToken
    {
        public bool IsBlock { get => Is.Block; set => Is.Block = value; }

        public bool IsBeginBlock { get => Is.BeginBlock; set => Is.BeginBlock = value; }

        public BeginBlockToken(string token) :
            base(token)
        {
            IsBlock = true;
            IsBeginBlock = true;
        }
    }
}
