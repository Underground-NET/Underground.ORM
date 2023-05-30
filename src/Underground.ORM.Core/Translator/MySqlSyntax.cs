using System.Reflection;

namespace Urderground.ORM.Core.Translator
{
    public class MySqlSyntax
    {
        public MethodInfo Method { get; private set; }

        public string RoutineName { get; private set; }

        public string Statement { get; private set; }

        public List<string> Code { get; private set; }

        public MySqlSyntax(MethodInfo method, 
                           string routineName,
                           string statement, 
                           List<string> code)
        {
            Method = method;
            RoutineName = routineName;
            Statement = statement;
            Code = code;
        }
    }
}
