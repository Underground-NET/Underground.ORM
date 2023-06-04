using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Underground.ORM.Core;
using Underground.ORM.Core.Attributes;

namespace Underground.ORM.CoreTests.Function
{
    [TestClass()]
    public class DeclareIntTests
    {
        private readonly OrmEngine _orm;

        public DeclareIntTests()
        {
            _orm = OrmEngineTests.OrmEngine.Value;
        }

        #region int

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

#pragma warning disable CS0219, CS0168, IDE0059, IDE0004, IDE0047

        [MySqlFunctionScope(nameof(DeclareIntFunctionTest))]
        private int DeclareIntFunctionTest(int a, int b)
        {
            int var1;
            int? var2 = null;
            int var3 = 1;

            int var4 = 1 + a;
            int var5 = 1 + a + b;
            int var6 = (1 + (a - b)) * 3;

            int var7, var8, var9 = 4;
            int var10 = 1, var11 = 2, var12 = 3;
            int? var13 = 4, var14 = 5, var15 = null;
            int? var16 = null, var17, var18 = 9;
            int var19 = a, var20 = b;

            int var21 = 1;
            int var22 = 1 + 2;
            int var23 = 1 + (2 - (3 + 5) - 3);

            int var24 = 5;
            int var25 = 1 + 2 + 3;
            int var26 = 1 + 2 + 3 - 2;
            int var27 = 1 + 2 + 3 - 2;

            int result = var3 + var4 + var5 + var6 + var20;

            return result;
        }

#pragma warning restore CS0219,CS0168,IDE0059,IDE0004,IDE0047

        #endregion

        #region Int32

