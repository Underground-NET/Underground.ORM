using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MySqlConnector;
using MySqlOrm.Core.Attributes;
using System.Linq.Expressions;
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
            
            //var command = GetCommand();
            //command.CommandType = System.Data.CommandType.Text;
            //command.CommandText = createStatement;
            //var affected = command.ExecuteNonQuery();

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
                var fullCodeLines = fullCodeLine.TrimEnd().Split(Environment.NewLine)
                                                     .Select(x => x.Trim()).ToList();

                #region Comments

                foreach (var line in fullCodeLines)
                {
                    if (line.StartsWith("//"))
                        sb.AppendLine("# " + line[2..].Trim());
                    else if (string.IsNullOrEmpty(line.Trim()))
                        sb.AppendLine();
                }

                #endregion

                var descendantNodesAndTokensAndSelf = statement.DescendantNodesAndTokensAndSelf().ToList();

                TranslateToMySql(csFileContent, 
                                 descendantNodesAndTokensAndSelf, 
                                 sb);

            }
        }

        private void TranslateToMySql(string csFileContent,
                                      List<SyntaxNodeOrToken> descendantNodesAndTokensAndSelf, 
                                      StringBuilder sb)
        {
            foreach (var item in descendantNodesAndTokensAndSelf)
            {
                var variableDeclaration = item.AsNode() as VariableDeclarationSyntax;

                if (variableDeclaration != null)
                {
                    TranslateDeclarationToMySql(csFileContent, 
                                                variableDeclaration,
                                                descendantNodesAndTokensAndSelf,
                                                sb);
                }
            }
        }

        private void TranslateDeclarationToMySql(string csFileContent,
                                                 VariableDeclarationSyntax declaration,
                                                 List<SyntaxNodeOrToken> descendants,
                                                 StringBuilder mysqlSyntaxOut)
        {
            Dictionary<int, string> mysqlDeclare = new();
            (int Order, List<string>)? mysqlExpression = null;
            (int Order, string) mysqlDbType;

            int variablesCount = 0;
            bool defaultValue = false;

            #region Predefined Type

            var predefinedTypeSyntax = descendants.Select(x => x.AsNode()).OfType<PredefinedTypeSyntax>().FirstOrDefault();
            var predefinedType = descendants.Select(x => x.AsNode()).OfType<VariableDeclarationSyntax>().FirstOrDefault();

            string dbType;
            if (predefinedTypeSyntax != null)
            {
                string contentDeclaration = csFileContent[predefinedTypeSyntax.Span.Start..predefinedTypeSyntax.Span.End];
                dbType = GetDbTypeFromToken(predefinedTypeSyntax.Keyword.Text, contentDeclaration);
                
            }
            else if (predefinedType != null)
            {
                string contentDeclaration = csFileContent[predefinedType.Span.Start..predefinedType.Span.End];

                var identifierNameSyntax = predefinedType.Type as IdentifierNameSyntax;
                var nullableTypeSyntax = predefinedType.Type as NullableTypeSyntax;

                string variableTypeName = "";

                if (nullableTypeSyntax != null)
                    variableTypeName = csFileContent[nullableTypeSyntax.Span.Start..nullableTypeSyntax.Span.End].TrimEnd('?');
                else
                    variableTypeName = csFileContent[identifierNameSyntax!.Span.Start..identifierNameSyntax.Span.End];

                dbType = GetDbTypeFromToken(variableTypeName, contentDeclaration);
            }
            else
                throw new NotImplementedException("Predefined type not found");

            mysqlDbType = (3, $" {dbType}");
            #endregion

            foreach (var item in descendants)
            {
                string contentDeclaration = csFileContent[item.Span.Start..item.Span.End];

                var node = item.AsNode();
                var token = item.AsToken();

                var variableDeclarator = item.AsNode() as VariableDeclaratorSyntax;
                var equalsValueClauseSyntax = item.AsNode() as EqualsValueClauseSyntax;
                var expressionSyntax = item.AsNode() as ExpressionSyntax;
                var identifierNameSyntax = item.AsNode() as IdentifierNameSyntax;

                if (variableDeclarator != null)
                {
                    defaultValue = false;

                    FinalizeDeclarationStatementMySql(mysqlDeclare, mysqlExpression, mysqlDbType, mysqlSyntaxOut);

                    string variableName = variableDeclarator.Identifier.ValueText;

                    mysqlDeclare.Add(1, "DECLARE ");
                    mysqlDeclare.Add(2, $"`{variableName}`");
                    
                    variablesCount++;
                }

                if (equalsValueClauseSyntax != null)
                {
                    defaultValue = true;
                    mysqlDeclare.Add(4, " DEFAULT ");
                }

                if (expressionSyntax != null && defaultValue)
                {
                    var descendantExpression = expressionSyntax.DescendantNodesAndTokensAndSelf().ToList();
                    var childNodesExpression = expressionSyntax.ChildNodesAndTokens().ToList();

                    var expTranslated = TranslateDeclarationExpressionToMySql(csFileContent,
                                                                        descendantExpression,
                                                                        childNodesExpression);

                    FinalizeDeclarationStatementMySql(mysqlDeclare, (5, expTranslated), mysqlDbType, mysqlSyntaxOut);

                    defaultValue = false;
                }
            }

            FinalizeDeclarationStatementMySql(mysqlDeclare, mysqlExpression, mysqlDbType, mysqlSyntaxOut);
        }

        private void FinalizeDeclarationStatementMySql(Dictionary<int, string> mysqlDeclare,
                                                    (int Order, List<string>)? mysqlExpression,
                                                    (int Order, string) mysqlDbType,
                                                    StringBuilder mysqlSyntaxOut)
        {
            if (!mysqlDeclare.Any()) return;

            mysqlDeclare.Add(mysqlDbType.Order, mysqlDbType.Item2);

            if (mysqlExpression != null)
                mysqlDeclare.Add(mysqlExpression.Value.Order, string.Join("", mysqlExpression.Value.Item2));

            // MySql Statement: (1)DECLARE (2)`variable` (3)type (4)DEFAULT (5)expression;

            var mysqlStatement = string.Join("", mysqlDeclare.OrderBy(x => x.Key).Select(x => x.Value));

            mysqlSyntaxOut.AppendLine(mysqlStatement);
            mysqlDeclare.Clear();
        }

        private List<string> TranslateDeclarationExpressionToMySql(string csFileContent,
                                                        List<SyntaxNodeOrToken> descendantNodesAndTokensAndSelf,
                                                        List<SyntaxNodeOrToken> childNodesAndTokens)
        {
            List<string> expression = new();

            foreach (var item in descendantNodesAndTokensAndSelf)
            {
                string contentExpression = csFileContent[item.Span.Start..item.Span.End];

                var node = item.AsNode();
                var token = item.AsToken();

                if (token.Parent != null)
                {
                    var identifierNameSyntax = token.Parent as IdentifierNameSyntax;

                    string valueText = token.ValueText;

                    if (identifierNameSyntax != null)
                    {
                        valueText = $"`{valueText}`";
                    }

                    expression.Add(valueText);
                }
            }

            return expression;
        }

        private List<string> TranslateExpressionToMySql(string csFileContent,
                                                        List<SyntaxNodeOrToken> descendantNodesAndTokensAndSelf,
                                                        List<SyntaxNodeOrToken> childNodesAndTokens)
        {
            List<string> expression = new ();

            foreach (var item in descendantNodesAndTokensAndSelf)
            {
                string contentExpression = csFileContent[item.Span.Start..item.Span.End];

                var node = item.AsNode();
                var token = item.AsToken();

                if (token.Parent != null)
                {
                    var identifierNameSyntax = token.Parent as IdentifierNameSyntax;

                    string valueText = token.ValueText;

                    if (identifierNameSyntax != null)
                    {
                        valueText = $"`{valueText}`";
                    }

                    expression.Add(valueText);
                }
            }

            return expression;
        }

        private string GetDbTypeFromToken(string tokenType, 
                                          string contentDeclaration)
        {
            if(tokenType == "var")
            {
                return "JSON";
            }
            else if (tokenType.StartsWith("List<") ||
                     tokenType.StartsWith("IList<") ||
                     tokenType.StartsWith("IEnumerable<") ||
                     tokenType.StartsWith("Enumerable<") ||
                     tokenType.StartsWith("Collection<") ||
                     tokenType.StartsWith("ICollection<") ||
                     tokenType.EndsWith("[]"))
            {
                return "JSON";
            }
            else if (tokenType == "object" || tokenType == "Object" || tokenType == "System.Object")
            {
                return "JSON";
            }
            else if (tokenType == "char" || tokenType == "Char" || tokenType == "System.Char")
            {
                return "CHAR";
            }
            else if (tokenType == "bool" || tokenType == "Boolean" || tokenType == "System.Boolean")
            {
                return "BOOL";
            }
            else if (tokenType == "sbyte" || tokenType == "SByte" || tokenType == "System.SByte")
            {
                return "TINYINT";
            }
            else if (tokenType == "byte" || tokenType == "Byte" || tokenType == "System.Byte")
            {
                return "TINYINT UNSIGNED";
            }
            else if (tokenType == "string" || tokenType == "String" || tokenType == "System.String")
            {
                return "VARCHAR(255)";
            }
            else if (tokenType == "short" || tokenType == "Int16" || tokenType == "System.Int16")
            {
                return "SMALLINT";
            }
            else if (tokenType == "ushort" || tokenType == "UInt16" || tokenType == "System.UInt16")
            {
                return "SMALLINT UNSIGNED";
            }
            else if (tokenType == "int" || tokenType == "Int32" || tokenType == "System.Int32")
            {
                return "INT";
            }
            else if (tokenType == "uint" || tokenType == "UInt32" || tokenType == "System.UInt32")
            {
                return "INT UNSIGNED";
            }
            else if (tokenType == "long" || tokenType == "Int64" || tokenType == "System.Int64")
            {
                return "BIGINT";
            }
            else if (tokenType == "ulong" || tokenType == "UInt64" || tokenType == "System.UInt64")
            {
                return "BIGINT UNSIGNED";
            }
            else if (tokenType == "decimal" || tokenType == "Decimal" || tokenType == "System.Decimal")
            {
                return "DECIMAL";
            }
            else if (tokenType == "double" || tokenType == "Double" || tokenType == "System.Double")
            {
                return "DOUBLE";
            }
            else if (tokenType == "float" || tokenType == "Single" || tokenType == "System.Single")
            {
                return "FLOAT";
            }
            else
                throw new NotImplementedException($"Tipo de declaração '{contentDeclaration}' não suportada");

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