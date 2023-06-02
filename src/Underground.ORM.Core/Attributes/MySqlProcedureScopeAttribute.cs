using System.Runtime.CompilerServices;

namespace Underground.ORM.Core.Attributes
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
