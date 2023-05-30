using System.Runtime.CompilerServices;

namespace Urderground.ORM.Core.Attributes
{
    public class MySqlFunctionScopeAttribute : Attribute
    {
        public string Name { get; private set; }

        public string CallerFilePath { get; private set; }

        public MySqlFunctionScopeAttribute(string name,
                                          [CallerFilePath] string callerFilePath = "")
        {
            Name = name;
            CallerFilePath = callerFilePath;
        }
    }
}
