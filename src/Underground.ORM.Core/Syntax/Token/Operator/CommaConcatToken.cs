namespace Underground.ORM.Core.Syntax.Token.Operator
{
    public class CommaConcatToken : SyntaxTokenBase
    {
        public bool IsOperator { get => Is.Operator; set => Is.Operator = value; }

        public bool IsCommaConcat { get => Is.CommaConcat; set => Is.CommaConcat = value; }
        
        public CommaConcatToken() : this(",")
        {
        }

        public CommaConcatToken(string token) :
            base(token)
        {
            IsOperator = true;
            IsCommaConcat = true;
        }
    }
}
