using System.Data;

namespace Underground.ORM.Core.Translator.Syntax.Token.Declaration
{
    public class VariableToken : MySqlSyntaxToken
    {
        public bool IsDeclaration { get => Is.Declaration; set => Is.Declaration = value; }

        public bool IsDbType { get => Is.DbType; set => Is.DbType = value; }

        public bool IsString { get => Is.String; set => Is.String = value; }

        public bool IsVar { get => Is.Var; set => Is.Var = value; }

        public VariableToken(string token) :
            base(token)
        {
            IsVar = true;
            IsDbType = true;
            IsDeclaration = true;
        }

        public VariableToken(string token, DbType dbType) :
            this(token)
        {
            DbType = dbType;
        }

        public void SetDbType(DbTypeToken token)
        {
            DbType = token.DbType;
        }
    }
}
