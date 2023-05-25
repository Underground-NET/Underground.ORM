using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualBasic;
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

                        sb.AppendLine(ConvertDeclarationToMySql(v, 
                                                                contentDeclaration, 
                                                                contentVariable) + ";");
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

        private string ConvertDeclarationToMySql(VariableDeclaratorSyntax v,
                                                 string contentDeclaration,
                                                 string contentVariable)
        {
            StringBuilder sb = new();
            object? valueInitialization = null;
            object? valueCreateInitialization = null;
            bool valueInitializationDefaultNull = false;
            var variableName = v.Identifier.ValueText;

            bool castFunction = false;
            string openCastFunction = "";
            string closeCastFunction = "";

            var parent = v.Parent as VariableDeclarationSyntax;
            var parentParent = parent!.Parent as LocalDeclarationStatementSyntax;

            #region ValueType

            var variableTypeSyntax = parent!.Type;
            var nullableTypeSyntax = variableTypeSyntax as NullableTypeSyntax;
            var genericNameSyntax = variableTypeSyntax as GenericNameSyntax;
            var predefinedTypeSyntax = variableTypeSyntax as PredefinedTypeSyntax;
            var arrayTypeSyntax = variableTypeSyntax as ArrayTypeSyntax;
            
            if (genericNameSyntax != null)
            {
                var identifierGenericName = genericNameSyntax.Identifier;
            }

            if (nullableTypeSyntax != null)
            {
                var nullableParent = (VariableDeclarationSyntax)nullableTypeSyntax.Parent!;
                var declaration = parentParent!.Declaration;
            }

            #endregion

            #region ValueInitialization

            var identifierParent = (VariableDeclaratorSyntax)v.Identifier.Parent!;
            var initializer = identifierParent.Initializer;
            var expressionValue = initializer?.Value as ExpressionSyntax;
            var literalValue = initializer?.Value as LiteralExpressionSyntax;
            var implicitNewCreation = initializer?.Value as ImplicitObjectCreationExpressionSyntax;
            var arrayCreationValue = initializer?.Value as ArrayCreationExpressionSyntax;

            if (literalValue != null)
            {
                valueInitialization = literalValue.Token.Value;

                if (valueInitialization == null)
                    valueInitializationDefaultNull = true;
            }
            else 
            {
                var castExpressionSyntax = initializer?.Value as CastExpressionSyntax;
                var createExpressionSyntax = initializer?.Value as ObjectCreationExpressionSyntax;

                if (castExpressionSyntax != null)
                {
                    var expressionCast = castExpressionSyntax.Expression;
                    var literalExpressionSyntax = expressionCast as LiteralExpressionSyntax;

                    castFunction = true;
                    valueInitialization = literalExpressionSyntax!.Token.Value;

                    if (valueInitialization == null)
                        valueInitializationDefaultNull = true;
                }

                if (createExpressionSyntax != null)
                {
                    var typeCreate = createExpressionSyntax.Type;
                    var genericNameCreate = typeCreate as GenericNameSyntax;
                    var identifierCreate = genericNameCreate?.Identifier;

                    if (identifierCreate!.Value.ValueText == "Collection" ||
                        identifierCreate.Value.ValueText == "List")
                    {
                        valueCreateInitialization = "JSON_ARRAY()";
                    }
                    else
                        throw new NotImplementedException($"Criação de declaração '{contentDeclaration}' não suportada");
                }
            }

            #endregion

            string[] linesDeclaration = contentDeclaration.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string[] linesVariable = contentVariable.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var typeName = linesDeclaration[0].TrimEnd('?');

            sb.Append($"DECLARE `{variableName}` ");

            if (typeName.StartsWith("List<") ||
                typeName.StartsWith("IList<") ||
                typeName.StartsWith("IEnumerable<") ||
                typeName.StartsWith("Enumerable<") ||
                typeName.StartsWith("Collection<") ||
                typeName.StartsWith("ICollection<") ||
                typeName.EndsWith("[]"))
            {
                if (implicitNewCreation != null || // = new()
                    arrayCreationValue != null)    // = new[] 
                {
                    valueCreateInitialization = "JSON_ARRAY()";
                }

                sb.Append("JSON");
            }
            else if (typeName == "object" || typeName == "Object" || typeName == "System.Object")
            {
                sb.Append("JSON");
            }
            else if (typeName == "char" || typeName == "Char" || typeName == "System.Char")
            {
                if (castFunction) // = (int)126
                {
                    if (valueInitialization is int)
                    {
                        openCastFunction = "CHAR(";
                        closeCastFunction = ")";
                    }
                    else
                        throw new NotImplementedException($"Tipo de conversão '{contentDeclaration}'não suportada");
                }

                sb.Append("CHAR");
            }
            else if (typeName == "bool" || typeName == "Boolean" || typeName == "System.Boolean")
            {
                sb.Append("BOOL");
            }
            else if (typeName == "sbyte" || typeName == "SByte" || typeName == "System.SByte")
            {
                sb.Append("TINYINT");
            }
            else if (typeName == "byte" || typeName == "Byte" || typeName == "System.Byte")
            {
                sb.Append("TINYINT UNSIGNED");
            }
            else if (typeName == "string" || typeName == "String" || typeName == "System.String")
            {
                sb.Append("VARCHAR(255)");
            }
            else if (typeName == "short" || typeName == "Int16" || typeName == "System.Int16")
            {
                sb.Append("SMALLINT");
            }
            else if (typeName == "ushort" || typeName == "UInt16" || typeName == "System.UInt16")
            {
                sb.Append("SMALLINT UNSIGNED");
            }
            else if (typeName == "int" || typeName == "Int32" || typeName == "System.Int32")
            {
                sb.Append("INT");
            }
            else if (typeName == "uint" || typeName == "UInt32" || typeName == "System.UInt32")
            {
                sb.Append("INT UNSIGNED");
            }
            else if (typeName == "long" || typeName == "Int64" || typeName == "System.Int64")
            {
                sb.Append("BIGINT");
            }
            else if (typeName == "ulong" || typeName == "UInt64" || typeName == "System.UInt64")
            {
                sb.Append("BIGINT UNSIGNED");
            }
            else if (typeName == "decimal" || typeName == "Decimal" || typeName == "System.Decimal")
            {
                sb.Append("DECIMAL");
            }
            else if (typeName == "double" || typeName == "Double" || typeName == "System.Double")
            {
                sb.Append("DOUBLE");
            }
            else if (typeName == "float" || typeName == "Single" || typeName == "System.Single")
            {
                sb.Append("FLOAT");
            }
            else
                throw new NotImplementedException($"Tipo de declaração '{contentDeclaration}' não suportada");

            if (valueInitializationDefaultNull)
            {
                sb.Append(" DEFAULT NULL");
            }
            else if (valueInitialization != null)
            {
                if (valueInitialization is string || valueInitialization is char)
                {
                    valueInitialization = $"\"{valueInitialization}\"";
                }

                sb.Append(
                    " DEFAULT "+
                    $"{openCastFunction}"+
                    $"{valueInitialization}"+
                    $"{closeCastFunction}");
            }
            else if (valueCreateInitialization != null)
            {
                sb.Append($" DEFAULT {valueCreateInitialization}");
            }

            return sb.ToString();
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