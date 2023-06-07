namespace Underground.ORM.Core.Translator.Syntax.Operator
{
    public class OpenParenthesisToken : MySqlSyntaxToken
    {
        public override bool IsOperator { get; set; }

        public override bool IsParenthesis { get; set; }

        public override bool IsOpenParenthesis { get; set; }

        public OpenParenthesisToken(string token) :
            base(token)
        {
            IsOperator = true;
            IsParenthesis = true;
            IsOpenParenthesis = true;
        }
    }
}
