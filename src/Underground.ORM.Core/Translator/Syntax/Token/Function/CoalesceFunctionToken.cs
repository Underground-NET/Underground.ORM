namespace Underground.ORM.Core.Translator.Syntax.Token.Function
{
    public class CoalesceFunctionToken : MySqlSyntaxToken
    {
        public bool IsFunction { get => Is.Function; set => Is.Function = value; }

        public bool IsCoalesceFunction { get => Is.CoalesceFunction; set => Is.CoalesceFunction = value; }

        public CoalesceFunctionToken(string token) :
            base(token)
        {
            IsFunction = true;
            IsCoalesceFunction = true;
        }
    }
}
