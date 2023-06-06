using System.Data;
using Underground.ORM.Core.Translator.Syntax;
using Underground.ORM.Core.Translator.Syntax.Variable;

namespace Urderground.ORM.Core.Translator
{
    public partial class MySqlTranslator
    {
        private MySqlSyntax TranslateDbTypeFromToken(string tokenType,
                                                     string contentDeclaration)
        {
            if (tokenType == "var")
            {
                return new MySqlSyntaxDbTypeToken(
                    "JSON ", DbType.Object);
            }
            else if (tokenType.StartsWith("List<") ||
                     tokenType.StartsWith("IList<") ||
                     tokenType.StartsWith("IEnumerable<") ||
                     tokenType.StartsWith("Enumerable<") ||
                     tokenType.StartsWith("Collection<") ||
                     tokenType.StartsWith("ICollection<") ||
                     tokenType.EndsWith("[]"))
            {
                return new MySqlSyntaxDbTypeToken(
                    "JSON ", DbType.Object);
            }
            else if (tokenType == "object" || tokenType == "Object" || tokenType == "System.Object")
            {
                return new MySqlSyntaxDbTypeToken(
                    "JSON", DbType.Object);
            }
            else if (tokenType == "char" || tokenType == "Char" || tokenType == "System.Char")
            {
                return new MySqlSyntaxDbTypeToken(
                    "CHAR ", DbType.StringFixedLength);
            }
            else if (tokenType == "bool" || tokenType == "Boolean" || tokenType == "System.Boolean")
            {
                return new MySqlSyntaxDbTypeToken(
                    "BOOL ", DbType.Boolean);
            }
            else if (tokenType == "sbyte" || tokenType == "SByte" || tokenType == "System.SByte")
            {
                return new MySqlSyntaxDbTypeToken(
                    "TINYINT ", DbType.SByte);
            }
            else if (tokenType == "byte" || tokenType == "Byte" || tokenType == "System.Byte")
            {
                return new MySqlSyntax((MySqlSyntaxDbTypeToken)
                    new ("TINYINT ", DbType.Byte), 
                    new ("UNSIGNED "));
            }
            else if (tokenType == "string" || tokenType == "String" || tokenType == "System.String")
            {
                return new MySqlSyntax((MySqlSyntaxDbTypeToken)
                    new("VARCHAR", DbType.String), 
                    "(", "255", ") ");
            }
            else if (tokenType == "short" || tokenType == "Int16" || tokenType == "System.Int16")
            {
                return new MySqlSyntaxDbTypeToken(
                    "SMALLINT ", DbType.Int16);
            }
            else if (tokenType == "ushort" || tokenType == "UInt16" || tokenType == "System.UInt16")
            {
                return new MySqlSyntax((MySqlSyntaxDbTypeToken)
                    new ("SMALLINT ", DbType.UInt16), 
                    new ("UNSIGNED "));
            }
            else if (tokenType == "int" || tokenType == "Int32" || tokenType == "System.Int32")
            {
                return new MySqlSyntaxDbTypeToken(
                    "INT ", DbType.Int32);
            }
            else if (tokenType == "uint" || tokenType == "UInt32" || tokenType == "System.UInt32")
            {
                return new MySqlSyntax((MySqlSyntaxDbTypeToken)
                    new ("INT ", DbType.UInt32), 
                    new ("UNSIGNED "));
            }
            else if (tokenType == "long" || tokenType == "Int64" || tokenType == "System.Int64")
            {
                return new MySqlSyntaxDbTypeToken(
                    "BIGINT ", DbType.Int64);
            }
            else if (tokenType == "ulong" || tokenType == "UInt64" || tokenType == "System.UInt64")
            {
                return new MySqlSyntax((MySqlSyntaxDbTypeToken)
                    new ("BIGINT ", DbType.UInt64), 
                    new ("UNSIGNED "));
            }
            else if (tokenType == "decimal" || tokenType == "Decimal" || tokenType == "System.Decimal")
            {
                return new MySqlSyntaxDbTypeToken(
                    "DECIMAL ", DbType.Decimal);
            }
            else if (tokenType == "double" || tokenType == "Double" || tokenType == "System.Double")
            {
                return new MySqlSyntaxDbTypeToken(
                    "DOUBLE ", DbType.Double);
            }
            else if (tokenType == "float" || tokenType == "Single" || tokenType == "System.Single")
            {
                return new MySqlSyntaxDbTypeToken(
                    "FLOAT ", DbType.Single);
            }
            else
                throw new NotImplementedException($"Declaration type '{contentDeclaration}' not supported");
        }
    }
}
