namespace Underground.ORM.Core.Translator.Syntax.Token.Operator
{
    public class CommaConcatToken : MySqlSyntaxToken
    {
        public bool IsOperator { get => Is.Operator; set => Is.Operator = value; }

        public bool IsCommaConcat { get => Is.CommaConcat; set => Is.CommaConcat = value; }

        public CommaConcatToken(string token) :
            base(token)
        {
            IsOperator = true;
            IsCommaConcat = true;
        }
    }
}
