namespace Underground.ORM.Core.Syntax.Token.Operator
{
    public class OpenParenthesisToken : SyntaxTokenBase
    {
        public bool IsOperator { get => Is.Operator; set => Is.Operator = value; }

        public bool IsParenthesis { get => Is.Parenthesis; set => Is.Parenthesis = value; }

        public bool IsOpenParenthesis { get => Is.OpenParenthesis; set => Is.OpenParenthesis = value; }

        public OpenParenthesisToken() : this("(")
        {
        }

        public OpenParenthesisToken(string token) :
            base(token)
        {
            IsOperator = true;
            IsParenthesis = true;
            IsOpenParenthesis = true;
        }
    }
}
