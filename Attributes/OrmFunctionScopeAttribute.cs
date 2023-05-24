using System.Runtime.CompilerServices;

namespace MySqlOrm.Core.Attributes
{
    public class OrmFunctionScopeAttribute : Attribute
    {
        public string Name { get; private set; }

        public string CallerFilePath { get; private set; }

        public string CallerMemberName { get; private set; }

        public OrmFunctionScopeAttribute(string name, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "") 
        {
            Name = name;
            CallerFilePath = callerFilePath;
            CallerMemberName = callerMemberName;
        }
    }
}
