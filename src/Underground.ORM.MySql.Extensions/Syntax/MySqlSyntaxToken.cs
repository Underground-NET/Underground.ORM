using Underground.ORM.Core.Syntax;

namespace Underground.ORM.MySql.Extensions.Syntax
{
    public class MySqlSyntaxToken : SyntaxTokenBase
    {
        public MySqlSyntaxToken(string token) : base(token)
        {
        }

        public MySqlSyntaxToken(string token, bool newline) : base(token, newline)
        {
        }
    }
}