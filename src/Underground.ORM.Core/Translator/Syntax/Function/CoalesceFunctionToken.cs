namespace Underground.ORM.Core.Translator.Syntax.Function
{
    public class CoalesceFunctionToken : MySqlSyntaxToken
    {
        public override bool IsFunction { get; set; }

        public override bool IsCoalesceFunction { get; set; }

        public CoalesceFunctionToken(string token) :
            base(token)
        {
            IsFunction = true;
            IsCoalesceFunction = true;
        }
    }
}
