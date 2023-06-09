namespace Underground.ORM.Core.Translator.Syntax.Token.Operator
{
    public class CommaToken : MySqlSyntaxToken
    {
        public bool IsOperator { get => Is.Operator; set => Is.Operator = value; }

        public bool IsComma { get => Is.Comma; set => Is.Comma = value; }

        public CommaToken(string token) :
            base(token)
        {
            IsOperator = true;
            IsComma = true;
        }
    }
}
