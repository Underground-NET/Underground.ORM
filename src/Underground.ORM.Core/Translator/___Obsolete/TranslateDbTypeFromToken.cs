using System.Data;
using Underground.ORM.Core.Translator.Syntax;
using Underground.ORM.Core.Translator.Syntax.Token.Declaration;
using Underground.ORM.Core.Translator.Syntax.Token.Operator;

namespace Urderground.ORM.Core.Translator
{
    public partial class ___MySqlTranslator
    {
        public static MySqlSyntax TranslateDbTypeFromToken(string tokenType,
                                                           string contentDeclaration)
        {
            if (tokenType == "var")
            {
                return new DbTypeToken("JSON ", DbType.Object);
            }
            else if (tokenType.StartsWith("List<") ||
                     tokenType.StartsWith("IList<") ||
                     tokenType.StartsWith("IEnumerable<") ||
                     tokenType.StartsWith("Enumerable<") ||
                     tokenType.StartsWith("Collection<") ||
                     tokenType.StartsWith("ICollection<") ||
                     tokenType.EndsWith("[]"))
            {
                return new DbTypeToken("JSON ", DbType.Object);
            }
            else if (tokenType == "object" || tokenType == "Object" || tokenType == "System.Object")
            {
                return new DbTypeToken("JSON", DbType.Object);
            }
            else if (tokenType == "char" || tokenType == "Char" || tokenType == "System.Char")
            {
                return new DbTypeToken("CHAR ", DbType.StringFixedLength);
            }
            else if (tokenType == "bool" || tokenType == "Boolean" || tokenType == "System.Boolean")
            {
                return new DbTypeToken("BOOL ", DbType.Boolean);
            }
            else if (tokenType == "sbyte" || tokenType == "SByte" || tokenType == "System.SByte")
            {
                return new DbTypeToken("TINYINT ", DbType.SByte);
            }
            else if (tokenType == "byte" || tokenType == "Byte" || tokenType == "System.Byte")
            {
                return new MySqlSyntax(
                    new DbTypeToken("TINYINT ", DbType.Byte), 
                    new DbTypeToken("UNSIGNED ", DbType.Byte));
            }
            else if (tokenType == "string" || tokenType == "String" || tokenType == "System.String")
            {
                return new MySqlSyntax(
                    new DbTypeToken("VARCHAR", DbType.String), 
                    new OpenParenthesisToken("("), "255", new CloseParenthesisToken(") "));
            }
            else if (tokenType == "short" || tokenType == "Int16" || tokenType == "System.Int16")
            {
                return new DbTypeToken("SMALLINT ", DbType.Int16);
            }
            else if (tokenType == "ushort" || tokenType == "UInt16" || tokenType == "System.UInt16")
            {
                return new MySqlSyntax(
                    new DbTypeToken("SMALLINT ", DbType.UInt16), 
                    new DbTypeToken("UNSIGNED ", DbType.UInt16));
            }
            else if (tokenType == "int" || tokenType == "Int32" || tokenType == "System.Int32")
            {
                return new DbTypeToken("INT ", DbType.Int32);
            }
            else if (tokenType == "uint" || tokenType == "UInt32" || tokenType == "System.UInt32")
            {
                return new MySqlSyntax(
                    new DbTypeToken ("INT ", DbType.UInt32), 
                    new DbTypeToken ("UNSIGNED ", DbType.UInt32));
            }
            else if (tokenType == "long" || tokenType == "Int64" || tokenType == "System.Int64")
            {
                return new DbTypeToken("BIGINT ", DbType.Int64);
            }
            else if (tokenType == "ulong" || tokenType == "UInt64" || tokenType == "System.UInt64")
            {
                return new MySqlSyntax(
                    new DbTypeToken("BIGINT ", DbType.UInt64), 
                    new DbTypeToken("UNSIGNED ", DbType.UInt64));
            }
            else if (tokenType == "decimal" || tokenType == "Decimal" || tokenType == "System.Decimal")
            {
                return new DbTypeToken("DECIMAL ", DbType.Decimal);
            }
            else if (tokenType == "double" || tokenType == "Double" || tokenType == "System.Double")
            {
                return new DbTypeToken("DOUBLE ", DbType.Double);
            }
            else if (tokenType == "float" || tokenType == "Single" || tokenType == "System.Single")
            {
                return new DbTypeToken("FLOAT ", DbType.Single);
            }
            else
                throw new NotImplementedException($"Declaration type '{contentDeclaration}' not supported");
        }
    }
}
