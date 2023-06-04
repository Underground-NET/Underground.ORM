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

        #region Int16

        [TestMethod()]
        public async Task DeclareInt16Test()
        {
            var function = _orm.BuildFunctionCreateStatement<Int16, Int16, Int16>(DeclareInt16FunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareInt16FunctionTest(10, 30);
            var resultMysql = await _orm.RunFunctionAsync(DeclareInt16FunctionTest, (Int16)10, (Int16)30);

            Assert.AreEqual(resultCSharp, resultMysql);
        }

#pragma warning disable CS0219, CS0168, IDE0059, IDE0004, IDE0047

        [MySqlFunctionScope(nameof(DeclareInt16FunctionTest))]
        private Int16 DeclareInt16FunctionTest(Int16 a, Int16 b)
        {
            Int16 var1;
            Int16? var2 = null;
            Int16 var3 = 1;

            Int16 var4 = (Int16)(1 + a);
            Int16 var5 = (Int16)(1 + a + b);
            Int16 var6 = (Int16)((1 + (a - b)) * 3);

            Int16 var7, var8, var9 = 4;
            Int16 var10 = 1, var11 = 2, var12 = 3;
            Int16? var13 = 4, var14 = 5, var15 = null;
            Int16? var16 = null, var17, var18 = 9;
            Int16 var19 = a, var20 = b;

            Int16 var21 = 1;
            Int16 var22 = 1 + 2;
            Int16 var23 = 1 + (2 - (3 + 5) - 3);

            Int16 var24 = 5;
            Int16 var25 = 1 + 2 + 3;
            Int16 var26 = 1 + 2 + 3 - 2;
            Int16 var27 = 1 + 2 + 3 - 2;

            Int16 result = (Int16)(var3 + var4 + var5 + var6 + var20);

            return result;
        }

#pragma warning restore CS0219,CS0168,IDE0059,IDE0004,IDE0047

        #endregion

        #region System.Int16

        [TestMethod()]
        public async Task DeclareSystemInt16Test()
        {
            var function = _orm.BuildFunctionCreateStatement<System.Int16, System.Int16, System.Int16>(DeclareSystemInt16FunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareSystemInt16FunctionTest(10, 30);
            var resultMysql = await _orm.RunFunctionAsync(DeclareSystemInt16FunctionTest, (System.Int16)10, (System.Int16)30);

            Assert.AreEqual(resultCSharp, resultMysql);
        }

#pragma warning disable CS0219, CS0168, IDE0059, IDE0004, IDE0047

        [MySqlFunctionScope(nameof(DeclareSystemInt16FunctionTest))]
        private System.Int16 DeclareSystemInt16FunctionTest(System.Int16 a, System.Int16 b)
        {
            System.Int16 var1;
            System.Int16? var2 = null;
            System.Int16 var3 = 1;

            System.Int16 var4 = (System.Int16)(1 + a);
            System.Int16 var5 = (System.Int16)(1 + a + b);
            System.Int16 var6 = (System.Int16)((1 + (a - b)) * 3);

            System.Int16 var7, var8, var9 = 4;
            System.Int16 var10 = 1, var11 = 2, var12 = 3;
            System.Int16? var13 = 4, var14 = 5, var15 = null;
            System.Int16? var16 = null, var17, var18 = 9;
            System.Int16 var19 = a, var20 = b;

            System.Int16 var21 = 1;
            System.Int16 var22 = 1 + 2;
            System.Int16 var23 = 1 + (2 - (3 + 5) - 3);

            System.Int16 var24 = 5;
            System.Int16 var25 = 1 + 2 + 3;
            System.Int16 var26 = 1 + 2 + 3 - 2;
            System.Int16 var27 = 1 + 2 + 3 - 2;

            System.Int16 result = (System.Int16)(var3 + var4 + var5 + var6 + var20);

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

        #region UInt16

        [TestMethod()]
        public async Task DeclareUInt16Test()
        {
            var function = _orm.BuildFunctionCreateStatement<UInt16, UInt16, UInt16>(DeclareUInt16FunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareUInt16FunctionTest(10, 30);
            var resultMysql = await _orm.RunFunctionAsync(DeclareUInt16FunctionTest, (UInt16)10, (UInt16)30);

            Assert.AreEqual(resultCSharp, resultMysql);
        }

#pragma warning disable CS0219, CS0168, IDE0059, IDE0004, IDE0047

        [MySqlFunctionScope(nameof(DeclareUInt16FunctionTest))]
        private UInt16 DeclareUInt16FunctionTest(UInt16 a, UInt16 b)
        {
            UInt16 var1;
            UInt16? var2 = null;
            UInt16 var3 = 1;

            UInt16 var4 = (UInt16)(1 + a);
            UInt16 var5 = (UInt16)(1 + a + b);
            UInt16 var6 = (UInt16)((1 + a + b) * 3);

            UInt16 var7, var8, var9 = 4;
            UInt16 var10 = 1, var11 = 2, var12 = 3;
            UInt16? var13 = 4, var14 = 5, var15 = null;
            UInt16? var16 = null, var17, var18 = 9;
            UInt16 var19 = a, var20 = b;

            UInt16 var21 = 1;
            UInt16 var22 = 1 + 2;
            UInt16 var23 = 1 + 20 + 3 + 5 + 3;

            UInt16 var24 = 5;
            UInt16 var25 = 1 + 2 + 3;
            UInt16 var26 = 1 + 2 + 3 - 2;
            UInt16 var27 = 1 + 2 + 3 + 2;

            UInt16 result = (UInt16)(var3 + var4 + var5 + var6 + var20);

            return result;
        }

#pragma warning restore CS0219, CS0168, IDE0059, IDE0004, IDE0047

        #endregion

        #region System.UInt16

        [TestMethod()]
        public async Task DeclareSystemUInt16Test()
        {
            var function = _orm.BuildFunctionCreateStatement<System.UInt16, System.UInt16, System.UInt16>(DeclareSystemUInt16FunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareSystemUInt16FunctionTest(10, 30);
            var resultMysql = await _orm.RunFunctionAsync(DeclareSystemUInt16FunctionTest, (System.UInt16)10, (System.UInt16)30);

            Assert.AreEqual(resultCSharp, resultMysql);
        }

#pragma warning disable CS0219, CS0168, IDE0059, IDE0004, IDE0047

        [MySqlFunctionScope(nameof(DeclareSystemUInt16FunctionTest))]
        private System.UInt16 DeclareSystemUInt16FunctionTest(System.UInt16 a, System.UInt16 b)
        {
            System.UInt16 var1;
            System.UInt16? var2 = null;
            System.UInt16 var3 = 1;

            System.UInt16 var4 = (System.UInt16)(1 + a);
            System.UInt16 var5 = (System.UInt16)(1 + a + b);
            System.UInt16 var6 = (System.UInt16)((1 + a + b) * 3);

            System.UInt16 var7, var8, var9 = 4;
            System.UInt16 var10 = 1, var11 = 2, var12 = 3;
            System.UInt16? var13 = 4, var14 = 5, var15 = null;
            System.UInt16? var16 = null, var17, var18 = 9;
            System.UInt16 var19 = a, var20 = b;

            System.UInt16 var21 = 1;
            System.UInt16 var22 = 1 + 2;
            System.UInt16 var23 = 1 + 20 + 3 + 5 + 3;

            System.UInt16 var24 = 5;
            System.UInt16 var25 = 1 + 2 + 3;
            System.UInt16 var26 = 1 + 2 + 3 - 2;
            System.UInt16 var27 = 1 + 2 + 3 + 2;

            System.UInt16 result = (System.UInt16)(var3 + var4 + var5 + var6 + var20);

            return result;
        }

#pragma warning restore CS0219, CS0168, IDE0059, IDE0004, IDE0047

        #endregion
    }
}