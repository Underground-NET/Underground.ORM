namespace Underground.ORM.Core.Translator.Syntax.Token.Statement
{
    public class ReturnStatementToken : MySqlSyntaxToken
    {
        public bool IsStatement { get => Is.Statement; set => Is.Statement = value; }

        public bool IsReturnStatement { get => Is.ReturnStatement; set => Is.ReturnStatement = value; }

        public ReturnStatementToken(string token) :
            base(token)
        {
            IsStatement = true;
            IsReturnStatement = true;
        }
    }
}
