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

        public OrmEngine(string connectionString)
        {
            _connectionString = connectionString;
        }

        public OrmEngine(MySqlConnectionStringBuilder connectionString)
        {
            _connectionString = connectionString.ToString();
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

        public async Task ConnectAsync(bool ensureCreateDatabase = true)
        {
            string? ensureDataBase = null;

        Retry:
            _connection = new MySqlConnection(_connectionString);

            try
            {
                await _connection.OpenAsync();
            }
            catch (MySqlException ex)
            {
                #region UnknownDatabase
                if (ensureCreateDatabase && ex.ErrorCode == MySqlErrorCode.UnknownDatabase)
                {
                    ensureDataBase = _connection.Database;
                    await _connection.CloseAsync();

                    var sb = new MySqlConnectionStringBuilder(_connectionString)
                    {
                        Database = null
                    };
                    _connectionString = sb.ToString();
                    goto Retry;
                }
                #endregion

                throw;
            }

            if (ensureDataBase != null)
            {
                using (_connection)
                {
                    var command = await GetCommandAsync();
                    command.CommandText = $"CREATE DATABASE `{ensureDataBase}`";
                    await command.ExecuteNonQueryAsync();
                }

                var sb = new MySqlConnectionStringBuilder(_connectionString)
                {
                    Database = ensureDataBase
                };

                _connectionString = sb.ToString();
                await ConnectAsync(ensureCreateDatabase);
            }
        }

        public async Task EnsureConnectedAync()
        {
            if (_connection == null || _connection.State == ConnectionState.Closed)
            {
                await ConnectAsync();
            }
        }

        public async Task<MySqlCommand> GetCommandAsync()
        {
            await EnsureConnectedAync();
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

        public MySqlSyntax BuildCreateFunctionStatement<T1>(Func<T1> function)
        {
            return BuildFunctionCreateStatement(function, function.GetMethodInfo(), Array.Empty<object?>());
        }

        public MySqlSyntax BuildCreateFunctionStatement<T1, T2>(Func<T1, T2> function, T1 arg1)
        {
            return BuildFunctionCreateStatement(function, function.GetMethodInfo(), new object?[] { arg1 });
        }

        public MySqlSyntax BuildFunctionCreateStatement<T1, T2, T3>(Func<T1, T2, T3> function, T1 arg1, T2 arg2)
        {
            return BuildFunctionCreateStatement(function, function.GetMethodInfo(), new object?[] { arg1, arg2 });
        }

        public MySqlSyntax BuildCreateFunctionStatement<T1, T2, T3, T4>(Func<T1, T2, T3, T4> function, T1 arg1, T2 arg2, T3 arg3)
        {
            return BuildFunctionCreateStatement(function, function.GetMethodInfo(), new object?[] { arg1, arg2, arg3 });
        }

        public MySqlSyntax BuildCreateFunctionStatement<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return BuildFunctionCreateStatement(function, function.GetMethodInfo(), new object?[] { arg1, arg2, arg3, arg4 });
        }

        public MySqlSyntax BuildCreateFunctionStatement<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return BuildFunctionCreateStatement(function, function.GetMethodInfo(), new object?[] { arg1, arg2, arg3, arg4, arg5 });
        }

        private MySqlSyntax BuildFunctionCreateStatement(object function,
                                                         MethodInfo method,
                                                         object?[] values)
         {
            MySqlTranslator translator = new();

            var mysqlSyntax = translator.TranslateToFunctionCreateSyntax(method, values);

            return mysqlSyntax;
        }

        public async Task<int> UpdateDatabaseAsync(MySqlSyntax mysqlSyntax)
        {
            var functionAttribute = mysqlSyntax.Method.GetCustomAttribute<MySqlFunctionScopeAttribute>();

            await EnsureConnectedAync();

            if (functionAttribute != null)
            {
                var command = await GetCommandAsync();
            Retry:

                command.CommandType = CommandType.Text;
                command.CommandText = mysqlSyntax.Statement;

                try
                {
                    return command.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    if (ex.ErrorCode == MySqlErrorCode.StoredProcedureAlreadyExists)
                    {
                        command.CommandText = $"USE `{_connection.Database}`; DROP FUNCTION `{mysqlSyntax.RoutineName}`";
                        await command.ExecuteNonQueryAsync();
                        goto Retry;
                    }

                    throw;
                }
            }

            throw new NotImplementedException($"Atributo de escopo não definido para o método '{mysqlSyntax.Method.Name}'");
        }
    }
}