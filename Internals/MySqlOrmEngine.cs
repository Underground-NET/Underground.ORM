using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MySqlConnector;
using MySqlOrm.Core.Attributes;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace MySqlOrm.Core.Internals
{
    public class MySqlOrmEngine
    {
        private static readonly List<(MethodInfo, OrmFunctionScopeAttribute)> Functions = new();
        private static readonly List<(MethodInfo, OrmProcedureScopeAttribute)> Procedures = new();

        private MySqlConnection _connection;
        private string _connectionString;

        public MySqlOrmEngine(string connectionString)
        {
            _connectionString = connectionString;
        }

        static MySqlOrmEngine()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var method in type.GetMethods())
                    {
                        var functionAttribute = method.GetCustomAttribute<OrmFunctionScopeAttribute>();

                        if (functionAttribute != null)
                        {
                            Functions.Add((method, functionAttribute));
                        }

                        var procedureAttribute = method.GetCustomAttribute<OrmProcedureScopeAttribute>();

                        if (procedureAttribute != null)
                        {
                            Procedures.Add((method, procedureAttribute));
                        }
                    }
                }
            }
        }

        public async Task ConnectAsync()
        {
            _connection = new MySqlConnection(_connectionString);
            await _connection.OpenAsync();
        }

        public MySqlCommand GetCommand()
        {
            return _connection.CreateCommand();
        }

        public void Insert<T>(T model) where T : MySqlOrmEntity
        {

        }

        public void InsertIgnore<T>(T model) where T : MySqlOrmEntity
        {

        }

        public void Update<T>(T model) where T : MySqlOrmEntity
        {

        }

        public void UpdateOnDuplicateKey<T>(T model) where T : MySqlOrmEntity
        {

        }

        public void Delete<T>(T model) where T : MySqlOrmEntity
        {

        }

        public string RunFunction<T1>(Func<T1> function)
        {
            return RunFunctionInternal(function, function.GetMethodInfo(), Array.Empty<object?>());
        }

        public string RunFunction<T1, T2>(Func<T1, T2> function, T1 arg1)
        {
            return RunFunctionInternal(function, function.GetMethodInfo(), new object?[] { arg1 });
        }

        public string RunFunction<T1, T2, T3>(Func<T1, T2, T3> function, T1 arg1, T2 arg2)
        {
            return RunFunctionInternal(function, function.GetMethodInfo(), new object?[] { arg1, arg2 });
        }

        public string RunFunction<T1, T2, T3, T4>(Func<T1, T2, T3, T4> function, T1 arg1, T2 arg2, T3 arg3)
        {
            return RunFunctionInternal(function, function.GetMethodInfo(), new object?[] { arg1, arg2, arg3 });
        }

        public string RunFunction<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return RunFunctionInternal(function, function.GetMethodInfo(), new object?[] { arg1, arg2, arg3, arg4 });
        }

        public string RunFunction<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return RunFunctionInternal(function, function.GetMethodInfo(), new object?[] { arg1, arg2, arg3, arg4, arg5 });
        }

        private string RunFunctionInternal(object function, 
                                         MethodInfo method, 
                                         object?[] values)
        {
            var functionAttribute = method.GetCustomAttribute<OrmFunctionScopeAttribute>();

            if (functionAttribute == null)
            {
                throw new NotImplementedException($"Atributo '{nameof(OrmFunctionScopeAttribute)}' não definido para este método");
            }

            var returnType = method.ReturnType;
            var returnDbType = GetReturnDbType(returnType);

            #region ParametersIn

            var parameters = method.GetParameters();

            var parametersIn = parameters
                .Select(x => new ParameterDbType(x.Name,
                                                            GetDbType(x.ParameterType), 
                                                            x.ParameterType)).ToList();

            #endregion

            string createStatement = TranslateToPlMySql(method,
                                                        functionAttribute,
                                                        returnDbType,
                                                        parametersIn, 
                                                        new(), 
                                                        new());
            Debug.Print(createStatement);

            var command = GetCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = createStatement;

            var affected = command.ExecuteNonQuery();

            return createStatement;
        }

        private string TranslateToPlMySql(MethodInfo method,
                                          OrmFunctionScopeAttribute functionScopeAttribute,
                                          ParameterDbType parameterReturns,
                                          List<ParameterDbType> parametersIn,
                                          List<ParameterDbType> parametersOut,
                                          List<ParameterDbType> parametersInOut)
        {
            var csFileContent = File.ReadAllText(functionScopeAttribute.CallerFilePath);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(csFileContent);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            var nds = (NamespaceDeclarationSyntax)root.Members[0];
            var cds = (ClassDeclarationSyntax)nds.Members[0];

            var sb = new StringBuilder();

            var parametersInFunction = string.Join(", ", parametersIn.Select(x => $"`{x.Argument}` {x.DbType}"));

            sb.AppendLine($"CREATE FUNCTION `{functionScopeAttribute.Name}` ({parametersInFunction})");
            sb.AppendLine($"RETURNS {parameterReturns.DbType}");
            sb.AppendLine("BEGIN");

            foreach (var ds in cds.Members)
            {
                if (ds is MethodDeclarationSyntax)
                {
                    var mds = (MethodDeclarationSyntax)ds;
                    var methodName = mds.Identifier.ValueText;

                    if (methodName == method.Name)
                    {
                        // TODO: Desenvolver regras para tradução de C# para MySql

                        ConvertMethodBlockSyntax(mds.Body!, csFileContent, sb);
                        break;
                    }
                }
            }
            
            sb.AppendLine("END;");

            return sb.ToString();
        }

        private void ConvertMethodBlockSyntax(BlockSyntax block, string csFileContent, StringBuilder sb)
        {
            var statements = block.Statements;

            foreach (var statement in statements)
            {
                string codeLine = csFileContent[statement.Span.Start..statement.Span.End];
                string fullCodeLine = csFileContent[statement.FullSpan.Start..statement.FullSpan.End];
                var fullCodeLines = fullCodeLine.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                                     .Select(x => x.Trim()).ToList();

                #region Comments

                var comments = fullCodeLines.Where(x => x.StartsWith("//")).ToList();

                foreach (var comment in comments)
                {
                    sb.AppendLine("# " + comment[2..].Trim());
                }

                #endregion

                #region Declarations

                if (statement is LocalDeclarationStatementSyntax)
                {
                    var lds = (LocalDeclarationStatementSyntax)statement;

                    var vds = lds.Declaration;
                    string contentDeclaration = csFileContent[vds.Span.Start..vds.Span.End];
                    string fullContentDeclaration = csFileContent[vds.FullSpan.Start..vds.FullSpan.End];

                    var variables = vds.Variables;

                    foreach (var v in variables)
                    {
                        string contentVariable = csFileContent[v.Span.Start..v.Span.End];
                        string fullContentVariable = csFileContent[v.FullSpan.Start..v.FullSpan.End];

                        sb.AppendLine(ConvertDeclarationToMySql(contentDeclaration, contentVariable) + ";");
                    }
                }

                #endregion

                #region Return

                if (statement is ReturnStatementSyntax)
                {
                    var rss = (ReturnStatementSyntax)statement;

                    string contentExpression = csFileContent[rss.Expression.Span.Start..rss.Expression.Span.End];
                    string fullContentExpression = csFileContent[rss.Expression.FullSpan.Start..rss.Expression.FullSpan.End];

                    sb.AppendLine($"RETURN {contentExpression};");
                }

                #endregion
            }
        }

        private string ConvertDeclarationToMySql(string contentDeclaration, 
                                                 string contentVariable)
        {
            string[] linesDeclaration = contentDeclaration.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string[] linesVariable = contentVariable.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var typeName = linesDeclaration[0];
            var variableName = linesVariable[0];

            // TODO: Pegar o tipo com o método GetDbType()

            if (typeName == "List<string>")
            {
                return $"DECLARE `{variableName}` JSON DEFAULT JSON_ARRAY()";
            }
            else if (typeName == "string")
            {
                return $"DECLARE `{variableName}` VARCHAR(255) DEFAULT \"\"";
            }
            else if (typeName == "int")
            {
                return $"DECLARE `{variableName}` INT DEFAULT 0";
            }

            throw new NotImplementedException($"Tipo de declaração '{contentDeclaration}' não implementada");
        }

        private ParameterDbType GetReturnDbType(Type returnType)
        {
            var dbType = ParameterDbType.Factory.Create();

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
            else if (returnType == typeof(long))
            {
                dbType = "BIGINT";
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
            else
                throw new NotImplementedException();

            return dbType;
        }
    }
}