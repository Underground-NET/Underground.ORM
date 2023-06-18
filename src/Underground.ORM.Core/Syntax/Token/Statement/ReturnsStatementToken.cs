namespace Underground.ORM.Core.Syntax.Token.Statement
{
    public class ReturnsStatementToken : SyntaxTokenBase
    {
        public bool IsStatement { get => Is.Statement; set => Is.Statement = value; }

        public bool IsReturnsStatement { get => Is.ReturnsStatement; set => Is.ReturnsStatement = value; }

        public ReturnsStatementToken() : this("RETURNS")
        {
        }

        public ReturnsStatementToken(string token) :
            base(token)
        {
            IsStatement = true;
            IsReturnsStatement = true;
        }
    }
}
