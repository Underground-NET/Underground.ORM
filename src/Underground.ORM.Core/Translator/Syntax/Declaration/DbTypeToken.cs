using System.Data;

namespace Underground.ORM.Core.Translator.Syntax.Declaration
{
    public class DbTypeToken : MySqlSyntaxToken
    {
        public override bool IsDeclaration { get; set; }

        public DbType? DbType { get; private set; } = null;

        public override bool IsDbType { get; set; }

        public DbTypeToken(string token, DbType dbType) :
            base(token)
        {
            IsDbType = true;
            DbType = dbType;
            IsDeclaration = true;
        }
    }
}
