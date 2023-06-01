namespace Urderground.ORM.Core.Translator.Pretty
{
    internal class MySqlPretty
    {
        public HashSet<string> AddTabBeforeReturn { get; set; } = new();

        public HashSet<string> RemTabBeforeReturn { get; set; } = new();

        public HashSet<string> AddTabAfterReturn { get; set; } = new();

        public HashSet<string> RemTabAfterReturn { get; set; } = new();

        public MySqlPretty()
        {
            #region BEGIN statement

            AddTabAfterReturn.Add("BEGIN");
            RemTabBeforeReturn.Add("END");

            #endregion

            #region RETURNS statement

            AddTabBeforeReturn.Add("RETURNS");
            RemTabAfterReturn.Add("RETURNS");

            #endregion
        }
    }
}
