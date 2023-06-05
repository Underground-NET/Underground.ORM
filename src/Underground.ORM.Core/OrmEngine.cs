using MySqlConnector;
using System.Data;
using System.Reflection;
using Underground.ORM.Core.Attributes;
using Underground.ORM.Core.Entity;
using Underground.ORM.Core.Translator.Syntax;
using Urderground.ORM.Core.Translator;

namespace Underground.ORM.Core
{
    public partial class OrmEngine
    {
        private static readonly List<(MethodInfo, MySqlFunctionScopeAttribute)> Functions = new();
        private static readonly List<(MethodInfo, MySqlProcedureScopeAttribute)> Procedures = new();

        private MySqlConnection _connection;
        private string _connectionString;

        private bool _ensureCreateDatabase;

        public bool EnsureCreateDatabase { get => _ensureCreateDatabase; set => _ensureCreateDatabase = value; }

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

        public async Task ConnectAsync(CancellationToken ct = default)
        {
            string? ensureDataBase = null;

        Retry:
            _connection = new MySqlConnection(_connectionString);

            try
            {
                await _connection.OpenAsync(ct);
            }
            catch (MySqlException ex)
            {
                #region UnknownDatabase
                if (_ensureCreateDatabase && ex.ErrorCode == MySqlErrorCode.UnknownDatabase)
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
                    var command = await GetCommandAsync(ct);
                    command.CommandText = $"CREATE DATABASE `{ensureDataBase}`";
                    await command.ExecuteNonQueryAsync(ct);
                }

                var sb = new MySqlConnectionStringBuilder(_connectionString)
                {
                    Database = ensureDataBase
                };

                _connectionString = sb.ToString();
                await ConnectAsync(ct);
            }
        }

        public async Task EnsureConnectedAync(CancellationToken ct)
        {
            if (_connection == null || _connection.State == ConnectionState.Closed)
            {
                await ConnectAsync(ct);
            }
        }

