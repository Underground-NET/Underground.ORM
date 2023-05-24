using System.Runtime.CompilerServices;

namespace MySqlOrm.Core.Attributes
{
    public class OrmProcedureScopeAttribute : Attribute
    {
        public string Name { get; private set; }

        public string CallerFilePath { get; private set; }

        public string CallerMemberName { get; private set; }

        public OrmProcedureScopeAttribute(string name, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "")
        {
            Name = name;
            CallerFilePath = callerFilePath;
            CallerMemberName = callerMemberName;
        }
    }
}
