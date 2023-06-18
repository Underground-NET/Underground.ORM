using Underground.ORM.Core.Syntax;

namespace Underground.ORM.Core.Translator.Parameter
{
    internal class SqlParameterDbType
    {
        internal string Argument { get; set; } = "";

        internal SyntaxBase DbType { get; set; } = "";

        internal Type? FromType { get; set; } = null;

        internal List<SqlParameterDbType> SubTypes { get; set; } = new();

        protected SqlParameterDbType() { }

        internal SqlParameterDbType(string? argument,
                                      SyntaxBase dbType,
                                      Type? fromType)
        {
            Argument = argument ?? "";
            DbType = dbType;
            FromType = fromType;
        }

        internal void AddSubType(string argument, 
                                 SyntaxBase dbType, 
                                 Type? fromType)
        {
            SubTypes.Add(new SqlParameterDbType(argument, dbType, fromType));
        }

        internal static class Factory
        {
            internal static SqlParameterDbType Create()
            {
                return new SqlParameterDbType();
            }
        }
    }
}
