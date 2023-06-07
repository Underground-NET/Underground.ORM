namespace Underground.ORM.Core.Translator.Syntax.Function
{
    public class CharFunctionToken : MySqlSyntaxToken
    {
        public override bool IsFunction { get; set; }

        public override bool IsCharFunction { get; set; }

        public CharFunctionToken(string token) :
            base(token)
        {
            IsFunction = true;
            IsCharFunction = true;
        }
    }
}
