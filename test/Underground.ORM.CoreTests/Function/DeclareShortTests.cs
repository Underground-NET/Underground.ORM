using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Underground.ORM.Core;
using Underground.ORM.Core.Attributes;

namespace Underground.ORM.CoreTests.Function
{
    [TestClass()]
    public class DeclareShortTests
    {
        private readonly OrmEngine _orm;

        public DeclareShortTests()
        {
            _orm = OrmEngineTests.OrmEngine.Value;
        }

        #region short

        [TestMethod()]
        public async Task DeclareShortTest()
        {
            var function = _orm.BuildFunctionCreateStatement<short, short, short>(DeclareShortFunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareShortFunctionTest(10, 30);
            var resultMysql = await _orm.RunFunctionAsync(DeclareShortFunctionTest, (short)10, (short)30);

            Assert.AreEqual(resultCSharp, resultMysql);
        }

#pragma warning disable CS0219, CS0168, IDE0059, IDE0004, IDE0047

        [MySqlFunctionScope(nameof(DeclareShortFunctionTest))]
        private short DeclareShortFunctionTest(short a, short b)
        {
            short var1;
            short? var2 = null;
            short var3 = 1;

            short var4 = (short)(1 + a);
            short var5 = (short)(1 + a + b);
            short var6 = (short)((1 + (a - b)) * 3);

            short var7, var8, var9 = 4;
            short var10 = 1, var11 = 2, var12 = 3;
            short? var13 = 4, var14 = 5, var15 = null;
            short? var16 = null, var17, var18 = 9;
            short var19 = a, var20 = b;

            short var21 = 1;
            short var22 = 1 + 2;
            short var23 = 1 + (2 - (3 + 5) - 3);

            short var24 = 5;
            short var25 = 1 + 2 + 3;
            short var26 = 1 + 2 + 3 - 2;
            short var27 = 1 + 2 + 3 - 2;

            short result = (short)(var3 + var4 + var5 + var6 + var20);

            return result;
        }

#pragma warning restore CS0219,CS0168,IDE0059,IDE0004,IDE0047

        #endregion

        #region ushort

        [TestMethod()]
        public async Task DeclareUShortTest()
        {
            var function = _orm.BuildFunctionCreateStatement<ushort, ushort, ushort>(DeclareUShortFunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareUShortFunctionTest(10, 30);
            var resultMysql = await _orm.RunFunctionAsync(DeclareUShortFunctionTest, (ushort)10, (ushort)30);

            Assert.AreEqual(resultCSharp, resultMysql);
        }

#pragma warning disable CS0219, CS0168, IDE0059, IDE0004, IDE0047

        [MySqlFunctionScope(nameof(DeclareUShortFunctionTest))]
        private ushort DeclareUShortFunctionTest(ushort a, ushort b)
        {
            ushort var1;
            ushort? var2 = null;
            ushort var3 = 1;

            ushort var4 = (ushort)(1 + a);
            ushort var5 = (ushort)(1 + a + b);
            ushort var6 = (ushort)((1 + a + b) * 3);

            ushort var7, var8, var9 = 4;
            ushort var10 = 1, var11 = 2, var12 = 3;
            ushort? var13 = 4, var14 = 5, var15 = null;
            ushort? var16 = null, var17, var18 = 9;
            ushort var19 = a, var20 = b;

            ushort var21 = 1;
            ushort var22 = 1 + 2;
            ushort var23 = 1 + 20 + 3 + 5 + 3;

            ushort var24 = 5;
            ushort var25 = 1 + 2 + 3;
            ushort var26 = 1 + 2 + 3 - 2;
            ushort var27 = 1 + 2 + 3 + 2;

            ushort result = (ushort)(var3 + var4 + var5 + var6 + var20);

            return result;
        }

#pragma warning restore CS0219, CS0168, IDE0059, IDE0004, IDE0047

        #endregion
    }
}