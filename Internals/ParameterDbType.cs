namespace MySqlOrm.Core.Internals
{
    internal class ParameterDbType
    {
        internal string Argument { get; set; } = "";

        internal string DbType { get; set; } = "";

        internal Type? FromType { get; set; } = null;

        internal List<ParameterDbType> SubTypes { get; set; } = new();

        protected ParameterDbType() { }

        internal ParameterDbType(string? argument, 
                                 string dbType, 
                                 Type? fromType)
        {
            Argument = argument ?? "";
            DbType = dbType;
            FromType = fromType;
        }

        internal void AddSubType(string argument, string dbType, Type? fromType)
        {
            SubTypes.Add(new ParameterDbType(argument, dbType, fromType));
        }

        internal static class Factory
        {
            internal static ParameterDbType Create()
            {
                return new ParameterDbType();
            }
        }

        
    }
}
