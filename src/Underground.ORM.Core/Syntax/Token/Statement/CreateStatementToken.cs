namespace Underground.ORM.Core.Syntax.Token.Statement
{
    public class CreateStatementToken : SyntaxTokenBase
    {
        public bool IsStatement { get => Is.Statement; set => Is.Statement = value; }

        public bool IsCreateStatement { get => Is.CreateStatement; set => Is.CreateStatement = value; }

        public CreateStatementToken() : this("CREATE")
        {
        }

        public CreateStatementToken(string token) :
            base(token)
        {
            IsStatement = true;
            IsCreateStatement = true;
        }
    }
}
