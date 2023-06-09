namespace Underground.ORM.Core.Translator.Syntax.Token.Statement
{
    public class CreateStatementToken : MySqlSyntaxToken
    {
        public bool IsStatement { get => Is.Statement; set => Is.Statement = value; }

        public bool IsCreateStatement { get => Is.CreateStatement; set => Is.CreateStatement = value; }

        public CreateStatementToken(string token) :
            base(token)
        {
            IsStatement = true;
            IsCreateStatement = true;
        }
    }
}
