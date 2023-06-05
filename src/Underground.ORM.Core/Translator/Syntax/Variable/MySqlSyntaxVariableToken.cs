using System.Data;

namespace Underground.ORM.Core.Translator.Syntax.Variable
{
    public class MySqlSyntaxVariableToken : MySqlSyntaxToken
    {
        public DbType? DbType { get; private set; } = null;

        public override bool IsVar { get; set; }

        public MySqlSyntaxVariableToken(string token) :
            base(token)
        {
            IsVar = true;
        }

        public void SetDbType(MySqlSyntaxDbTypeToken token)
        {
            DbType = token.DbType;
        }
    }
}
