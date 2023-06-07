using System.Data;

namespace Underground.ORM.Core.Translator.Syntax
{
    public class MySqlSyntaxToken : ICloneable
    {
        private DbType? dbType = null;

        public string Name { get; private set; }

        public string Token { get; protected set; }

        public int LineNumber { get; private set; }

        public bool StartLine { get; private set; }

        public bool EndLine { get; private set; }

        public bool RightSpace { get; private set; }

        public int ElevatorLevel { get; set; }

        public virtual bool IsDbType { get; set; }

        public virtual bool IsVar { get; set; }

        public virtual bool IsString { get; set; }

        public virtual bool IsVarRef { get; set; }

        public virtual bool IsCommaConcat { get; set; }

        public virtual bool IsOpenParenthesis { get; set; }

        public virtual bool IsCloseParenthesis { get; set; }

        public virtual bool IsParenthesis { get; set; }

        public virtual bool IsFunction { get; set; }

        public virtual bool IsOperator { get; set; }

        public virtual bool IsDeclaration { get; set; }

        public virtual bool IsReference { get; set; }

        public virtual bool IsAttribution { get; set; }

        public virtual bool IsConcatFunction { get; set; }

        public virtual bool IsCoalesceFunction { get; set; }

        public virtual bool IsCastFunction { get; set; }

        public virtual bool IsCharFunction { get; set; }

        public virtual bool IsMarker { get; set; }

        public virtual bool IsComment { get; set; }

        public virtual bool IsBlock { get; set; }

        public virtual bool IsBeginBlock { get; set; }

        public virtual bool IsEndBlock { get; set; }

        public virtual bool IsStatement { get; set; }

        public virtual bool IsReturnStatement { get; set; }

        public DbType? DbType { 
            get => dbType;
            protected set 
            { 
                dbType = value;
                IsString = dbType == System.Data.DbType.String;
            } 
        }

        public bool NotDefinedToken { get; set; }

        public MySqlSyntaxToken? Previous { get; set; }

        public MySqlSyntaxToken? Next { get; set; }

        public MySqlSyntaxToken? Reference { get; set; }

        public override string ToString() => $"Line: {LineNumber}, '{Token}'";

        internal protected void SetLineNumber(int lineNumber) => LineNumber = lineNumber;

        internal protected void SetStartLine(bool startLine) => StartLine = startLine;

        public MySqlSyntaxToken(string token) :
            this(token, newline: false)
        {
            Name = this.GetType().Name;
            NotDefinedToken = Name == nameof(MySqlSyntaxToken);
        }

        public MySqlSyntaxToken(string token,
                                bool newline)
        {
            if (token.Length > 0 && token[^1] == ' ') RightSpace = true;

            Token = token.TrimEnd();
            EndLine = newline;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        internal void SetEndLine()
        {
            EndLine = true;
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
