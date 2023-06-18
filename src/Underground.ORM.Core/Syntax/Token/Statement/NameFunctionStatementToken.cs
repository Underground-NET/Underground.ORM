namespace Underground.ORM.Core.Syntax.Token.Statement
{
    public class NameFunctionStatementToken : SyntaxTokenBase
    {
        public bool IsStatement { get => Is.Statement; set => Is.Statement = value; }

        public bool IsNameFunctionStatement { get => Is.NameFunctionStatement; set => Is.NameFunctionStatement = value; }

        public NameFunctionStatementToken(string token) :
            base(token)
        {
            IsStatement = true;
            IsNameFunctionStatement = true;
        }
    }
}
