using MySqlConnector;

namespace Underground.ORM.Core.Translator.Syntax.Declaration
{
    public class StringToken : MySqlSyntaxToken
    {
        public override bool IsDeclaration { get; set; }

        public override bool IsString { get; set; }

        public override bool IsDbType { get; set; }

        public StringToken(string token) :
            base(token)
        {
            IsString = true;
            IsDbType = true;
            IsDeclaration = true;
            DbType = System.Data.DbType.String;

            //Token = FormatString(token);
        }

        private string FormatString(string token)
        {
            return MySqlHelper.EscapeString(token);
        }
    }
}
