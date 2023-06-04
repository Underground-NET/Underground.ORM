using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Underground.ORM.Core;
using Underground.ORM.Core.Attributes;

namespace Underground.ORM.CoreTests.Function
{
    [TestClass()]
    public class DeclareLongTests
    {
        private readonly OrmEngine _orm;

        public DeclareLongTests()
        {
            _orm = OrmEngineTests.OrmEngine.Value;
        }

        #region long

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

#pragma warning disable CS0219, CS0168, IDE0059, IDE0004, IDE0047

        [MySqlFunctionScope(nameof(DeclareLongFunctionTest))]
        private long DeclareLongFunctionTest(long a, long b)
        {
            long var1;
            long? var2 = null;
            long var3 = 1;

            long var4 = 1 + a;
            long var5 = 1 + a + b;
            long var6 = (1 + (a - b)) * 3;

            long var7, var8, var9 = 4;
            long var10 = 1, var11 = 2, var12 = 3;
            long? var13 = 4, var14 = 5, var15 = null;
            long? var16 = null, var17, var18 = 9;
            long var19 = a, var20 = b;

            long var21 = 1;
            long var22 = 1 + (long)2;
            long var23 = 1 + (2 - ((long)3 + 5) - 3);

            long var24 = 5;
            long var25 = 1 + 2 + 3;
            long var26 = (long)(1 + 2 + 3) - 2;
            long var27 = 1 + 2 + 3 - (long)2;

            long result = var3 + var4 + var5 + var6 + var20;

            return result;
        }

#pragma warning restore CS0219,CS0168,IDE0059,IDE0004,IDE0047

        #endregion

        #region Int64

