using System.Data;

namespace Underground.ORM.Core.Translator.Syntax.Variable
{
    public class MySqlSyntaxConcatToken : MySqlSyntaxToken
    {
        public override bool IsConcat { get; set; }

        public MySqlSyntaxConcatToken(string token) :
            base(token)
        {
            IsConcat = true;
        }
    }
}
