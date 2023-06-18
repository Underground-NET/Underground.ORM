namespace Underground.ORM.Core.Syntax.Token.Declaration
{
    public class DeclareToken : SyntaxTokenBase
    {
        public bool IsDeclaration { get => Is.Declaration; set => Is.Declaration = value; }

        public bool IsDeclare { get => Is.Declare; set => Is.Declare = value; }

        public DeclareToken() : this("DECLARE")
        {
        }

        public DeclareToken(string token) :
            base(token)
        {
            IsDeclare = true;
            IsDeclaration = true;
        }
    }
}
