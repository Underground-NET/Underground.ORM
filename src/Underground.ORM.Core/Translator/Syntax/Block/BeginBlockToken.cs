namespace Underground.ORM.Core.Translator.Syntax.Block
{
    public class BeginBlockToken : MySqlSyntaxToken
    {
        public override bool IsBlock { get; set; }

        public override bool IsBeginBlock { get; set; }

        public BeginBlockToken(string token) :
            base(token)
        {
            IsBlock = true;
            IsBeginBlock = true;
        }
    }
}
