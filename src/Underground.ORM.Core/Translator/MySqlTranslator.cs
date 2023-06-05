using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data;
using System.Reflection;
using Underground.ORM.Core.Attributes;
using Underground.ORM.Core.Translator.Parameter;
using Underground.ORM.Core.Translator.Syntax;

namespace Urderground.ORM.Core.Translator
{
    public partial class MySqlTranslator
    {
        public MySqlSyntaxBuilt TranslateToFunctionCreateStatementSyntax(MethodInfo method)
        {
            var functionAttribute = method.GetCustomAttribute<MySqlFunctionScopeAttribute>();

            if (functionAttribute == null)
            {
                throw new NotImplementedException($"Atributo '{nameof(MySqlFunctionScopeAttribute)}' não definido para este método");
            }

            #region Function In Parameters

            var parameters = method.GetParameters();

            var parametersIn = parameters
                .Select(x => new MySqlParameterDbType(x.Name,
                                                      GetDbType(x.ParameterType),
                                                      x.ParameterType)).ToList();

            #endregion

            #region Function Out Parameters

            // TODO: Para tipos Out

            #endregion

            #region Function InOut Parameters

            // TODO: Para tipos Ref

            #endregion

            #region Function Return Parameter

            var returnType = method.ReturnType;
            var mysqlReturnDbType = GetMySqlReturnDbType(returnType);
            
            #endregion

            var csFileContent = File.ReadAllText(functionAttribute.CallerFilePath);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(csFileContent);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var nds = (NamespaceDeclarationSyntax)root.Members[0];
            var cds = (ClassDeclarationSyntax)nds.Members[0];

            MySqlSyntax mysqlSyntaxOut = new();

            mysqlSyntaxOut.Append("CREATE ", "FUNCTION ", $"`{functionAttribute.RoutineName}`", "(");

            foreach (var dbType in parametersIn)
            {
                mysqlSyntaxOut.Append(new MySqlSyntaxVariableToken($"`{dbType.Argument}` "));
                mysqlSyntaxOut.AppendRange(dbType.DbType);
                mysqlSyntaxOut.Append(", ");
            }
            mysqlSyntaxOut.RemoveLast();

            mysqlSyntaxOut.AppendLine(")");
            mysqlSyntaxOut.AppendLine("RETURNS ", $"{mysqlReturnDbType.DbType}");
            mysqlSyntaxOut.AppendLine("BEGIN");

            foreach (var ds in cds.Members)
            {
                if (ds is MethodDeclarationSyntax)
                {
                    var mds = (MethodDeclarationSyntax)ds;
                    var methodName = mds.Identifier.ValueText;

                    if (methodName == method.Name)
                    {
                        TranslateMethodBlockSyntax(mds.Body!, csFileContent, mysqlSyntaxOut);
                        break;
                    }
                }
            }

            mysqlSyntaxOut.Append("END", ";");

            return new MySqlSyntaxBuilt(method,
                                        functionAttribute.RoutineName,
                                        mysqlSyntaxOut.ToMySqlString(true), 
                                        mysqlSyntaxOut);
        }

        private MySqlParameterDbType GetMySqlReturnDbType(Type returnType)
        {
            var dbType = MySqlParameterDbType.Factory.Create();

            dbType.DbType = GetDbType(returnType);
            dbType.FromType = returnType;

            if (returnType.IsGenericType)
            {
                foreach (var genericType in returnType.GenericTypeArguments)
                {
                    dbType.AddSubType(genericType.Name, GetDbType(genericType), genericType);
                }
            }

            return dbType;
        }

        private MySqlSyntax GetDbType(Type returnType)
        {
            MySqlSyntax dbType;

            if (returnType.Name == "List`1")
            {
                dbType = new MySqlSyntaxToken(
                    "JSON", DbType.Object);
            }
            else if (returnType == typeof(string))
            {
                dbType = new((MySqlSyntaxToken)
                    new ("VARCHAR", DbType.AnsiString), 
                    "(", "255", ")");
            }
            else if (returnType == typeof(bool))
            {
                dbType = new MySqlSyntaxToken(
                    "BOOL", DbType.Boolean);
            }
            else if (returnType == typeof(short))
            {
                dbType = new MySqlSyntaxToken(
                    "SMALLINT", DbType.Int16);
            }
            else if (returnType == typeof(ushort))
            {
                dbType = new ((MySqlSyntaxToken)
                    new ("SMALLINT ", DbType.UInt16), 
                    new ("UNSIGNED"));
            }
            else if (returnType == typeof(int))
            {
                dbType = new MySqlSyntaxToken(
                    "INT", DbType.Int32);
            }
            else if (returnType == typeof(uint))
            {
                dbType = new ((MySqlSyntaxToken)
                    new ("INT ", DbType.UInt32), 
                    new ("UNSIGNED"));
            }
            else if (returnType == typeof(long))
            {
                dbType = new MySqlSyntaxToken(
                    "BIGINT", DbType.Int64);
            }
            else if (returnType == typeof(ulong))
            {
                dbType = new((MySqlSyntaxToken)
                    new("BIGINT ", DbType.UInt64), 
                    new("UNSIGNED"));
            }
            else if (returnType == typeof(decimal))
            {
                dbType = new MySqlSyntaxToken(
                    "DECIMAL", DbType.Decimal);
            }
            else if (returnType == typeof(double))
            {
                dbType = new MySqlSyntaxToken(
                    "DOUBLE", DbType.Double);
            }
            else if (returnType == typeof(byte[]))
            {
                dbType = new MySqlSyntaxToken(
                    "LONGBLOB", DbType.Binary);
            }
            else if (returnType == typeof(DateTime))
            {
                dbType = new MySqlSyntaxToken(
                    "DATETIME", DbType.DateTime);
            }
            else if (returnType == typeof(TimeSpan))
            {
                dbType = new MySqlSyntaxToken(
                    "TIMESTAMP", DbType.Time);
            }
            else if (returnType == typeof(Guid))
            {
                dbType = new ((MySqlSyntaxToken)
                    new("CHAR", DbType.AnsiStringFixedLength), 
                    "(", "36", ")");
            }
            else if (returnType == typeof(char))
            {
                dbType = new MySqlSyntaxToken(
                    "CHAR", DbType.AnsiStringFixedLength);
            }
            else
                throw new NotImplementedException();

            return dbType;
        }
    }
}
