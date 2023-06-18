namespace Underground.ORM.Core.Syntax.Token.Operator
{
    public class CloseParenthesisToken : SyntaxTokenBase
    {
        public bool IsOperator { get => Is.Operator; set => Is.Operator = value; }

        public bool IsParenthesis { get => Is.Parenthesis; set => Is.Parenthesis = value; }

        public bool IsCloseParenthesis { get => Is.CloseParenthesis; set => Is.CloseParenthesis = value; }

        public CloseParenthesisToken() : this(")")
        {
        }

        public CloseParenthesisToken(string token) :
            base(token)
        {
            IsOperator = true;
            IsParenthesis = true;
            IsCloseParenthesis = true;
        }
    }
}
