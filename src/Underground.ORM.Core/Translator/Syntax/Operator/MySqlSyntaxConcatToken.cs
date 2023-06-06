namespace Underground.ORM.Core.Translator.Syntax.Operator
{
    public class MySqlSyntaxConcatToken : MySqlSyntaxToken
    {
        public override bool IsConcat { get; set; }

        public MySqlSyntaxConcatToken(string token) :
            base(token)
        {
            IsConcat = true;
        }
    }
}
