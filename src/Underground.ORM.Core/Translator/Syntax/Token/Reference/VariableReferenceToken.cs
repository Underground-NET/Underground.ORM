namespace Underground.ORM.Core.Translator.Syntax.Token.Reference
{
    public class VariableReferenceToken : MySqlSyntaxToken
    {
        public bool IsReference { get => Is.Reference; set => Is.Reference = value; }

        public bool IsVarRef { get => Is.VarRef; set => Is.VarRef = value; }

        public VariableReferenceToken(string token) :
            base(token)
        {
            IsVarRef = true;
            IsReference = true;
        }
    }
}