        [TestMethod()]
        public async Task DeclareInt32Test()
        {
            var function = _orm.BuildFunctionCreateStatement<Int32, Int32, Int32>(DeclareInt32FunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareInt32FunctionTest(10, 30);
            var resultMysql = await _orm.RunFunctionAsync(DeclareInt32FunctionTest, 10, 30);

            Assert.AreEqual(resultCSharp, resultMysql);
        }

#pragma warning disable CS0219, CS0168, IDE0059, IDE0004, IDE0047

        [MySqlFunctionScope(nameof(DeclareInt32FunctionTest))]
        private Int32 DeclareInt32FunctionTest(Int32 a, Int32 b)
        {
            Int32 var1;
            Int32? var2 = null;
            Int32 var3 = 1;

            Int32 var4 = 1 + a;
            Int32 var5 = 1 + a + b;
            Int32 var6 = (1 + (a - b)) * 3;

            Int32 var7, var8, var9 = 4;
            Int32 var10 = 1, var11 = 2, var12 = 3;
            Int32? var13 = 4, var14 = 5, var15 = null;
            Int32? var16 = null, var17, var18 = 9;
            Int32 var19 = a, var20 = b;

            Int32 var21 = 1;
            Int32 var22 = 1 + 2;
            Int32 var23 = 1 + (2 - (3 + 5) - 3);

            Int32 var24 = 5;
            Int32 var25 = 1 + 2 + 3;
            Int32 var26 = 1 + 2 + 3 - 2;
            Int32 var27 = 1 + 2 + 3 - 2;

            Int32 result = var3 + var4 + var5 + var6 + var20;

            return result;
        }

#pragma warning restore CS0219,CS0168,IDE0059,IDE0004,IDE0047

        #endregion

        #region System.Int32

        [TestMethod()]
        public async Task DeclareSystemInt32Test()
        {
            var function = _orm.BuildFunctionCreateStatement<System.Int32, System.Int32, System.Int32>(DeclareSystemInt32FunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareSystemInt32FunctionTest(10, 30);
            var resultMysql = await _orm.RunFunctionAsync(DeclareSystemInt32FunctionTest, 10, 30);

            Assert.AreEqual(resultCSharp, resultMysql);
        }

#pragma warning disable CS0219, CS0168, IDE0059, IDE0004, IDE0047

        [MySqlFunctionScope(nameof(DeclareSystemInt32FunctionTest))]
        private System.Int32 DeclareSystemInt32FunctionTest(System.Int32 a, System.Int32 b)
        {
            System.Int32 var1;
            System.Int32? var2 = null;
            System.Int32 var3 = 1;

            System.Int32 var4 = 1 + a;
            System.Int32 var5 = 1 + a + b;
            System.Int32 var6 = (1 + (a - b)) * 3;

            System.Int32 var7, var8, var9 = 4;
            System.Int32 var10 = 1, var11 = 2, var12 = 3;
            System.Int32? var13 = 4, var14 = 5, var15 = null;
            System.Int32? var16 = null, var17, var18 = 9;
            System.Int32 var19 = a, var20 = b;

            System.Int32 var21 = 1;
            System.Int32 var22 = 1 + 2;
            System.Int32 var23 = 1 + (2 - (3 + 5) - 3);

            System.Int32 var24 = 5;
            System.Int32 var25 = 1 + 2 + 3;
            System.Int32 var26 = 1 + 2 + 3 - 2;
            System.Int32 var27 = 1 + 2 + 3 - 2;

            System.Int32 result = var3 + var4 + var5 + var6 + var20;

            return result;
        }

#pragma warning restore CS0219,CS0168,IDE0059,IDE0004,IDE0047

        #endregion

        #region uint

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

#pragma warning disable CS0219, CS0168, IDE0059, IDE0004, IDE0047

        [MySqlFunctionScope(nameof(DeclareUIntFunctionTest))]
        private uint DeclareUIntFunctionTest(uint a, uint b)
        {
            uint var1;
            uint? var2 = null;
            uint var3 = 1;

            uint var4 = 1 + a;
            uint var5 = 1 + a + b;
            uint var6 = (1 + (a + b)) * 3;

            uint var7, var8, var9 = 4;
            uint var10 = 1, var11 = 2, var12 = 3;
            uint? var13 = 4, var14 = 5, var15 = null;
            uint? var16 = null, var17, var18 = 9;
            uint var19 = a, var20 = b;

            uint var21 = 1;
            uint var22 = 1 + (uint)2;
            uint var23 = 1 + (20 + ((uint)3 + 5) + 3);

            uint var24 = 5;
            uint var25 = 1 + 2 + 3;
            uint var26 = (uint)(1 + 2 + 3) - 2;
            uint var27 = 1 + 2 + 3 + (uint)2;

            uint result = var3 + var4 + var5 + var6 + var20;

            return result;
        }

#pragma warning restore CS0219,CS0168,IDE0059,IDE0004,IDE0047

        #endregion

        #region UInt32

        [TestMethod()]
        public async Task DeclareUInt32Test()
        {
            var function = _orm.BuildFunctionCreateStatement<UInt32, UInt32, UInt32>(DeclareUInt32FunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareUInt32FunctionTest(10U, 30U);
            var resultMysql = await _orm.RunFunctionAsync(DeclareUInt32FunctionTest, 10U, 30U);

            Assert.AreEqual(resultCSharp, resultMysql);
        }

#pragma warning disable CS0219, CS0168, IDE0059, IDE0004, IDE0047

        [MySqlFunctionScope(nameof(DeclareUInt32FunctionTest))]
        private UInt32 DeclareUInt32FunctionTest(UInt32 a, UInt32 b)
        {
            UInt32 var1;
            UInt32? var2 = null;
            UInt32 var3 = 1;

            UInt32 var4 = 1 + a;
            UInt32 var5 = 1 + a + b;
            UInt32 var6 = (1 + (a + b)) * 3;

            UInt32 var7, var8, var9 = 4;
            UInt32 var10 = 1, var11 = 2, var12 = 3;
            UInt32? var13 = 4, var14 = 5, var15 = null;
            UInt32? var16 = null, var17, var18 = 9;
            UInt32 var19 = a, var20 = b;

            UInt32 var21 = 1;
            UInt32 var22 = 1 + (UInt32)2;
            UInt32 var23 = 1 + (20 + ((UInt32)3 + 5) + 3);

            UInt32 var24 = 5;
            UInt32 var25 = 1 + 2 + 3;
            UInt32 var26 = (UInt32)(1 + 2 + 3) - 2;
            UInt32 var27 = 1 + 2 + 3 + (UInt32)2;

            UInt32 result = var3 + var4 + var5 + var6 + var20;

            return result;
        }

#pragma warning restore CS0219,CS0168,IDE0059,IDE0004,IDE0047

        #endregion

        #region System.UInt32

        [TestMethod()]
        public async Task DeclareSystemUInt32Test()
        {
            var function = _orm.BuildFunctionCreateStatement<System.UInt32, System.UInt32, System.UInt32>(DeclareSystemUInt32FunctionTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = DeclareSystemUInt32FunctionTest(10U, 30U);
            var resultMysql = await _orm.RunFunctionAsync(DeclareSystemUInt32FunctionTest, 10U, 30U);

            Assert.AreEqual(resultCSharp, resultMysql);
        }

#pragma warning disable CS0219, CS0168, IDE0059, IDE0004, IDE0047

        [MySqlFunctionScope(nameof(DeclareSystemUInt32FunctionTest))]
        private System.UInt32 DeclareSystemUInt32FunctionTest(System.UInt32 a, System.UInt32 b)
        {
            System.UInt32 var1;
            System.UInt32? var2 = null;
            System.UInt32 var3 = 1;

            System.UInt32 var4 = 1 + a;
            System.UInt32 var5 = 1 + a + b;
            System.UInt32 var6 = (1 + (a + b)) * 3;

            System.UInt32 var7, var8, var9 = 4;
            System.UInt32 var10 = 1, var11 = 2, var12 = 3;
            System.UInt32? var13 = 4, var14 = 5, var15 = null;
            System.UInt32? var16 = null, var17, var18 = 9;
            System.UInt32 var19 = a, var20 = b;

            System.UInt32 var21 = 1;
            System.UInt32 var22 = 1 + (System.UInt32)2;
            System.UInt32 var23 = 1 + (20 + ((System.UInt32)3 + 5) + 3);

            System.UInt32 var24 = 5;
            System.UInt32 var25 = 1 + 2 + 3;
            System.UInt32 var26 = (System.UInt32)(1 + 2 + 3) - 2;
            System.UInt32 var27 = 1 + 2 + 3 + (System.UInt32)2;

            System.UInt32 result = var3 + var4 + var5 + var6 + var20;

            return result;
        }

#pragma warning restore CS0219,CS0168,IDE0059,IDE0004,IDE0047
        
        #endregion
    }
}