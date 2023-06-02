using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Underground.ORM.Core;
using Urderground.ORM.Core.Attributes;

namespace Urderground.ORM.CoreTests.Function
{
    [TestClass()]
    public class DeclareIntTests
    {
        private readonly OrmEngine _orm;

        public DeclareIntTests() 
        {
            _orm = OrmEngineTests.OrmEngine.Value;
        }

        [TestMethod()]
        public async Task DeclareIntTest()
        {
            var function = _orm.BuildFunctionCreateStatement<int, int, int>(DeclareIntFunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareIntFunctionTest(10, 30);
            var resultMysql = await _orm.RunFunctionAsync(DeclareIntFunctionTest, 10, 30);
            
            Assert.AreEqual(resultCSharp, resultMysql);
        }

        [TestMethod()]
        public async Task DeclareUIntTest()
        {
            var function = _orm.BuildFunctionCreateStatement<uint, uint, uint>(DeclareUIntFunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareUIntFunctionTest(10U, 30U);
            var resultMysql = await _orm.RunFunctionAsync(DeclareUIntFunctionTest, 10U, 30U);

            Assert.AreEqual(resultCSharp, resultMysql);
        }

#pragma warning disable CS0219,CS0168,IDE0059,IDE0004,IDE0047

        [MySqlFunctionScope(nameof(DeclareIntFunctionTest))]
        private int DeclareIntFunctionTest(int a, int b)
        {
            // Simples
            int var1;
            int? var2 = null;
            int var3 = 1;

            // Usando expressões
            int var4 = 1 + a;
            int var5 = 1 + a + b;
            int var6 = (1 + (a - b)) * 3;

            // Usando múltiplas variáveis
            int var7, var8, var9 = 4;
            int var10 = 1, var11 = 2, var12 = 3;
            int? var13 = 4, var14 = 5, var15 = null;
            int? var16 = null, var17, var18 = 9;
            int var19 = a, var20 = b;

            // Usando conversões cast
            int var21 = (int)1;
            int var22 = (int)1 + (int)2;
            int var23 = (int)1 + ((int)2 - ((int)3 + 5) - (int)3);

            // Usando conversões cast estranhas
            int var24 = (int)(((5)));
            int var25 = (int)(1 + 2 + 3);
            int var26 = (int)(((1 + 2 + 3))) - 2;
            int var27 = (int)(((1 + (2) + ((3))))) - ((int)((2)));

            int result = var3 + var4 + var5 + var6 + var20;

            return result;
        }

        [MySqlFunctionScope(nameof(DeclareUIntFunctionTest))]
        private uint DeclareUIntFunctionTest(uint a, uint b)
        {
            // Simples
            uint var1;
            uint? var2 = null;
            uint var3 = 1;

            // Usando expressões
            uint var4 = 1 + a;
            uint var5 = 1 + a + b;
            uint var6 = (1 + (a + b)) * 3;

            // Usando múltiplas variáveis
            uint var7, var8, var9 = 4;
            uint var10 = 1, var11 = 2, var12 = 3;
            uint? var13 = 4, var14 = 5, var15 = null;
            uint? var16 = null, var17, var18 = 9;
            uint var19 = a, var20 = b;

            // Usando conversões cast
            uint var21 = (uint)1;
            uint var22 = (uint)1 + (uint)2;
            uint var23 = (uint)1 + ((uint)20 + ((uint)3 + 5) + (uint)3);

            // Usando conversões cast estranhas
            uint var24 = (uint)(((5)));
            uint var25 = (uint)(1 + 2 + 3);
            uint var26 = (uint)(((1 + 2 + 3))) - 2;
            uint var27 = (uint)(((1 + (2) + ((3))))) + ((uint)((2)));

            uint result = var3 + var4 + var5 + var6 + var20;

            return result;
        }

#pragma warning restore CS0219,CS0168,IDE0059,IDE0004,IDE0047
    }
}