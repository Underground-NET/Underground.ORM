using MySqlConnector;

namespace Underground.ORM.Core.Translator.Syntax.Variable
{
    public class MySqlSyntaxStringToken : MySqlSyntaxToken
    {
        public override bool IsString { get; set; }

        public MySqlSyntaxStringToken(string token) :
            base(token)
        {
            IsString = true;

            //Token = FormatString(token);
        }

        private string FormatString(string token)
        {
            return MySqlHelper.EscapeString(token);
        }
    }
}
