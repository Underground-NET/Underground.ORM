using System.Collections.ObjectModel;
using System.Diagnostics;
using Urderground.ORM.Core;
using Urderground.ORM.Core.Attributes;

namespace Underground.ORM.Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Server=localhost;Database=MySqlOrmTests;Uid=root;Pwd=12345678;Pooling=True;";
            var mysql = new OrmEngine(connectionString);

            mysql.ConnectAsync().GetAwaiter().GetResult();

            TableEntity table = new("GUILHERME",
                                              new DateTime(1988, 02, 01),
                                              35);

            mysql.Insert(table);

            var sql = mysql.RunFunction(MetodoFuncao, "string", 2);

            Debug.Print(sql);
        }

        [MySqlFunctionScope("FuncaoTeste")]
        public static int MetodoFuncao(string teste, int valor)
        {
            // TODO: Função que será executado no banco

            int ddd = 2;

            if (ddd == 2)
                ddd = 1;
            else
                ddd = 4;

            uint v1 = 1, v2 = 2, v3 = 3;

            var calc = 1 + (2 + 3 - 4 + (v1 + v2 + v3));

            char chf = (char)(3 + 4 + 5 - 3 * 2 - (char)1 + (char)1 + (char)2), chg = (char)4;
            char che = (char)1;

            string aa;
            string bb = "";
            string cc = "1221";
            string dd = null;
            string ee = "s", ff = "a", gg = "e";
            string hh, ii, jj = "jj";

            float sina;
            float sind = 2;
            float? sinc = null;
            float sine = 1, sinf = 2, sing = 3;
            float sinh, sini, sinj = 9;

            float flu;


            return (int)calc;

            List<string> lista = new();
            List<string> listaa = null;
            List<string> listab;

            string[] lista2 = new string[0];
            IEnumerable<string> lista3 = new string[0] { };
            Collection<string> lista4 = new Collection<string>();
            ICollection<string> lista5 = new Collection<string>();
            IList<string> lista6 = new List<string>();

            //lista.Add("teste");

            object? objeto;
            bool teste1 = true;
            bool teste2 = true;
            bool? teste3 = null;

            char cha = (char)1;
            char chb = 'c';
            char chc = 'a';
            char? chd = null;
            char chh, chi, chj = (char)67;

            byte bya;
            byte byb = 2;
            byte byc = 255;
            byte? byd = null;
            byte bye = 0, byf = 100, byg = 128;
            byte byh, byi, byj = 67;

            sbyte sbya;
            sbyte sbyb = -128;
            sbyte sbyc = 127;
            sbyte? sbyd = null;
            sbyte sbye = 0, sbyf = 100, sbyg = 100;
            sbyte sbyh, sbyi, sbyj = -50;



            short sa;
            short sd = 2;
            short? sc = null;
            short se = 1, sf = 2, sg = 3;
            short sh, si, sj = 9;

            ushort usa;
            ushort usd = 2;
            ushort? usc = null;
            ushort use = 1, usf = 2, usg = 3;
            ushort ush, usi, usj = 9;

            int a;
            int d = 2;
            int? c = null;
            int e = 1, f = 2, g = 3;
            int h, i, j = 9;

            uint ua;
            uint ud = 2;
            uint? uc = null;
            uint ue = 1, uf = 2, ug = 3;
            uint uh, ui, uj = 9;

            long la;
            long ld = 2;
            long? lc = null;
            long le = 1, lf = 2, lg = 3;
            long lh, li, lj = 9;

            ulong ula;
            ulong uld = 2;
            ulong? ulc = null;
            ulong ule = 1, ulf = 2, ulg = 3;
            ulong ulh, uli, ulj = 9;



            decimal deca;
            decimal decd = 2;
            decimal? decc = null;
            decimal dece = 1, decf = 2, decg = 3;
            decimal dech, deci, decj = 9;

            double doua;
            double doud = 2;
            double? douc = null;
            double doue = 1, douf = 2, doug = 3;
            double douh, doui, douj = 9;

            float fla;
            float fld = 2;
            float? flc = null;
            float fle = 1, flf = 2, flg = 3;
            float flh, fli, flj = 9;



            //return lista;
            return chj;
        }
    }
}