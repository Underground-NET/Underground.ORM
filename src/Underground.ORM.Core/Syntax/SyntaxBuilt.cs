using System.Reflection;

namespace Underground.ORM.Core.Syntax
{
    public class SyntaxBuiltBase : SyntaxBase
    {
        public MethodInfo Method { get; private set; }

        public string RoutineName { get; private set; }

        public string Statement { get; private set; }

        public SyntaxBase Syntax { get; private set; }

        public SyntaxBuiltBase(MethodInfo method,
                               string routineName,
                               string statement,
                               SyntaxBase syntax)
        {
            Method = method;
            RoutineName = routineName;
            Statement = statement;
            Syntax = syntax;
        }
    }
}
