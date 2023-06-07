namespace Underground.ORM.Core.Translator.Syntax.Reference
{
    public class VariableReferenceToken : MySqlSyntaxToken
    {
        public override bool IsReference { get; set; }

        public override bool IsVarRef { get; set; }

        public VariableReferenceToken(string token) :
            base(token)
        {
            IsVarRef = true;
            IsReference = true;
        }
    }
}
