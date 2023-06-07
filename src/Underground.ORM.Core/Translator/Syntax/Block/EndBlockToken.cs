namespace Underground.ORM.Core.Translator.Syntax.Function
{
    public class EndBlockToken : MySqlSyntaxToken
    {
        public override bool IsBlock { get; set; }

        public override bool IsEndBlock { get; set; }

        public EndBlockToken(string token) :
            base(token)
        {
            IsBlock = true;
            IsEndBlock = true;
        }
    }
}
