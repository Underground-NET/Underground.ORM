using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Underground.ORM.Core;
using Urderground.ORM.Core.Attributes;

namespace Urderground.ORM.CoreTests.Function
{
    [TestClass()]
    public class DeclareLongTests
    {
        private readonly OrmEngine _orm;

        public DeclareLongTests() 
        {
            _orm = OrmEngineTests.OrmEngine.Value;
        }

        [TestMethod()]
        public async Task DeclareLongTest()
        {
            var function = _orm.BuildFunctionCreateStatement<long, long, long>(DeclareLongFunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareLongFunctionTest(10L, 30L);
            var resultMysql = await _orm.RunFunctionAsync(DeclareLongFunctionTest, 10L, 30L);
            
            Assert.AreEqual(resultCSharp, resultMysql);
        }

        [TestMethod()]
        public async Task DeclareULongTest()
        {
            var function = _orm.BuildFunctionCreateStatement<ulong, ulong, ulong>(DeclareULongFunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareULongFunctionTest(10LU, 30LU);
            var resultMysql = await _orm.RunFunctionAsync(DeclareULongFunctionTest, 10LU, 30LU);

            Assert.AreEqual(resultCSharp, resultMysql);
        }

#pragma warning disable CS0219,CS0168,IDE0059,IDE0004,IDE0047

        [MySqlFunctionScope(nameof(DeclareLongFunctionTest))]
        private long DeclareLongFunctionTest(long a, long b)
        {
            // Simples
            long var1;
            long? var2 = null;
            long var3 = 1;

            // Usando expressões
            long var4 = 1 + a;
            long var5 = 1 + a + b;
            long var6 = (1 + (a - b)) * 3;

            // Usando múltiplas variáveis
            long var7, var8, var9 = 4;
            long var10 = 1, var11 = 2, var12 = 3;
            long? var13 = 4, var14 = 5, var15 = null;
            long? var16 = null, var17, var18 = 9;
            long var19 = a, var20 = b;

            // Usando conversões cast
            long var21 = (long)1;
            long var22 = (long)1 + (long)2;
            long var23 = (long)1 + ((long)2 - ((long)3 + 5) - (long)3);

            // Usando conversões cast estranhas
            long var24 = (long)(((5)));
            long var25 = (long)(1 + 2 + 3);
            long var26 = (long)(((1 + 2 + 3))) - 2;
            long var27 = (long)(((1 + (2) + ((3))))) - ((long)((2)));

            long result = var3 + var4 + var5 + var6 + var20;

            return result;
        }

        [MySqlFunctionScope(nameof(DeclareULongFunctionTest))]
        private ulong DeclareULongFunctionTest(ulong a, ulong b)
        {
            // Simples
            ulong var1;
            ulong? var2 = null;
            ulong var3 = 1;

            // Usando expressões
            ulong var4 = 1 + a;
            ulong var5 = 1 + a + b;
            ulong var6 = (1 + (a + b)) * 3;

            // Usando múltiplas variáveis
            ulong var7, var8, var9 = 4;
            ulong var10 = 1, var11 = 2, var12 = 3;
            ulong? var13 = 4, var14 = 5, var15 = null;
            ulong? var16 = null, var17, var18 = 9;
            ulong var19 = a, var20 = b;

            // Usando conversões cast
            ulong var21 = (ulong)1;
            ulong var22 = (ulong)1 + (ulong)2;
            ulong var23 = (ulong)1 + ((ulong)20 + ((ulong)3 + 5) + (ulong)3);

            // Usando conversões cast estranhas
            ulong var24 = (ulong)(((5)));
            ulong var25 = (ulong)(1 + 2 + 3);
            ulong var26 = (ulong)(((1 + 2 + 3))) - 2;
            ulong var27 = (ulong)(((1 + (2) + ((3))))) + ((ulong)((2)));

            ulong result = var3 + var4 + var5 + var6 + var20;

            return result;
        }

#pragma warning restore CS0219,CS0168,IDE0059,IDE0004,IDE0047
    }
}