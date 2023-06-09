namespace Underground.ORM.Core.Translator.Syntax.Token.Function
{
    public class CharFunctionToken : MySqlSyntaxToken
    {
        public bool IsFunction { get => Is.Function; set => Is.Function = value; }

        public bool IsCharFunction { get => Is.CharFunction; set => Is.CharFunction = value; }

        public CharFunctionToken(string token) :
            base(token)
        {
            IsFunction = true;
            IsCharFunction = true;
        }
    }
}