        public async Task<MySqlCommand> GetCommandAsync(CancellationToken ct = default)
        {
            await EnsureConnectedAync(ct);
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

        #region RunFunction

        public Task<TReturn?> RunFunctionAsync<TReturn>(Func<TReturn> function, CancellationToken ct = default)
        {
            return RunFunctionAsync<TReturn?>(function, function.GetMethodInfo(), Array.Empty<object?>(), ct);
        }

        public Task<TReturn?> RunFunctionAsync<T1, TReturn>(Func<T1, TReturn> function, T1 arg1, CancellationToken ct = default)
        {
            return RunFunctionAsync<TReturn?>(function, function.GetMethodInfo(), new object?[] { arg1 }, ct);
        }

        public Task<TReturn?> RunFunctionAsync<T1, T2, TReturn>(Func<T1, T2, TReturn> function, T1 arg1, T2 arg2, CancellationToken ct = default)
        {
            return RunFunctionAsync<TReturn?>(function, function.GetMethodInfo(), new object?[] { arg1, arg2 }, ct);
        }

        public Task<TReturn?> RunFunctionAsync<T1, T2, T3, TReturn>(Func<T1, T2, T3, TReturn> function, T1 arg1, T2 arg2, T3 arg3, CancellationToken ct = default)
        {
            return RunFunctionAsync<TReturn?>(function, function.GetMethodInfo(), new object?[] { arg1, arg2, arg3 }, ct);
        }

        public Task<TReturn?> RunFunctionAsync<T1, T2, T3, T4, TReturn>(Func<T1, T2, T3, T4, TReturn> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, CancellationToken ct = default)
        {
            return RunFunctionAsync<TReturn?>(function, function.GetMethodInfo(), new object?[] { arg1, arg2, arg3, arg4 }, ct);
        }

        public Task<TReturn?> RunFunctionAsync<T1, T2, T3, T4, T5, TReturn>(Func<T1, T2, T3, T4, T5, TReturn> function, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, CancellationToken ct = default)
        {
            return RunFunctionAsync<TReturn?>(function, function.GetMethodInfo(), new object?[] { arg1, arg2, arg3, arg4, arg5 }, ct);
        }


        private async Task<TReturn?> RunFunctionAsync<TReturn>(object function,
                                                               MethodInfo method,
                                                               object?[] values,
                                                               CancellationToken ct)
        {
            var functionAttribute = method.GetCustomAttribute<MySqlFunctionScopeAttribute>();

            if (functionAttribute != null)
            {
                await EnsureConnectedAync(ct);

                var command = await GetCommandAsync(ct);

                var parameters = values.Select((x, i) => new MySqlParameter($"arg{i + 1}", x)).ToArray();
                command.Parameters.AddRange(parameters);

                command.CommandText =
                    $"SELECT `{_connection.Database}`.`{functionAttribute.RoutineName}`" +
                    $"({string.Join(",", parameters.Select(x => $"@{x.ParameterName}"))})";

                return (TReturn?)await command.ExecuteScalarAsync(ct);
            }

            throw new Exception($"Scope attribute not set for method '{method.Name}'");
        }

        #endregion

        #region BuildCreateFunctionStatement

        public MySqlSyntaxBuilt BuildCreateFunctionStatement<TReturn>(Func<TReturn> function)
        {
            return BuildFunctionCreateStatement(function.GetMethodInfo());
        }

        public MySqlSyntaxBuilt BuildCreateFunctionStatement<T1, TReturn>(Func<T1, TReturn> function)
        {
            return BuildFunctionCreateStatement(function.GetMethodInfo());
        }

        public MySqlSyntaxBuilt BuildFunctionCreateStatement<T1, T2, TReturn>(Func<T1, T2, TReturn> function)
        {
            return BuildFunctionCreateStatement(function.GetMethodInfo());
        }

        public MySqlSyntaxBuilt BuildCreateFunctionStatement<T1, T2, T3, TReturn>(Func<T1, T2, T3, TReturn> function)
        {
            return BuildFunctionCreateStatement(function.GetMethodInfo());
        }

        public MySqlSyntaxBuilt BuildCreateFunctionStatement<T1, T2, T3, T4, TReturn>(Func<T1, T2, T3, T4, TReturn> function)
        {
            return BuildFunctionCreateStatement(function.GetMethodInfo());
        }

        public MySqlSyntaxBuilt BuildCreateFunctionStatement<T1, T2, T3, T4, T5, TReturn>(Func<T1, T2, T3, T4, T5, TReturn> function)
        {
            return BuildFunctionCreateStatement(function.GetMethodInfo());
        }

        private MySqlSyntaxBuilt BuildFunctionCreateStatement(MethodInfo method)
        {
            MySqlTranslator translator = new();

            var mysqlSyntaxBuilt = translator.TranslateToFunctionCreateStatementSyntax(method);

            return mysqlSyntaxBuilt;
        }

        #endregion

        public async Task<int> UpdateDatabaseAsync(MySqlSyntaxBuilt mysqlSyntax, CancellationToken ct = default)
        {
            var functionAttribute = mysqlSyntax.Method.GetCustomAttribute<MySqlFunctionScopeAttribute>();

            await EnsureConnectedAync(ct);

            if (functionAttribute != null)
            {
                var command = await GetCommandAsync(ct);
            Retry:

                command.CommandType = CommandType.Text;
                command.CommandText = mysqlSyntax.Statement;

                try
                {
                    return await command.ExecuteNonQueryAsync(ct);
                }
                catch (MySqlException ex)
                {
                    if (ex.ErrorCode == MySqlErrorCode.StoredProcedureAlreadyExists)
                    {
                        command.CommandText = $"DROP FUNCTION `{_connection.Database}`.`{mysqlSyntax.RoutineName}`";
                        await command.ExecuteNonQueryAsync(ct);
                        goto Retry;
                    }

                    throw;
                }
            }

            throw new Exception($"Scope attribute not set for method '{mysqlSyntax.Method.Name}'");
        }
    }
}