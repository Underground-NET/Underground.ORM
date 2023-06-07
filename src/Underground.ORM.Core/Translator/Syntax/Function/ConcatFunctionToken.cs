namespace Underground.ORM.Core.Translator.Syntax.Function
{
    public class ConcatFunctionToken : MySqlSyntaxToken
    {
        public override bool IsFunction { get; set; }

        public override bool IsConcatFunction { get; set; }

        public ConcatFunctionToken(string token) :
            base(token)
        {
            IsFunction = true;
            IsConcatFunction = true;
        }
    }
}
