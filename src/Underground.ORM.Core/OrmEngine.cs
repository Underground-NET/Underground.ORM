using Microsoft.CodeAnalysis;
using MySqlConnector;
using System.Data;
using System.Reflection;
using Urderground.ORM.Core.Attributes;
using Urderground.ORM.Core.Entity;
using Urderground.ORM.Core.Translator;

namespace Urderground.ORM.Core
{
    public partial class OrmEngine
    {
        private static readonly List<(MethodInfo, MySqlFunctionScopeAttribute)> Functions = new();
        private static readonly List<(MethodInfo, MySqlProcedureScopeAttribute)> Procedures = new();

        private MySqlConnection _connection;
        private string _connectionString;

        public OrmEngine()
        {

        }

        public OrmEngine(string connectionString)
        {
            _connectionString = connectionString;
        }

        static OrmEngine()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var method in type.GetMethods())
                    {
                        var functionAttribute = method.GetCustomAttribute<MySqlFunctionScopeAttribute>();

                        if (functionAttribute != null)
                        {
                            Functions.Add((method, functionAttribute));
                        }

                        var procedureAttribute = method.GetCustomAttribute<MySqlProcedureScopeAttribute>();

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

        public void Insert<T>(T model) where T : OrmBaseEntity
        {

        }

        public void InsertIgnore<T>(T model) where T : OrmBaseEntity
        {

        }

        public void Update<T>(T model) where T : OrmBaseEntity
        {

        }

        public void UpdateOnDuplicateKey<T>(T model) where T : OrmBaseEntity
        {

        }

        public void Delete<T>(T model) where T : OrmBaseEntity
        {

        }

        public MySqlSyntax RunFunction<T1>(Func<T1> function)
        {
            return CreateFunctionInternal(function, function.GetMethodInfo(), Array.Empty<object?>());
        }

        public MySqlSyntax RunFunction<T1, T2>(Func<T1, T2> function, T1 arg1)
        {
            return CreateFunctionInternal(function, function.GetMethodInfo(), new object?[] { arg1 });
        }

        public MySqlSyntax CreateFunctionStatement<T1, T2, T3>(Func<T1, T2, T3> function, T1 arg1, T2 arg2)
        {
            return CreateFunctionInternal(function, function.GetMethodInfo(), new object?[] { arg1, arg2 });
        }

        public MySqlSyntax RunFunction<T1, T2, T3, T4>(Func<T1, T2, T3, T4> function, T1 arg1, T2 arg2, T3 arg3)
        {
            return CreateFunctionInternal(function, function.GetMethodInfo(), new object?[] { arg1, arg2, arg3 });
        }

        public MySqlSyntax RunFunction<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return CreateFunctionInternal(function, function.GetMethodInfo(), new object?[] { arg1, arg2, arg3, arg4 });
        }

        public MySqlSyntax RunFunction<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return CreateFunctionInternal(function, function.GetMethodInfo(), new object?[] { arg1, arg2, arg3, arg4, arg5 });
        }

        private MySqlSyntax CreateFunctionInternal(object function,
                                              MethodInfo method,
                                              object?[] values)
        {
            MySqlTranslator translator = new();

            var mysqlSyntax = translator.TranslateToMySqlSyntax(method, values);

            // TODO: Conectar no banco de dados e criar a função

            //var command = GetCommand();
            //command.CommandType = System.Data.CommandType.Text;
            //command.CommandText = createStatement;
            //var affected = command.ExecuteNonQuery();

            return mysqlSyntax;
        }
    }
}