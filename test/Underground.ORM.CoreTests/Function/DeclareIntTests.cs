using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Urderground.ORM.Core;
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
            var function = _orm.BuildFunctionCreateStatement<int, int, int>(FuncaoDeclareIntTest);
            Debug.Print(function.Statement);

            await _orm.UpdateDatabaseAsync(function);

            var resultCSharp = FuncaoDeclareIntTest(10, 30);
            var resultMysql = await _orm.RunFunctionAsync(FuncaoDeclareIntTest, 10, 30);
            
            Assert.AreEqual(resultCSharp, resultMysql);
        }

#pragma warning disable CS0219,CS0168,IDE0059,IDE0004,IDE0047

        [MySqlFunctionScope(nameof(FuncaoDeclareIntTest))]
        private int FuncaoDeclareIntTest(int idade, int dias)
        {
            // Simples
            int var1;
            int? var2 = null;
            int var3 = 1;

            // Usando expressões
            int var4 = 1 + idade;
            int var5 = 1 + idade + dias;
            int var6 = (1 + (idade - dias)) * 3;

            // Usando múltiplas variáveis
            int var7, var8, var9 = 4;
            int var10 = 1, var11 = 2, var12 = 3;
            int? var13 = 4, var14 = 5, var15 = null;
            int? var16 = null, var17, var18 = 9;
            int var19 = idade, var20 = dias;

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

#pragma warning restore CS0219,CS0168,IDE0059,IDE0004,IDE0047
    }
}