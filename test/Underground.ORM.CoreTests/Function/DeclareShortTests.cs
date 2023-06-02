using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Underground.ORM.Core;
using Urderground.ORM.Core.Attributes;

namespace Urderground.ORM.CoreTests.Function
{
    [TestClass()]
    public class DeclareShortTests
    {
        private readonly OrmEngine _orm;

        public DeclareShortTests() 
        {
            _orm = OrmEngineTests.OrmEngine.Value;
        }

        [TestMethod()]
        public async Task DeclareShortTest()
        {
            var function = _orm.BuildFunctionCreateStatement<short, short, short>(DeclareShortFunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareShortFunctionTest((short)10, (short)30);
            var resultMysql = await _orm.RunFunctionAsync(DeclareShortFunctionTest, (short)10, (short)30);
            
            Assert.AreEqual(resultCSharp, resultMysql);
        }

        [TestMethod()]
        public async Task DeclareUShortTest()
        {
            var function = _orm.BuildFunctionCreateStatement<ushort, ushort, ushort>(DeclareUShortFunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareUShortFunctionTest((ushort)10, (ushort)30);
            var resultMysql = await _orm.RunFunctionAsync(DeclareUShortFunctionTest, (ushort)10, (ushort)30);

            Assert.AreEqual(resultCSharp, resultMysql);
        }

#pragma warning disable CS0219,CS0168,IDE0059,IDE0004,IDE0047

        [MySqlFunctionScope(nameof(DeclareShortFunctionTest))]
        private short DeclareShortFunctionTest(short a, short b)
        {
            // Simples
            short var1;
            short? var2 = null;
            short var3 = 1;

            // Usando expressões
            short var4 = (short)(1 + a);
            short var5 = (short)(1 + a + b);
            short var6 = (short)((1 + (a - b)) * 3);

            // Usando múltiplas variáveis
            short var7, var8, var9 = 4;
            short var10 = 1, var11 = 2, var12 = 3;
            short? var13 = 4, var14 = 5, var15 = null;
            short? var16 = null, var17, var18 = 9;
            short var19 = a, var20 = b;

            // Usando conversões cast
            short var21 = (short)1;
            short var22 = (short)1 + (short)2;
            short var23 = (short)1 + ((short)2 - ((short)3 + 5) - (short)3);

            // Usando conversões cast estranhas
            short var24 = (short)(((5)));
            short var25 = (short)(1 + 2 + 3);
            short var26 = (short)(((1 + 2 + 3))) - 2;
            short var27 = (short)(((1 + (2) + ((3))))) - ((short)((2)));

            short result = (short)(var3 + var4 + var5 + var6 + var20);

            return result;
        }

        [MySqlFunctionScope(nameof(DeclareUShortFunctionTest))]
        private ushort DeclareUShortFunctionTest(ushort a, ushort b)
        {
            // Simples
            ushort var1;
            ushort? var2 = null;
            ushort var3 = 1;

            // Usando expressões
            ushort var4 = (ushort)(1 + a);
            ushort var5 = (ushort)(1 + a + b);
            ushort var6 = (ushort)((1 + (a + b)) * 3);

            // Usando múltiplas variáveis
            ushort var7, var8, var9 = 4;
            ushort var10 = 1, var11 = 2, var12 = 3;
            ushort? var13 = 4, var14 = 5, var15 = null;
            ushort? var16 = null, var17, var18 = 9;
            ushort var19 = a, var20 = b;

            // Usando conversões cast
            ushort var21 = (ushort)1;
            ushort var22 = (ushort)1 + (ushort)2;
            ushort var23 = (ushort)1 + ((ushort)20 + ((ushort)3 + 5) + (ushort)3);

            // Usando conversões cast estranhas
            ushort var24 = (ushort)(((5)));
            ushort var25 = (ushort)(1 + 2 + 3);
            ushort var26 = (ushort)(((1 + 2 + 3))) - 2;
            ushort var27 = (ushort)(((1 + (2) + ((3))))) + ((ushort)((2)));

            ushort result = (ushort)(var3 + var4 + var5 + var6 + var20);

            return result;
        }

#pragma warning restore CS0219,CS0168,IDE0059,IDE0004,IDE0047
    }
}