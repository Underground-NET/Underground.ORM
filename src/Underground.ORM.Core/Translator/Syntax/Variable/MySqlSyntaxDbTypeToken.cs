using System.Data;

namespace Underground.ORM.Core.Translator.Syntax.Variable
{
    public class MySqlSyntaxDbTypeToken : MySqlSyntaxToken
    {
        public DbType? DbType { get; private set; } = null;

        public override bool IsDbType { get; set; }

        public MySqlSyntaxDbTypeToken(string token, DbType dbType) :
            base(token)
        {
            IsDbType = true;
            DbType = dbType;
        }
    }
}
