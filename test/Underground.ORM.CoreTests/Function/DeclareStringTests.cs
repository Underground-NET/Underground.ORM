using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Underground.ORM.Core;
using Underground.ORM.Core.Attributes;

namespace Underground.ORM.CoreTests.Function
{
    [TestClass()]
    public class DeclareStringTests
    {
        private readonly OrmEngine _orm;

        public DeclareStringTests()
        {
            _orm = OrmEngineTests.OrmEngine.Value;
        }

        #region string

        [TestMethod()]
        public async Task DeclareStringTest()
        {
            var function = _orm.BuildFunctionCreateStatement<string, string, string>(DeclareStringFunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareStringFunctionTest("string a", "string b");
            var resultMysql = await _orm.RunFunctionAsync(DeclareStringFunctionTest, "string a", "string b");

            Assert.AreEqual(resultCSharp, resultMysql);
        }

#pragma warning disable CS0219,CS0168,IDE0059,IDE0004,IDE0047,CS8600

        [MySqlFunctionScope(nameof(DeclareStringFunctionTest))]
        private string DeclareStringFunctionTest(string a, string b)
        {
            string var1;
            string var2 = null;
            string? var3 = null;
            string var4 = "";
            string var5 = "string tests";

            return var2 + var3 + var4 + var5 + " " + a + " " + b;
        }

#pragma warning restore CS0219,CS0168,IDE0059,IDE0004,IDE0047,CS8600

        #endregion
    }
}