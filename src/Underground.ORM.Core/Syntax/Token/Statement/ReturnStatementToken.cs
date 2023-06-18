namespace Underground.ORM.Core.Syntax.Token.Statement
{
    public class ReturnStatementToken : SyntaxTokenBase
    {
        public bool IsStatement { get => Is.Statement; set => Is.Statement = value; }

        public bool IsReturnStatement { get => Is.ReturnStatement; set => Is.ReturnStatement = value; }

        public ReturnStatementToken() : this("RETURN")
        {
        }

        public ReturnStatementToken(string token) :
            base(token)
        {
            IsStatement = true;
            IsReturnStatement = true;
        }
    }
}
