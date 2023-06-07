namespace Underground.ORM.Core.Translator.Syntax.Function
{
    public class CastFunctionToken : MySqlSyntaxToken
    {
        public override bool IsFunction { get; set; }

        public override bool IsCastFunction { get; set; }

        public CastFunctionToken(string token) :
            base(token)
        {
            IsFunction = true;
            IsCastFunction = true;
        }
    }
}
