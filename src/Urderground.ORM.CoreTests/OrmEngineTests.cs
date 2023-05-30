using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using Urderground.ORM.Core;

namespace Urderground.ORM.CoreTests
{
    [TestClass()]
    public class OrmEngineTests
    {
        public static Lazy<OrmEngine> OrmEngine = new (() =>
        {
            MySqlConnectionStringBuilder sb = new();

            sb.Server = "localhost";
            sb.Port = 3306;
            sb.UserID = "root";
            sb.Password = "12345678";
            sb.Database = "underground_orm_tests";
            sb.AllowUserVariables = true;

            return new OrmEngine(sb) { EnsureCreateDatabase = true };
        });
    }
}