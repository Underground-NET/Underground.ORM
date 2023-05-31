using System.Reflection;
using Urderground.ORM.Core.Translator.List;

namespace Urderground.ORM.Core.Translator
{
    public class MySqlSyntaxBuilt
    {
        public MethodInfo Method { get; private set; }

        public string RoutineName { get; private set; }

        public string Statement { get; private set; }

        public MySqlSyntaxList Syntax { get; private set; }

        public MySqlSyntaxBuilt(MethodInfo method, 
                                string routineName,
                                string statement, 
                                MySqlSyntaxList syntax)
        {
            Method = method;
            RoutineName = routineName;
            Statement = statement;
            Syntax = syntax;
        }
    }
}