        [TestMethod()]
        public async Task DeclareInt64Test()
        {
            var function = _orm.BuildFunctionCreateStatement<Int64, Int64, Int64>(DeclareInt64FunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareInt64FunctionTest(10L, 30L);
            var resultMysql = await _orm.RunFunctionAsync(DeclareInt64FunctionTest, 10L, 30L);

            Assert.AreEqual(resultCSharp, resultMysql);
        }

#pragma warning disable CS0219, CS0168, IDE0059, IDE0004, IDE0047

        [MySqlFunctionScope(nameof(DeclareInt64FunctionTest))]
        private Int64 DeclareInt64FunctionTest(Int64 a, Int64 b)
        {
            Int64 var1;
            Int64? var2 = null;
            Int64 var3 = 1;

            Int64 var4 = 1 + a;
            Int64 var5 = 1 + a + b;
            Int64 var6 = (1 + (a - b)) * 3;

            Int64 var7, var8, var9 = 4;
            Int64 var10 = 1, var11 = 2, var12 = 3;
            Int64? var13 = 4, var14 = 5, var15 = null;
            Int64? var16 = null, var17, var18 = 9;
            Int64 var19 = a, var20 = b;

            Int64 var21 = 1;
            Int64 var22 = 1 + (Int64)2;
            Int64 var23 = 1 + (2 - ((Int64)3 + 5) - 3);

            Int64 var24 = 5;
            Int64 var25 = 1 + 2 + 3;
            Int64 var26 = (Int64)(1 + 2 + 3) - 2;
            Int64 var27 = 1 + 2 + 3 - (Int64)2;

            Int64 result = var3 + var4 + var5 + var6 + var20;

            return result;
        }

#pragma warning restore CS0219,CS0168,IDE0059,IDE0004,IDE0047

        #endregion

        #region System.Int64

        [TestMethod()]
        public async Task DeclareSystemInt64Test()
        {
            var function = _orm.BuildFunctionCreateStatement<System.Int64, System.Int64, System.Int64>(DeclareSystemInt64FunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareSystemInt64FunctionTest(10L, 30L);
            var resultMysql = await _orm.RunFunctionAsync(DeclareSystemInt64FunctionTest, 10L, 30L);

            Assert.AreEqual(resultCSharp, resultMysql);
        }

#pragma warning disable CS0219, CS0168, IDE0059, IDE0004, IDE0047

        [MySqlFunctionScope(nameof(DeclareSystemInt64FunctionTest))]
        private System.Int64 DeclareSystemInt64FunctionTest(System.Int64 a, System.Int64 b)
        {
            System.Int64 var1;
            System.Int64? var2 = null;
            System.Int64 var3 = 1;

            System.Int64 var4 = 1 + a;
            System.Int64 var5 = 1 + a + b;
            System.Int64 var6 = (1 + (a - b)) * 3;

            System.Int64 var7, var8, var9 = 4;
            System.Int64 var10 = 1, var11 = 2, var12 = 3;
            System.Int64? var13 = 4, var14 = 5, var15 = null;
            System.Int64? var16 = null, var17, var18 = 9;
            System.Int64 var19 = a, var20 = b;

            System.Int64 var21 = 1;
            System.Int64 var22 = 1 + (System.Int64)2;
            System.Int64 var23 = 1 + (2 - ((System.Int64)3 + 5) - 3);

            System.Int64 var24 = 5;
            System.Int64 var25 = 1 + 2 + 3;
            System.Int64 var26 = (System.Int64)(1 + 2 + 3) - 2;
            System.Int64 var27 = 1 + 2 + 3 - (System.Int64)2;

            System.Int64 result = var3 + var4 + var5 + var6 + var20;

            return result;
        }

#pragma warning restore CS0219,CS0168,IDE0059,IDE0004,IDE0047

        #endregion

        #region ulong

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

#pragma warning disable CS0219, CS0168, IDE0059, IDE0004, IDE0047

        [MySqlFunctionScope(nameof(DeclareULongFunctionTest))]
        private ulong DeclareULongFunctionTest(ulong a, ulong b)
        {
            ulong var1;
            ulong? var2 = null;
            ulong var3 = 1;

            ulong var4 = 1 + a;
            ulong var5 = 1 + a + b;
            ulong var6 = (1 + (a + b)) * 3;

            ulong var7, var8, var9 = 4;
            ulong var10 = 1, var11 = 2, var12 = 3;
            ulong? var13 = 4, var14 = 5, var15 = null;
            ulong? var16 = null, var17, var18 = 9;
            ulong var19 = a, var20 = b;

            ulong var21 = 1;
            ulong var22 = 1 + (ulong)2;
            ulong var23 = 1 + (20 + ((ulong)3 + 5) + 3);

            ulong var24 = 5;
            ulong var25 = 1 + 2 + 3;
            ulong var26 = (ulong)(1 + 2 + 3) - 2;
            ulong var27 = 1 + 2 + 3 + (ulong)2;

            ulong result = var3 + var4 + var5 + var6 + var20;

            return result;
        }

#pragma warning restore CS0219, CS0168, IDE0059, IDE0004, IDE0047

        #endregion

        #region UInt64

        [TestMethod()]
        public async Task DeclareUInt64Test()
        {
            var function = _orm.BuildFunctionCreateStatement<UInt64, UInt64, UInt64>(DeclareUInt64FunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareUInt64FunctionTest(10LU, 30LU);
            var resultMysql = await _orm.RunFunctionAsync(DeclareUInt64FunctionTest, 10LU, 30LU);

            Assert.AreEqual(resultCSharp, resultMysql);
        }

#pragma warning disable CS0219, CS0168, IDE0059, IDE0004, IDE0047

        [MySqlFunctionScope(nameof(DeclareUInt64FunctionTest))]
        private UInt64 DeclareUInt64FunctionTest(UInt64 a, UInt64 b)
        {
            UInt64 var1;
            UInt64? var2 = null;
            UInt64 var3 = 1;

            UInt64 var4 = 1 + a;
            UInt64 var5 = 1 + a + b;
            UInt64 var6 = (1 + (a + b)) * 3;

            UInt64 var7, var8, var9 = 4;
            UInt64 var10 = 1, var11 = 2, var12 = 3;
            UInt64? var13 = 4, var14 = 5, var15 = null;
            UInt64? var16 = null, var17, var18 = 9;
            UInt64 var19 = a, var20 = b;

            UInt64 var21 = 1;
            UInt64 var22 = 1 + (UInt64)2;
            UInt64 var23 = 1 + (20 + ((UInt64)3 + 5) + 3);

            UInt64 var24 = 5;
            UInt64 var25 = 1 + 2 + 3;
            UInt64 var26 = (UInt64)(1 + 2 + 3) - 2;
            UInt64 var27 = 1 + 2 + 3 + (UInt64)2;

            UInt64 result = var3 + var4 + var5 + var6 + var20;

            return result;
        }

#pragma warning restore CS0219, CS0168, IDE0059, IDE0004, IDE0047

        #endregion

        #region System.UInt64

        [TestMethod()]
        public async Task DeclareSystemUInt64Test()
        {
            var function = _orm.BuildFunctionCreateStatement<System.UInt64, System.UInt64, System.UInt64>(DeclareSystemUInt64FunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareSystemUInt64FunctionTest(10LU, 30LU);
            var resultMysql = await _orm.RunFunctionAsync(DeclareSystemUInt64FunctionTest, 10LU, 30LU);

            Assert.AreEqual(resultCSharp, resultMysql);
        }

#pragma warning disable CS0219, CS0168, IDE0059, IDE0004, IDE0047

        [MySqlFunctionScope(nameof(DeclareSystemUInt64FunctionTest))]
        private System.UInt64 DeclareSystemUInt64FunctionTest(System.UInt64 a, System.UInt64 b)
        {
            System.UInt64 var1;
            System.UInt64? var2 = null;
            System.UInt64 var3 = 1;

            System.UInt64 var4 = 1 + a;
            System.UInt64 var5 = 1 + a + b;
            System.UInt64 var6 = (1 + (a + b)) * 3;

            System.UInt64 var7, var8, var9 = 4;
            System.UInt64 var10 = 1, var11 = 2, var12 = 3;
            System.UInt64? var13 = 4, var14 = 5, var15 = null;
            System.UInt64? var16 = null, var17, var18 = 9;
            System.UInt64 var19 = a, var20 = b;

            System.UInt64 var21 = 1;
            System.UInt64 var22 = 1 + (System.UInt64)2;
            System.UInt64 var23 = 1 + (20 + ((System.UInt64)3 + 5) + 3);

            System.UInt64 var24 = 5;
            System.UInt64 var25 = 1 + 2 + 3;
            System.UInt64 var26 = (System.UInt64)(1 + 2 + 3) - 2;
            System.UInt64 var27 = 1 + 2 + 3 + (System.UInt64)2;

            System.UInt64 result = var3 + var4 + var5 + var6 + var20;

            return result;
        }

#pragma warning restore CS0219, CS0168, IDE0059, IDE0004, IDE0047

        #endregion
    }
}