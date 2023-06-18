using System.Data;

namespace Underground.ORM.Core.Syntax
{
    public class SyntaxTokenBase : ICloneable
    {
        private DbType? dbType = null;

        public int Order { get; set; }

        public string Name { get; protected set; }

        public string Token { get; protected set; }

        public int LineNumber { get; protected set; }

        public bool StartLine { get; protected set; }

        public bool EndLine { get; protected set; }

        public bool RightSpace { get; protected set; }

        public int ElevatorLevel { get; protected set; }

        public IsToken Is { get; protected set; } = new();

        public DbType? DbType
        {
            get => dbType;
            protected set
            {
                dbType = value;
                Is.String = dbType == System.Data.DbType.String;
            }
        }

        public bool NotDefinedToken { get; set; }

        public SyntaxTokenBase? Previous { get; set; }

        public SyntaxTokenBase? Next { get; set; }

        public SyntaxTokenBase? Reference { get; set; }

        public override string ToString() => $"Line: {LineNumber}, '{Token}'";

        public void SetElevatorLevel(int elevatorLevel) => ElevatorLevel = elevatorLevel;

        public void SetLineNumber(int lineNumber) => LineNumber = lineNumber;

        public void SetStartLine(bool startLine) => StartLine = startLine;


        public SyntaxTokenBase(string token) :
            this(token, newline: false)
        {
            Name = GetType().Name;
            NotDefinedToken = Name == nameof(SyntaxTokenBase);
            Order = int.MaxValue;
        }

        public SyntaxTokenBase(string token,
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

        public void SetEndLine()
        {
            EndLine = true;
        }

        public static implicit operator SyntaxTokenBase(string token)
        {
            return new SyntaxTokenBase(token);
        }

        public static bool operator ==(SyntaxTokenBase left, string right)
        {
            return left.Token.Equals(right);
        }

        public static bool operator !=(SyntaxTokenBase left, string right)
        {
            return !left.Token.Equals(right);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            throw new NotImplementedException();
        }
    }
}
