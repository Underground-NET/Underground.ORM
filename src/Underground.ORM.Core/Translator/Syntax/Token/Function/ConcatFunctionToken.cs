namespace Underground.ORM.Core.Translator.Syntax.Token.Function
{
    public class ConcatFunctionToken : MySqlSyntaxToken
    {
        public bool IsFunction { get => Is.Function; set => Is.Function = value; }

        public bool IsConcatFunction { get => Is.ConcatFunction; set => Is.ConcatFunction = value; }

        public ConcatFunctionToken(string token) :
            base(token)
        {
            IsFunction = true;
            IsConcatFunction = true;
        }
    }
}
