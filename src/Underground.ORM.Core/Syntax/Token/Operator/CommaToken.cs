namespace Underground.ORM.Core.Syntax.Token.Operator
{
    public class CommaToken  : SyntaxTokenBase
    {
        public bool IsOperator { get => Is.Operator; set => Is.Operator = value; }

        public bool IsComma { get => Is.Comma; set => Is.Comma = value; }

        public CommaToken() : this(",")
        {
        }

        public CommaToken(string token) :
            base(token)
        {
            IsOperator = true;
            IsComma = true;
        }
    }
}
