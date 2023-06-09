namespace Underground.ORM.Core.Translator.Syntax.Token.Declaration
{
    public class DefaultToken : MySqlSyntaxToken
    {
        public bool IsDeclaration { get => Is.Declaration; set => Is.Declaration = value; }

        public bool IsDefault { get => Is.Default; set => Is.Default = value; }

        public DefaultToken(string token) :
            base(token)
        {
            IsDefault = true;
            IsDeclaration = true;
        }
    }
}
