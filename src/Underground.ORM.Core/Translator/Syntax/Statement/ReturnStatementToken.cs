namespace Underground.ORM.Core.Translator.Syntax.Statement
{
    public class ReturnStatementToken : MySqlSyntaxToken
    {
        public override bool IsStatement { get; set; }

        public override bool IsReturnStatement { get; set; }

        public ReturnStatementToken(string token) :
            base(token)
        {
            IsStatement = true;
            IsReturnStatement = true;
        }
    }
}
