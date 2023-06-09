namespace Underground.ORM.Core.Translator.Syntax.Token.Operator
{
    public class SemicolonToken : MySqlSyntaxToken
    {
        public bool IsOperator { get => Is.Operator; set => Is.Operator = value; }

        public bool IsSemicolon { get => Is.Semicolon; set => Is.Semicolon = value; }

        public SemicolonToken(string token) :
            base(token)
        {
            IsOperator = true;
            IsSemicolon = true;
        }
    }
}
