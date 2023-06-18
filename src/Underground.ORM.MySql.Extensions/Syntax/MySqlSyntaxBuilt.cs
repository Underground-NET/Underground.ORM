using System.Reflection;
using Underground.ORM.Core.Syntax;

namespace Underground.ORM.MySql.Extensions.Syntax
{
    public class MySqlSyntaxBuilt : SyntaxBuiltBase
    {
        public MySqlSyntaxBuilt(MethodInfo method, 
                                string routineName, 
                                string statement, 
                                SyntaxBase syntax) : 
            base(method, routineName, statement, syntax)
        {
        }
    }
}
