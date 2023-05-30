using Microsoft.CodeAnalysis;
using MySqlConnector;
using MySqlOrm.Core.Attributes;
using MySqlOrm.Core.Translator;
using System.Data;
using System.Reflection;

namespace MySqlOrm.Core.Internals
{
    public partial class MySqlOrmEngine
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
            MySqlTranslator translator = new();

            var functionAttribute = method.GetCustomAttribute<OrmFunctionScopeAttribute>();

            if (functionAttribute == null)
            {
                throw new NotImplementedException($"Atributo '{nameof(OrmFunctionScopeAttribute)}' não definido para este método");
            }

            var returnType = method.ReturnType;
            var returnDbType = translator.GetReturnDbType(returnType);

            #region ParametersIn

            var parameters = method.GetParameters();

            var parametersIn = parameters
                .Select(x => new ParameterDbType(x.Name,
                                                            translator.GetDbType(x.ParameterType),
                                                            x.ParameterType)).ToList();

            #endregion


            string createStatement = translator.TranslateToPlMySql(method,
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
    }
}