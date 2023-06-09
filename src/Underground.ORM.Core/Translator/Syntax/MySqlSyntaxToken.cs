using System.Data;

namespace Underground.ORM.Core.Translator.Syntax
{
    public class IsToken
    {
        public bool DbType { get; set; }

        public bool Var { get; set; }

        public bool String { get; set; }

        public bool VarRef { get; set; }

        public bool CommaConcat { get; set; }

        public bool OpenParenthesis { get; set; }

        public bool CloseParenthesis { get; set; }

        public bool Parenthesis { get; set; }

        public bool Function { get; set; }

        public bool Operator { get; set; }

        public bool Declaration { get; set; }

        public bool Reference { get; set; }

        public bool Attribution { get; set; }

        public bool ConcatFunction { get; set; }

        public bool CoalesceFunction { get; set; }

        public bool CastFunction { get; set; }

        public bool CharFunction { get; set; }

        public bool Marker { get; set; }

        public bool Comment { get; set; }

        public bool Block { get; set; }

        public bool BeginBlock { get; set; }

        public bool EndBlock { get; set; }

        public bool Statement { get; set; }

        public bool ReturnStatement { get; set; }

        public bool Declare { get; set; }

        public bool Default { get; set; }

        public bool Semicolon { get; set; }

        public bool CreateStatement { get; set; }

        public bool FunctionStatement { get; set; }

        public bool NameFunctionStatement { get; set; }

        public bool Comma { get; set; }

        public bool ReturnsStatement { get; set; }
    }

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

        public IsToken Is { get; set; } = new();

        public DbType? DbType { 
            get => dbType;
            protected set 
            { 
                dbType = value;
                Is.String = dbType == System.Data.DbType.String;
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
