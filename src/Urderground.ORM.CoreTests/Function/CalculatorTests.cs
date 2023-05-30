using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Urderground.ORM.Core;
using Urderground.ORM.Core.Attributes;

namespace Urderground.ORM.CoreTests.Function
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
        public async Task SomarTest()
        {
            #region Soma

            var function = _orm.BuildFunctionCreateStatement<int, int, int>(FuncaoSomarTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var somaCSharp = FuncaoSomarTest(10, 5);
            var somaMySql = await _orm.RunFunctionAsync(FuncaoSomarTest, 10, 5);
            
            Assert.AreEqual(somaMySql, somaCSharp);

            #endregion

            #region Subtração

            function = _orm.BuildFunctionCreateStatement<int, int, int>(FuncaoSubtrairTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var subtracaoCSharp = FuncaoSubtrairTest(10, 5);
            var subtracaoMySql = await _orm.RunFunctionAsync(FuncaoSubtrairTest, 10, 5);
            
            Assert.AreEqual(subtracaoMySql, subtracaoCSharp);

            #endregion
        }

        [MySqlFunctionScope(nameof(FuncaoSomarTest))]
        private int FuncaoSomarTest(int a, int b)
        {
            return a + b;
        }

        [MySqlFunctionScope(nameof(FuncaoSubtrairTest))]
        private int FuncaoSubtrairTest(int a, int b)
        {
            return a - b;
        }
    }
}