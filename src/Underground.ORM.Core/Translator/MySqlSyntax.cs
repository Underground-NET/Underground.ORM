using System.Reflection;

namespace Urderground.ORM.Core.Translator
{
    public class MySqlSyntax
    {
        public MethodInfo Method { get; private set; }

        public string RoutineName { get; private set; }

        public string Statement { get; private set; }

        public List<string> SourceCode { get; private set; }

        public MySqlSyntax(MethodInfo method, 
                           string routineName,
                           string statement, 
                           List<string> sourceCode)
        {
            Method = method;
            RoutineName = routineName;
            Statement = statement;
            SourceCode = sourceCode;
        }
    }
}
