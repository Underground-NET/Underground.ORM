using Underground.ORM.Core.Syntax.Token.Declaration;

namespace Underground.ORM.Core.Syntax.Token.Reference
{
    public class VariableReferenceToken : SyntaxTokenBase
    {
        public bool IsReference { get => Is.Reference; set => Is.Reference = value; }

        public bool IsVarRef { get => Is.VarRef; set => Is.VarRef = value; }

        public VariableReferenceToken(VariableToken variable) 
            : this(variable.Token)
        {
            Reference = variable;
        }

        public VariableReferenceToken(string token) :
            base(token)
        {
            IsVarRef = true;
            IsReference = true;
        }
    }
}
