using System.Data;

namespace Underground.ORM.Core.Translator.Syntax.Declaration
{
    public class VariableToken : MySqlSyntaxToken
    {
        public override bool IsDeclaration { get; set; }

        public override bool IsDbType { get; set; }

        public override bool IsString { get; set; }

        public override bool IsVar { get; set; }

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
