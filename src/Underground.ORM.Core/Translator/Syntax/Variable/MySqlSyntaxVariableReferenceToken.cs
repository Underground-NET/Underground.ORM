namespace Underground.ORM.Core.Translator.Syntax.Variable
{
    public class MySqlSyntaxVariableReferenceToken : MySqlSyntaxToken
    {
        public override bool IsVarRef { get; set; }

        public MySqlSyntaxVariableReferenceToken(string token) :
            base(token)
        {
            IsVarRef = true;
        }
    }
}
