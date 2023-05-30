using System.Runtime.CompilerServices;

namespace Urderground.ORM.Core.Attributes
{
    public class MySqlFunctionScopeAttribute : Attribute
    {
        public string RoutineName { get; private set; }

        public string CallerFilePath { get; private set; }

        public MySqlFunctionScopeAttribute(string routineName,
                                          [CallerFilePath] string callerFilePath = "")
        {
            RoutineName = routineName;
            CallerFilePath = callerFilePath;
        }
    }
}
