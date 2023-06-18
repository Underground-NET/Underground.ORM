namespace Underground.ORM.Core.Syntax.Token.Operator
{
    public class SemicolonToken : SyntaxTokenBase
    {
        public bool IsOperator { get => Is.Operator; set => Is.Operator = value; }

        public bool IsSemicolon { get => Is.Semicolon; set => Is.Semicolon = value; }

        public SemicolonToken() : this(";")
        {
        }

        public SemicolonToken(string token) :
            base(token)
        {
            IsOperator = true;
            IsSemicolon = true;
        }
    }
}
