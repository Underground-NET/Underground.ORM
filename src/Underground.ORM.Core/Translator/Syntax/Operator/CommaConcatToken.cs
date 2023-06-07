namespace Underground.ORM.Core.Translator.Syntax.Operator
{
    public class CommaConcatToken : MySqlSyntaxToken
    {
        public override bool IsOperator { get; set; }

        public override bool IsCommaConcat { get; set; }

        public CommaConcatToken(string token) :
            base(token)
        {
            IsOperator = true;
            IsCommaConcat = true;
        }
    }
}
