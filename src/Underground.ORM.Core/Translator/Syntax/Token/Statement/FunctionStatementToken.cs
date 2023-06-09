namespace Underground.ORM.Core.Translator.Syntax.Token.Statement
{
    public class FunctionStatementToken : MySqlSyntaxToken
    {
        public bool IsStatement { get => Is.Statement; set => Is.Statement = value; }

        public bool IsFunctionStatement { get => Is.FunctionStatement; set => Is.FunctionStatement = value; }

        public FunctionStatementToken(string token) :
            base(token)
        {
            IsStatement = true;
            IsFunctionStatement = true;
        }
    }
}
