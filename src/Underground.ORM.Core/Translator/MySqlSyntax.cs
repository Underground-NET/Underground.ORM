namespace Urderground.ORM.Core.Translator
{
    public class MySqlSyntax
    {
        public string Statement { get; private set; }

        public List<string> Debug { get; private set; }

        public MySqlSyntax(string statement, List<string> debug)
        {
            Statement = statement;
            Debug = debug;
        }
    }
}
