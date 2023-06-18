namespace Underground.ORM.Core.Syntax.Token.Function
{
    public class CastFunctionToken : SyntaxTokenBase
    {
        public bool IsFunction { get => Is.Function; set => Is.Function = value; }

        public bool IsCastFunction { get => Is.CastFunction; set => Is.CastFunction = value; }

        public CastFunctionToken() : this("CAST")
        {
        }

        public CastFunctionToken(string token) :
            base(token)
        {
            IsFunction = true;
            IsCastFunction = true;
        }
    }
}
