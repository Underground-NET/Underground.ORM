namespace Underground.ORM.Core.Translator.Syntax
{
    public class MySqlSyntaxToken : ICloneable
    {
        public string Token { get; protected set; }

        public int LineNumber { get; private set; }

        public bool StartLine { get; private set; }

        public bool EndLine { get; private set; }

        public bool RightSpace { get; private set; }

        public virtual bool IsDbType { get; set; }

        public virtual bool IsVar { get; set; }

        public virtual bool IsString { get; set; }

        public virtual bool IsVarRef { get; set; }

        public int ElevatorLevel { get; set; }

        public virtual MySqlSyntaxToken? Previous { get; set; }

        public virtual MySqlSyntaxToken? Next { get; set; }

        public virtual MySqlSyntaxToken? Reference { get; set; }

        public override string ToString() => $"Line: {LineNumber}, '{Token}'";

        internal protected void SetLineNumber(int lineNumber) => LineNumber = lineNumber;

        internal protected void SetStartLine(bool startLine) => StartLine = startLine;

        public object Clone()
        {
            return MemberwiseClone();
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

        public static bool operator ==(MySqlSyntaxToken left, string right)
        {
            return left.Token.Equals(right);
        }

        public static bool operator !=(MySqlSyntaxToken left, string right)
        {
            return !left.Token.Equals(right);
        }
    }
}
