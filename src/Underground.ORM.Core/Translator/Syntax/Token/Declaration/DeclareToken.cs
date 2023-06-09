namespace Underground.ORM.Core.Translator.Syntax.Token.Declaration
{
    public class DeclareToken : MySqlSyntaxToken
    {
        public bool IsDeclaration { get => Is.Declaration; set => Is.Declaration = value; }

        public bool IsDeclare { get => Is.Declare; set => Is.Declare = value; }

        public DeclareToken(string token) :
            base(token)
        {
            IsDeclare = true;
            IsDeclaration = true;
        }
    }
}
