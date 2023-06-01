namespace Urderground.ORM.Core.Translator.Pretty
{
    internal class MySqlPretty
    {
        /// <summary>
        /// Add tab in the current line
        /// </summary>
        public HashSet<string> AddTabBeforeReturn { get; set; } = new();

        /// <summary>
        /// Remove tab in the current line
        /// </summary>
        public HashSet<string> RemTabBeforeReturn { get; set; } = new();

        /// <summary>
        /// Add tab on next line
        /// </summary>
        public HashSet<string> AddTabAfterReturn { get; set; } = new();

        /// <summary>
        /// Remove tab on next line
        /// </summary>
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
