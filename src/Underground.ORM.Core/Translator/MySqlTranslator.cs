using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;
using Urderground.ORM.Core.Attributes;
using Urderground.ORM.Core.Translator.Parameter;
using Urderground.ORM.Core.Translator.Syntax;

namespace Urderground.ORM.Core.Translator
{
    public partial class MySqlTranslator
    {
        public MySqlSyntaxBuilt TranslateToFunctionCreateSyntax(MethodInfo method)
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

            var arguments = parametersIn
                .Select(x => new MySqlSyntax() 
                {
                    $"`{x.Argument}` ", x.DbType, ", ",
                }).ToList();

            arguments.Last().RemoveLast();

            mysqlSyntaxOut.Append("CREATE ", "FUNCTION ", $"`{functionAttribute.RoutineName}`", "(");
            mysqlSyntaxOut.AppendRange(arguments);
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

        private string GetDbType(Type returnType)
        {
            string dbType;

            if (returnType.Name == "List`1")
            {
                dbType = "JSON";
            }
            else if (returnType == typeof(string))
            {
                dbType = "VARCHAR(255)";
            }
            else if (returnType == typeof(bool))
            {
                dbType = "TINYINT";
            }
            else if (returnType == typeof(short))
            {
                dbType = "SMALLINT";
            }
            else if (returnType == typeof(int))
            {
                dbType = "INT";
            }
            else if (returnType == typeof(uint))
            {
                dbType = "INT UNSIGNED";
            }
            else if (returnType == typeof(long))
            {
                dbType = "BIGINT";
            }
            else if (returnType == typeof(ulong))
            {
                dbType = "BIGINT UNSIGNED";
            }
            else if (returnType == typeof(decimal))
            {
                dbType = "DECIMAL";
            }
            else if (returnType == typeof(double))
            {
                dbType = "DECIMAL";
            }
            else if (returnType == typeof(byte[]))
            {
                dbType = "LONGBLOB";
            }
            else if (returnType == typeof(DateTime))
            {
                dbType = "DATETIME";
            }
            else if (returnType == typeof(TimeSpan))
            {
                dbType = "TIMESTAMP";
            }
            else if (returnType == typeof(Guid))
            {
                dbType = "CHAR(36)";
            }
            else if (returnType == typeof(char))
            {
                dbType = "CHAR";
            }
            else
                throw new NotImplementedException();

            return dbType;
        }
    }
}
