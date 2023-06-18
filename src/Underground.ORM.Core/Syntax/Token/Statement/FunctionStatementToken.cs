namespace Underground.ORM.Core.Syntax.Token.Statement
{
    public class FunctionStatementToken : SyntaxTokenBase
    {
        public bool IsStatement { get => Is.Statement; set => Is.Statement = value; }

        public bool IsFunctionStatement { get => Is.FunctionStatement; set => Is.FunctionStatement = value; }

        public FunctionStatementToken() : this("FUNCTION")
        {
        }

        public FunctionStatementToken(string token) :
            base(token)
        {
            IsStatement = true;
            IsFunctionStatement = true;
        }
    }
}
