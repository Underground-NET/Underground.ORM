using MySqlConnector;

namespace Underground.ORM.Core.Translator.Syntax.Declaration
{
    public class StringToken : MySqlSyntaxToken
    {
        public override bool IsDeclaration { get; set; }

        public override bool IsString { get; set; }

        public StringToken(string token) :
            base(token)
        {
            IsString = true;
            IsDeclaration = true;

            //Token = FormatString(token);
        }

        private string FormatString(string token)
        {
            return MySqlHelper.EscapeString(token);
        }
    }
}
