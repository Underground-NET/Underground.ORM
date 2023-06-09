namespace Underground.ORM.Core.Translator.Syntax.Token.Statement
{
    public class ReturnsStatementToken : MySqlSyntaxToken
    {
        public bool IsStatement { get => Is.Statement; set => Is.Statement = value; }

        public bool IsReturnsStatement { get => Is.ReturnsStatement; set => Is.ReturnsStatement = value; }

        public ReturnsStatementToken(string token) :
            base(token)
        {
            IsStatement = true;
            IsReturnsStatement = true;
        }
    }
}
