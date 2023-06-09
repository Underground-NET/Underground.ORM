namespace Underground.ORM.Core.Translator.Syntax.Token.Operator
{
    public class AttributionToken : MySqlSyntaxToken
    {
        public bool IsOperator { get => Is.Operator; set => Is.Operator = value; }

        public bool IsAttribution { get => Is.Attribution; set => Is.Attribution = value; }

        public AttributionToken(string token) :
            base(token)
        {
            IsOperator = true;
            IsAttribution = true;
        }
    }
}
