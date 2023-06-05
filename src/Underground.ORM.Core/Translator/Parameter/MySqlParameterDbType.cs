using Underground.ORM.Core.Translator.Syntax;

namespace Underground.ORM.Core.Translator.Parameter
{
    internal class MySqlParameterDbType
    {
        internal string Argument { get; set; } = "";

        internal MySqlSyntax DbType { get; set; } = "";

        internal Type? FromType { get; set; } = null;

        internal List<MySqlParameterDbType> SubTypes { get; set; } = new();

        protected MySqlParameterDbType() { }

        internal MySqlParameterDbType(string? argument,
                                      MySqlSyntax dbType,
                                      Type? fromType)
        {
            Argument = argument ?? "";
            DbType = dbType;
            FromType = fromType;
        }

        internal void AddSubType(string argument, 
                                 MySqlSyntax dbType, 
                                 Type? fromType)
        {
            SubTypes.Add(new MySqlParameterDbType(argument, dbType, fromType));
        }

        internal static class Factory
        {
            internal static MySqlParameterDbType Create()
            {
                return new MySqlParameterDbType();
            }
        }
    }
}
