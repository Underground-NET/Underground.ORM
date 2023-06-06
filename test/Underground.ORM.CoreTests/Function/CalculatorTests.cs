using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Underground.ORM.Core;
using Underground.ORM.Core.Attributes;

namespace Underground.ORM.CoreTests.Function
{
    [TestClass()]
    public class CalculatorTests
    {
        private readonly OrmEngine _orm;

        public CalculatorTests()
        {
            _orm = OrmEngineTests.OrmEngine.Value;
        }

        [TestMethod()]
        public async Task SumTest()
        {
            var function = _orm.BuildFunctionCreateStatement<int, int, int>(SumFunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var somaCSharp = SumFunctionTest(10, 5);
            var somaMySql = await _orm.RunFunctionAsync(SumFunctionTest, 10, 5);

            Assert.AreEqual(somaMySql, somaCSharp);
        }

        [MySqlFunctionScope(nameof(SumFunctionTest))]
        private int SumFunctionTest(int a, int b)
        {
            return a + b;
        }

        [TestMethod()]
        public async Task SubtractTest()
        {
            var function = _orm.BuildFunctionCreateStatement<int, int, int>(SubtractFunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var subtracaoCSharp = SubtractFunctionTest(10, 5);
            var subtracaoMySql = await _orm.RunFunctionAsync(SubtractFunctionTest, 10, 5);

            Assert.AreEqual(subtracaoMySql, subtracaoCSharp);
        }

        [MySqlFunctionScope(nameof(SubtractFunctionTest))]
        private int SubtractFunctionTest(int a, int b)
        {
            return a - b;
        }
    }
}