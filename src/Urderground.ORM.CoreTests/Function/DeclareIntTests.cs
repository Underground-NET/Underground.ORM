using Microsoft.VisualStudio.TestTools.UnitTesting;
using Urderground.ORM.Core;
using Urderground.ORM.Core.Attributes;

namespace Urderground.ORM.CoreTests
{
    [TestClass()]
    public class DeclareIntTests
    {
        OrmEngine _engine;

        public DeclareIntTests()
        {
            _engine = new();
        }

        [TestMethod()]
        public void DeclareIntTest()
        {
            var mysqlSyntax = _engine.CreateFunctionStatement(FuncaoDeclareInt, 10, 20);

            // "DECLARE `var27` INT DEFAULT CAST((((1+(2)+((3))))) AS SIGNED INT)-(CAST(((2)) AS SIGNED INT));";
        }

        [MySqlFunctionScope(nameof(DeclareIntTest))]
        private int FuncaoDeclareInt(int idade, int dias)
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

            return idade;
        }
    }
}