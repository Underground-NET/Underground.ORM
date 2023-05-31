namespace Urderground.ORM.Core.Translator.List
{
    public class MySqlSyntaxItem
    {
        public string Token { get; private set; }

        public int LineNumber { get; private set; }

        public bool NewLine { get; private set; }

        public bool RightSpace { get; private set; }

        public override string ToString() => $"Line: {LineNumber}, \"{Token}\"";

        internal protected void SetLineNumber(int lineNumber) => LineNumber = lineNumber;

        public MySqlSyntaxItem(string token) : 
            this(token, false)
        {
        }

        public MySqlSyntaxItem(string token, bool newline)
        {
            Token = token;
            NewLine = newline;
             
            if (token.Length > 0 && token[^1] == ' ') RightSpace = true;
        }

        public static implicit operator MySqlSyntaxItem(string token)
        {
            return new MySqlSyntaxItem(token);
        }
    }
}
