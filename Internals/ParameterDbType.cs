namespace MySqlOrm.Core.Internals
{
    internal class ParameterDbType
    {
        internal string DbType { get; set; } = "";

        internal Type? FromType { get; set; } = null;

        internal List<ParameterDbType> SubTypes { get; set; } = new();

        protected ParameterDbType() { }

        internal ParameterDbType(string dbType, Type? fromType)
        {
            DbType = dbType;
            FromType = fromType;
        }

        internal void AddSubType(string dbType, Type? fromType)
        {
            SubTypes.Add(new ParameterDbType(dbType, fromType));
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
