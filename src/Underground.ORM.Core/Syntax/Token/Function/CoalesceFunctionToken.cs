namespace Underground.ORM.Core.Syntax.Token.Function
{
    public class CoalesceFunctionToken : SyntaxTokenBase
    {
        public bool IsFunction { get => Is.Function; set => Is.Function = value; }

        public bool IsCoalesceFunction { get => Is.CoalesceFunction; set => Is.CoalesceFunction = value; }

        public CoalesceFunctionToken() : this("COALESCE")
        {
        }

        public CoalesceFunctionToken(string token) :
            base(token)
        {
            IsFunction = true;
            IsCoalesceFunction = true;
        }
    }
}
