using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Underground.ORM.Core;
using Underground.ORM.Core.Attributes;

namespace Underground.ORM.CoreTests.Iteration
{
    [TestClass()]
    public class IterationTests
    {
        private readonly OrmEngine _orm;

        public IterationTests()
        {
            _orm = OrmEngineTests.OrmEngine.Value;
        }

        [TestMethod()]
        public async Task ForIterationTest()
        {
            var function = _orm.BuildFunctionCreateStatement<int, int, int>(ForIterationTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = ForIterationTest(10, 50);
            var resultMySql = await _orm.RunFunctionAsync(ForIterationTest, 10, 50);

            Assert.AreEqual(resultMySql, resultCSharp);
        }

        [MySqlFunctionScope(nameof(ForIterationTest))]
        private int ForIterationTest(int start, int length)
        {
            int count = 0;

            // Simple
            for (int i = 0; i < 100; i++)
            {
                count++;
            }

            // With declaration
            for (int i = start; i < length; i++)
            {
                count++;
            }

            return count;
        }
    }
}