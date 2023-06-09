namespace Underground.ORM.Core.Translator.Syntax.Token.Function
{
    public class CastFunctionToken : MySqlSyntaxToken
    {
        public bool IsFunction { get => Is.Function; set => Is.Function = value; }

        public bool IsCastFunction { get => Is.CastFunction; set => Is.CastFunction = value; }

        public CastFunctionToken(string token) :
            base(token)
        {
            IsFunction = true;
            IsCastFunction = true;
        }
    }
}
