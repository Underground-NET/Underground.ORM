using MySqlConnector;

namespace MySqlOrm
{
    public class MySqlOrmEngine
    {
        private MySqlConnection _connection;

        public MySqlOrmEngine()
        {
            
        }

        public async Task<bool> ConnectAsync(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
            await _connection.OpenAsync();
        }
    }
}