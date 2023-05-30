using System.Runtime.CompilerServices;

namespace Urderground.ORM.Core.Attributes
{
    public class MySqlProcedureScopeAttribute : Attribute
    {
        public string Name { get; private set; }

        public string CallerFilePath { get; private set; }

        public MySqlProcedureScopeAttribute(string name,
                                            [CallerFilePath] string callerFilePath = "")
        {
            Name = name;
            CallerFilePath = callerFilePath;
        }
    }
}
