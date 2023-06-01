using System.Reflection;

namespace Urderground.ORM.Core.Translator.Syntax
{
    public class MySqlSyntaxBuilt
    {
        public MethodInfo Method { get; private set; }

        public string RoutineName { get; private set; }

        public string Statement { get; private set; }

        public MySqlSyntax Syntax { get; private set; }

        public MySqlSyntaxBuilt(MethodInfo method,
                                string routineName,
                                string statement,
                                MySqlSyntax syntax)
        {
            Method = method;
            RoutineName = routineName;
            Statement = statement;
            Syntax = syntax;
        }
    }
}
