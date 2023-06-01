using Urderground.ORM.Core.Translator.Syntax;

namespace Urderground.ORM.Core.Translator
{
    public partial class MySqlTranslator
    {
        private MySqlSyntax TranslateDbTypeFromToken(string tokenType,
                                                     string contentDeclaration)
        {
            if (tokenType == "var")
            {
                return new MySqlSyntaxItem("JSON ");
            }
            else if (tokenType.StartsWith("List<") ||
                     tokenType.StartsWith("IList<") ||
                     tokenType.StartsWith("IEnumerable<") ||
                     tokenType.StartsWith("Enumerable<") ||
                     tokenType.StartsWith("Collection<") ||
                     tokenType.StartsWith("ICollection<") ||
                     tokenType.EndsWith("[]"))
            {
                return new MySqlSyntaxItem("JSON ");
            }
            else if (tokenType == "object" || tokenType == "Object" || tokenType == "System.Object")
            {
                return new MySqlSyntaxItem("JSON");
            }
            else if (tokenType == "char" || tokenType == "Char" || tokenType == "System.Char")
            {
                return new MySqlSyntaxItem("CHAR ");
            }
            else if (tokenType == "bool" || tokenType == "Boolean" || tokenType == "System.Boolean")
            {
                return new MySqlSyntaxItem("BOOL ");
            }
            else if (tokenType == "sbyte" || tokenType == "SByte" || tokenType == "System.SByte")
            {
                return new MySqlSyntaxItem("TINYINT ");
            }
            else if (tokenType == "byte" || tokenType == "Byte" || tokenType == "System.Byte")
            {
                return new MySqlSyntax("TINYINT ", "UNSIGNED ");
            }
            else if (tokenType == "string" || tokenType == "String" || tokenType == "System.String")
            {
                return new MySqlSyntax("VARCHAR", "(", "255", ") ");
            }
            else if (tokenType == "short" || tokenType == "Int16" || tokenType == "System.Int16")
            {
                return new MySqlSyntaxItem("SMALLINT ");
            }
            else if (tokenType == "ushort" || tokenType == "UInt16" || tokenType == "System.UInt16")
            {
                return new MySqlSyntax("SMALLINT ", "UNSIGNED ");
            }
            else if (tokenType == "int" || tokenType == "Int32" || tokenType == "System.Int32")
            {
                return new MySqlSyntaxItem("INT ");
            }
            else if (tokenType == "uint" || tokenType == "UInt32" || tokenType == "System.UInt32")
            {
                return new MySqlSyntax("INT ", "UNSIGNED");
            }
            else if (tokenType == "long" || tokenType == "Int64" || tokenType == "System.Int64")
            {
                return new MySqlSyntaxItem("BIGINT ");
            }
            else if (tokenType == "ulong" || tokenType == "UInt64" || tokenType == "System.UInt64")
            {
                return new MySqlSyntax("BIGINT ", "UNSIGNED");
            }
            else if (tokenType == "decimal" || tokenType == "Decimal" || tokenType == "System.Decimal")
            {
                return new MySqlSyntaxItem("DECIMAL ");
            }
            else if (tokenType == "double" || tokenType == "Double" || tokenType == "System.Double")
            {
                return new MySqlSyntaxItem("DOUBLE ");
            }
            else if (tokenType == "float" || tokenType == "Single" || tokenType == "System.Single")
            {
                return new MySqlSyntaxItem("FLOAT ");
            }
            else
                throw new NotImplementedException($"Tipo de declaração '{contentDeclaration}' não suportada");
        }
    }
}
