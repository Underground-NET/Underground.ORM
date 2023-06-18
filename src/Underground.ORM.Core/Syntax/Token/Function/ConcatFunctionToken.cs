namespace Underground.ORM.Core.Syntax.Token.Function
{
    public class ConcatFunctionToken : SyntaxTokenBase
    {
        public bool IsFunction { get => Is.Function; set => Is.Function = value; }

        public bool IsConcatFunction { get => Is.ConcatFunction; set => Is.ConcatFunction = value; }

        public ConcatFunctionToken() : this("CONCAT")
        {
        }

        public ConcatFunctionToken(string token) :
            base(token)
        {
            IsFunction = true;
            IsConcatFunction = true;
        }
    }
}
