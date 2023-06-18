namespace Underground.ORM.Core.Syntax.Token.Operator
{
    public class AttributionToken : SyntaxTokenBase
    {
        public bool IsOperator { get => Is.Operator; set => Is.Operator = value; }

        public bool IsAttribution { get => Is.Attribution; set => Is.Attribution = value; }

        public AttributionToken() : this("=")
        {
        }

        public AttributionToken(string token) :
            base(token)
        {
            IsOperator = true;
            IsAttribution = true;
        }
    }
}
