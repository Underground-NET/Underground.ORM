namespace Underground.ORM.Core.Translator.Syntax.Operator
{
    public class AttributionToken : MySqlSyntaxToken
    {
        public override bool IsOperator { get; set; }

        public override bool IsAttribution { get; set; }

        public AttributionToken(string token) :
            base(token)
        {
            IsOperator = true;
            IsAttribution = true;
        }
    }
}
