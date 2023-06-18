namespace Underground.ORM.Core.Syntax.Token.Function
{
    public class CharFunctionToken : SyntaxTokenBase
    {
        public bool IsFunction { get => Is.Function; set => Is.Function = value; }

        public bool IsCharFunction { get => Is.CharFunction; set => Is.CharFunction = value; }

        public CharFunctionToken() : this("CHAR")
        {
        }

        public CharFunctionToken(string token) :
            base(token)
        {
            IsFunction = true;
            IsCharFunction = true;
        }
    }
}
