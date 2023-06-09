namespace Underground.ORM.Core.Translator.Syntax.Token.Operator
{
    public class CloseParenthesisToken : MySqlSyntaxToken
    {
        public bool IsOperator { get => Is.Operator; set => Is.Operator = value; }

        public bool IsParenthesis { get => Is.Parenthesis; set => Is.Parenthesis = value; }

        public bool IsCloseParenthesis { get => Is.CloseParenthesis; set => Is.CloseParenthesis = value; }

        public CloseParenthesisToken(string token) :
            base(token)
        {
            IsOperator = true;
            IsParenthesis = true;
            IsCloseParenthesis = true;
        }
    }
}
