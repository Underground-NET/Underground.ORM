namespace Underground.ORM.Core.Syntax.Token.Declaration
{
    public class DefaultToken : SyntaxTokenBase
    {
        public bool IsDeclaration { get => Is.Declaration; set => Is.Declaration = value; }

        public bool IsDefault { get => Is.Default; set => Is.Default = value; }

        public DefaultToken() : this("DEFAULT")
        {
        }

        public DefaultToken(string token) :
            base(token)
        {
            IsDefault = true;
            IsDeclaration = true;
        }
    }
}
