using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MySqlConnector;
using MySqlOrm.Core.Attributes;
using System.Collections.ObjectModel;
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

        public void RunFunction<T1>(Func<T1> function)
        {
            var expression = (Expression<Func<T1>>)(() => function());
            RunFunctionInternal(function, function.GetMethodInfo(), expression, Array.Empty<object?>());
        }

        public void RunFunction<T1, T2>(Func<T1, T2> function, T1 arg1)
        {
            var expression = (Expression<Func<T1, T2>>)((arg1) => function(arg1));
            RunFunctionInternal(function, function.GetMethodInfo(), expression, new object?[] { arg1 });
        }

        public void RunFunction<T1, T2, T3>(Func<T1, T2, T3> function, T1 arg1, T2 arg2)
        {
            var expression = (Expression<Func<T1, T2, T3>>)((arg1, arg2) => function(arg1, arg2));
            RunFunctionInternal(function, function.GetMethodInfo(), expression, new object?[] { arg1, arg2 });
        }

        public void RunFunction<T1, T2, T3, T4>(Func<T1, T2, T3, T4> function, T1 arg1, T2 arg2, T3 arg3)
        {
            var expression = (Expression<Func<T1, T2, T3, T4>>)((arg1, arg2, arg3) => function(arg1, arg2, arg3));
            RunFunctionInternal(function, function.GetMethodInfo(), expression, new object?[] { arg1, arg2, arg3 });
        }

        public void RunFunction<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var expression = (Expression<Func<T1, T2, T3, T4, T5>>)((arg1, arg2, arg3, arg4) => function(arg1, arg2, arg3, arg4));
            RunFunctionInternal(function, function.GetMethodInfo(), expression, new object?[] { arg1, arg2, arg3, arg4 });
        }

        public void RunFunction<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var expression = (Expression<Func<T1, T2, T3, T4, T5, T6>>)((arg1, arg2, arg3, arg4, arg5) => function(arg1, arg2, arg3, arg4, arg5));
            RunFunctionInternal(function, function.GetMethodInfo(), expression, new object?[] { arg1, arg2, arg3, arg4, arg5 });
        }

        private void RunFunctionInternal(object function, 
                                         MethodInfo method, 
                                         Expression expression, 
                                         object?[] values)
        {
            var functionAttribute = method.GetCustomAttribute<OrmFunctionScopeAttribute>();

            if (functionAttribute == null)
            {
                throw new NotImplementedException($"Atributo '{nameof(OrmFunctionScopeAttribute)}' não definido para este método");
            }

            var returnType = (Type)((dynamic)expression).ReturnType;
            var returnDbType = GetReturnDbType(returnType);

            #region ParametersIn

            var parameters =
                (ReadOnlyCollection<ParameterExpression>)((dynamic)expression).Parameters;

            var parametersIn = parameters
                .Select(x => new ParameterDbType(x.Name,
                                                                GetDbType(x.Type), 
                                                                 x.Type)).ToList();

            #endregion

            string createStatement = TranslateToPlMySql(method,
                                                        functionAttribute,
                                                        returnDbType,
                                                        parametersIn, 
                                                        new(), 
                                                        new());

            var command = GetCommand();

            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = createStatement;
            var affected = command.ExecuteNonQuery();
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
                        sb.AppendLine("RETURN null;");
                        break;
                    }
                }
            }
            
            sb.AppendLine("END;");

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
            else
                throw new NotImplementedException();

            return dbType;
        }
    }
}