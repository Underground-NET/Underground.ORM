using System.Data;

namespace Underground.ORM.Core.Translator.Syntax
{
    public class MySqlSyntaxToken : ICloneable
    {
        public string Token { get; private set; }

        public int LineNumber { get; private set; }

        public bool StartLine { get; private set; }

        public bool EndLine { get; private set; }

        public bool RightSpace { get; private set; }

        public DbType? DbType { get; private set; } = null;

        public bool IsVar { get; protected set; }

        public override string ToString() => $"Line: {LineNumber}, '{Token}'";

        internal protected void SetLineNumber(int lineNumber) => LineNumber = lineNumber;

        internal protected void SetStartLine(bool startLine) => StartLine = startLine;

        public object Clone()
        {
            return MemberwiseClone();
        }

        public MySqlSyntaxToken(string token, 
                                DbType dbType) :
            this(token, newline: false)
        {
            DbType = dbType;
        }

        public MySqlSyntaxToken(string token) :
            this(token, newline: false)
        {
        }

        public MySqlSyntaxToken(string token, 
                                bool newline)
        {
            if (token.Length > 0 && token[^1] == ' ') RightSpace = true;

            Token = token.TrimEnd();
            EndLine = newline;
        }

        public static implicit operator MySqlSyntaxToken(string token)
        {
            return new MySqlSyntaxToken(token);
        }
    }
}
