namespace Underground.ORM.Core.Syntax
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
}
