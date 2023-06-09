using System.Data;

namespace Underground.ORM.Core.Translator.Syntax.Token.Declaration
{
    public class DbTypeToken : MySqlSyntaxToken
    {
        public bool IsDeclaration { get => Is.Declaration; set => Is.Declaration = value; }

        public bool IsDbType { get => Is.DbType; set => Is.DbType = value; }

        public bool IsString { get => Is.String; set => Is.String = value; }

        public DbTypeToken(string token, DbType dbType) :
            base(token)
        {
            IsDbType = true;
            DbType = dbType;
            IsDeclaration = true;
        }
    }
}
