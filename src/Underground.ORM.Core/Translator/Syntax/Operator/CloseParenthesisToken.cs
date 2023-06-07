namespace Underground.ORM.Core.Translator.Syntax.Operator
{
    public class CloseParenthesisToken : MySqlSyntaxToken
    {
        public override bool IsOperator { get; set; }

        public override bool IsParenthesis { get; set; }

        public override bool IsCloseParenthesis { get; set; }

        public CloseParenthesisToken(string token) :
            base(token)
        {
            IsOperator = true;
            IsParenthesis = true;
            IsCloseParenthesis = true;
        }
    }
}
