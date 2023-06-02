namespace Underground.ORM.Core.Translator.Syntax
{
    public class MySqlSyntaxItem : ICloneable
    {
        public string Token { get; private set; }

        public int LineNumber { get; private set; }

        public bool StartLine { get; private set; }

        public bool EndLine { get; private set; }

        public bool RightSpace { get; private set; }

        public override string ToString() => $"Line: {LineNumber}, '{Token}'";

        internal protected void SetLineNumber(int lineNumber) => LineNumber = lineNumber;

        internal protected void SetStartLine(bool startLine) => StartLine = startLine;

        public object Clone()
        {
            return MemberwiseClone();
        }

        public MySqlSyntaxItem(string token) :
            this(token, false)
        {
        }

        public MySqlSyntaxItem(string token, bool newline)
        {
            if (token.Length > 0 && token[^1] == ' ') RightSpace = true;

            Token = token.TrimEnd();
            EndLine = newline;
        }

        public static implicit operator MySqlSyntaxItem(string token)
        {
            return new MySqlSyntaxItem(token);
        }
    }
}
