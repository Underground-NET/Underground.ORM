namespace Underground.ORM.Core.Syntax.Token.Block
{
    public class BeginBlockToken : SyntaxTokenBase
    {
        public bool IsBlock { get => Is.Block; set => Is.Block = value; }

        public bool IsBeginBlock { get => Is.BeginBlock; set => Is.BeginBlock = value; }

        public BeginBlockToken() : this("BEGIN")
        {
        }

        public BeginBlockToken(string token) :
            base(token)
        {
            IsBlock = true;
            IsBeginBlock = true;
        }
    }
}
