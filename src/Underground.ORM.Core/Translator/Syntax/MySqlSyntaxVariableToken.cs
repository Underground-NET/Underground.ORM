namespace Underground.ORM.Core.Translator.Syntax
{
    public class MySqlSyntaxVariableToken : MySqlSyntaxToken
    {
        public MySqlSyntaxVariableToken(string token) :
            base(token)
        {
            base.IsVar = true;
        }
    }
}
